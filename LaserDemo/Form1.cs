using DevExpress.Data.TreeList;
using DevExpress.Utils.MVVM;
using DevExpress.XtraWaitForm;
using LaserDemo.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserDemo
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        Task _task;
        IntPtr hBoxes = IntPtr.Zero;
        bool _isRunning = false;
        bool _firstClose = true;
        bool _isRequireReset = false;
        WaitForm_Service _waitForm;
        struct Boxes
        {
            public IntPtr[] hBoxes;    // MUST change to ulong for 64-bit targets (both here and in wrap.cs file)
            public ushort[] PortABrdType;
            public ushort[] PortBBrdType;
        }
        public Form1()
        {
            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
            this.FormClosing += Form1_FormClosing;
            InitializeComponent();
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_firstClose)
            {
                _firstClose = false;
                e.Cancel = true;
                await StopAsync();
                await Task.Delay(100);
                e.Cancel = true;
                this.Close();
            }
            else
            {
                try
                {
                    Application.Exit();
                }
                catch (Exception)
                {

                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _waitForm = new WaitForm_Service(this);
        }
        
        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (_isRunning == false)
            {
                _isRunning = true;
                btnConnect.Enabled = false;
                btnConnect.Text = "Disconnect";
                _task = Task.Run(() => ProcessLaser());
            }
            else
            {
                await StopAsync();
            }
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            _isRequireReset = true;
        }

        
        private async Task StopAsync()
        {
            _waitForm.ShowProgressPanel();
            await Task.Delay(200);
            if(_isRunning)
            {
                _isRunning = false;
                await WaitForTask(_task);
            }    
            btnReset.Enabled = false;
            btnConnect.Enabled = true;
            btnConnect.Text = "Connect";
            _isRequireReset = false;
            _waitForm.CloseProgressPanel();
        }

        private async Task WaitForTask(Task task)
        {
            try
            {
                if (task != null && !task.IsCompleted)
                    await task.ConfigureAwait(false);
            }
            catch (Exception)
            {

            }
        }

        private async Task ProcessLaser()
        {
            ALSRCALRC rc;
            Boxes LsrBox = new Boxes()
            {
                hBoxes = new IntPtr[4],
                PortABrdType = new ushort[4],
                PortBBrdType = new ushort[4]
            };
            ushort MxBoxes = 4;

            WriteLog("Program Starting");
            rc = LaserApi.aLsrCalFind(ref LsrBox.hBoxes[0], ref LsrBox.PortABrdType[0], ref LsrBox.PortBBrdType[0], ref MxBoxes);
            WriteLog("Find returned " + Convert.ToString(rc) + ", boxes = " + Convert.ToString(MxBoxes));

            if ((MxBoxes > 0) && ((BOARDTYPE)LsrBox.PortABrdType[0] == BOARDTYPE.ALSRCAL_PCAL_BOARD))
            {
                IntPtr hBoxes = LsrBox.hBoxes[0]; // MUST change to ulong for 64-bit targets

                rc = LaserApi.aLsrCalPcalOpen(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                WriteLog("Box 1 Pcal Open returns: " + Convert.ToString(rc));
                if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                {
                    rc = LaserApi.aLsrCalPcalInit(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                    WriteLog("  Pcal Init returns: " + Convert.ToString(rc));

                    PCAL PcalSetup = new PCAL();
                    rc = LaserApi.aLsrCalPcalGetSettings(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref PcalSetup);
                    WriteLog("  Pcal Get Settings returns: " + Convert.ToString(rc));
                    if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                        ShowLsrSetup(ref PcalSetup);

                    POS ZeroPos = new POS();
                    ZeroPos.dPosition = 0.0;
                    rc = LaserApi.aLsrCalPcalResetLaser(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ZeroPos);
                    WriteLog("  Pcal Reset returns: " + Convert.ToString(rc));
                    if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                    {
                        POS LivePos = new POS();
                        float LiveBeam = 0;
                        uint LiveSmpl = 0, Samples = 0;
                        TRIGSTAT LiveTrig = TRIGSTAT.TRIG_ARMED;
                        ALSRCALRC rc_async = ALSRCALRC.ALSRCAL_APIDLL_ERROR;
                        EventWaitHandle MyEvent = new EventWaitHandle(false, EventResetMode.AutoReset); // created to avoid null pointer, but is not used below (but could be)

                        rc = LaserApi.aLsrCalPcalSetSampleMode(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, DCSDRVMODE.DCSDM_NEVER, 0, 0, SMPLMODE.SMPL_SW);
                        WriteLog("  Set Smpl Mode returns: " + Convert.ToString(rc));

                        rc = LaserApi.aLsrCalPcalSetTriggering(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, TRIGMODE.TRIG_NONE, TRIGMODE.TRIG_HW_REC, ZeroPos, ZeroPos);
                        WriteLog("  Set Trigger Mode returns: " + Convert.ToString(rc));

                        rc = LaserApi.aLsrCalPcalSetTimeout(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, 60000);
                        WriteLog("  Set Timeout returns: " + Convert.ToString(rc));

                        rc = LaserApi.aLsrCalPcalGetSettings(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref PcalSetup);
                        WriteLog("  Pcal Get Settings returns: " + Convert.ToString(rc));
                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                            ShowLsrSetup(ref PcalSetup);

                        POS[] MyData = new POS[10];
                        for (uint i = 0; i < 10; i++)
                            MyData[i].dPosition = (i + 1) * 1.11111;
                        POS pposEncoder = new POS();
                        rc = LaserApi.aLsrCalPcalReadPosAsync(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref MyData[0], ref pposEncoder, 10, ref Samples, MyEvent.SafeWaitHandle.DangerousGetHandle());
                        WriteLog("  Read Async returns: " + Convert.ToString(rc));
                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                        {
                            WriteConnectLaser(true);
                            do
                            {
                                POS posX = new POS();
                                POS posECd = new POS();
                                rc = LaserApi.aLsrCalPcalGetCurState(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref LivePos, ref posX, ref posECd, ref LiveBeam, ref LiveSmpl, ref LiveTrig);
                                WriteLog(
                                        "  Pos: " + string.Format("{0,12:0.#####0}", LivePos.dPosition) +
                                        " mm, Beam: " + LiveBeam.ToString("##.0") +
                                        "%, Smpls: " + LiveSmpl.ToString() +
                                        ", Trig: " + LiveTrig.ToString()
                                        );
                                WritePos(LivePos.dPosition, LiveBeam, LiveSmpl, LiveTrig);
                                if(_isRequireReset)
                                {
                                    _isRequireReset = false;
                                    rc = LaserApi.aLsrCalPcalResetLaser(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ZeroPos);
                                    WriteLog("  Pcal Reset returns: " + Convert.ToString(rc));
                                    rc = LaserApi.aLsrCalPcalSetTriggering(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, TRIGMODE.TRIG_NONE, TRIGMODE.TRIG_HW_REC, ZeroPos, ZeroPos);
                                    WriteLog("  Set Trigger Mode returns: " + Convert.ToString(rc));
                                    
                                }    
                                await Task.Delay(1000);
                            }
                            while (_isRunning);
                        }
                        else
                        {
                            WriteConnectLaser(false);
                        }

                        rc = LaserApi.aLsrCalPcalGetAsyncRC(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref rc_async);
                        WriteLog("  Get Async Result returns: " + Convert.ToString(rc) + ", Async result:" + Convert.ToString(rc_async));
                        for (uint i = 0; i < 10; i++)
                            WriteLog("    Sample " + i.ToString() + " = " + MyData[i].dPosition.ToString());
                    }
                    else
                    {
                        WriteConnectLaser(false);
                    }
                    rc = LaserApi.aLsrCalPcalClose(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                    WriteLog("  Pcal Close returns: " + Convert.ToString(rc));
                }
                else
                {
                    WriteConnectLaser(false);
                }
            }
            else
            {
                WriteConnectLaser(false);
            }
            

        }

        void ShowDevDetails(DEVICEINFO Info, uint IndentSize)
        {
            StringBuilder temp = new StringBuilder();
            for (uint i = 0; i < 16; i++)
            {
                if (Info.SerialNumber[i] == 0) break;
                temp.Append(Convert.ToChar(Info.SerialNumber[i]));
            }

            StringBuilder Indent = new StringBuilder();
            for (uint i = 0; i < 2 + IndentSize; i++) Indent.Append(" ");

            WriteLog(Indent + " PCB Rev: 0x" + Info.PCBRevision.ToString("x8"));
            WriteLog(Indent + "Sldr Opt: 0x" + Info.SolderOption.ToString("x8"));
            WriteLog(Indent + " MCU Rev: 0x" + Info.MCURevision.ToString("x8"));
            WriteLog(Indent + "Serial #: " + temp.ToString());
            WriteLog(Indent + "Mfg Date: 0x" + Info.ManufactureDate.ToString("x8"));
        }

        void ShowLsrSetup(ref PCAL Setup)
        {
            string Indent = "    ";
            WriteLog(Indent + "    Version: " + Setup.version.ToString("x8"));
            WriteLog(Indent + "    Timeout: " + Setup.timeout.ToString());
            WriteLog(Indent + "     Lambda: " + Setup.lambda.ToString());
            WriteLog(Indent + "   DeadPath: " + Setup.deadpath.ToString());
            WriteLog(Indent + "        TCN: " + Setup.tcn.ToString());
            WriteLog(Indent + "      STcal: " + Setup.stcal.ToString());
            WriteLog(Indent + "      AOcal: " + Setup.aocal.ToString());
            WriteLog(Indent + "      Optic: " + Setup.optics.ToString());
            WriteLog(Indent + "      Units: " + Setup.units.ToString());
            WriteLog(Indent + "     Preset: " + Setup.preset.ToString());
            WriteLog(Indent + "  XRes Mode: " + Setup.extresmode.ToString());
            WriteLog(Indent + " Accum Rate: " + Setup.accumrate.ToString());
            WriteLog(Indent + "    NumVals: " + Setup.numvals.ToString());
            WriteLog(Indent + " Smpl  Mode: " + Setup.samplemode.ToString());
            WriteLog(Indent + " Smpl DCsdm: " + Setup.smpl_dcsdm.ToString());
            WriteLog(Indent + " Smpl DCden: " + Setup.smpl_swexten.ToString());
            WriteLog(Indent + "   TB Ntrvl: " + Setup.tbgntrvl.ToString());
            WriteLog(Indent + "    ASM Inc: " + Setup.asm_incrment.ToString());
            WriteLog(Indent + "    ASM Eps: " + Setup.asm_epsilon.ToString());
            WriteLog(Indent + "    ASM Dur: " + Setup.asm_duration.ToString());
            WriteLog(Indent + " Strt Tmode: " + Setup.starttrig.ToString());
            WriteLog(Indent + " Stop Tmode: " + Setup.stoptrig.ToString());
            WriteLog(Indent + "  Strt Tpos: " + Setup.starttrigpos.ToString());
            WriteLog(Indent + "  Stop Tpos: " + Setup.stoptrigpos.ToString());
            WriteLog(Indent + "Ncdr   Type: " + Setup.ncdr_inp.ToString());
            WriteLog(Indent + "Ncdr    Res: " + Setup.ncdr_res.ToString());
            WriteLog(Indent + "Ncdr   Unit: " + Setup.ncdr_unit.ToString());
            WriteLog(Indent + "Ncdr   Hyst: " + Setup.ncdr_hyst.ToString());
            WriteLog(Indent + "Ncdr PreScl: " + Setup.ncdr_scl.ToString());
            WriteLog(Indent + "Ncdr PreSet: " + Setup.ncdr_pre.ToString());
            WriteLog(Indent + "Ncdr    SwL: " + Setup.ncdr_swl.ToString());
        }

        private void WriteLog(string message)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                listLog.Items.Add(message);
                while (listLog.Items.Count > 300)
                {
                    listLog.Items.RemoveAt(0);
                }
                listLog.SelectedIndex = listLog.Items.Count - 1;
            }));
        }

        private void WriteConnectLaser(bool isConnect)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (isConnect)
                {
                    btnConnect.Enabled = true;
                    btnConnect.Text = "Disconnect";
                    btnReset.Enabled = true;
                }
                else
                {
                    btnReset.Enabled = false;
                    btnConnect.Enabled = true;
                    btnConnect.Text = "Connect";
                    _isRunning = false;
                    hBoxes = IntPtr.Zero;
                }
            }));
        }

        private void WritePos(double pos,float beam,uint smpl,TRIGSTAT trig)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                labelPos.Text = string.Format("{0,12:0.#####0}", pos);
                if (beam > 100)
                    beam = 100;
                progressBarBeam.Position = (int)beam;
            }));
        }



    }
}