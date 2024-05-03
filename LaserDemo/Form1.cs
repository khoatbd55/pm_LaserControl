using DevExpress.Utils.MVVM;
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
    public unsafe partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        Queue<MessageLaserBase> _queueLog=new Queue<MessageLaserBase>();
        object _syncQueue=new object();
        AutoResetEvent _waitQueueLog = new AutoResetEvent(false);
        Thread threadLaser;
        public Form1()
        {
            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
            this.FormClosing += Form1_FormClosing;
            InitializeComponent();
            
        }

        private unsafe void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadLaser != null)
            {
                if (threadLaser.IsAlive)
                    try
                    {
                        threadLaser.Abort();
                    }
                    catch (Exception)
                    {

                    }
            }
            if(_isRunning)
            {
                _isRunning = false;
                if(hBoxes!=0)
                {
                    LaserApi.aLsrCalPcalClose(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                }    
            }    
            timer1.Stop();
        }

        private unsafe void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private unsafe void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
        
        private unsafe void btnConnect_Click(object sender, EventArgs e)
        {
            if (_isRunning == false)
            {
                _isRunning = true;
                btnConnect.Enabled = false;
                btnConnect.Text = "Disconnect";
                threadLaser= new Thread(ProcessLaser);
                threadLaser.Start();
            }
            else
            {
                _isRunning = false;
                btnReset.Enabled = false;
                btnConnect.Enabled = false;
                hBoxes = 0;
            }
        }


        private unsafe void btnReset_Click(object sender, EventArgs e)
        {
            if(_isRunning && hBoxes!=0)
            {
                POS ZeroPos = new POS();
                ZeroPos.dPosition = 0.0;
                LaserApi.aLsrCalPcalResetLaser(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ZeroPos);
            }    
            
        }

        uint hBoxes = 0;
        bool _isRunning = false;

        private unsafe void ProcessLaser()
        {
            {
                ALSRCALRC rc;
                Boxes LsrBox = new Boxes();
                ushort MxBoxes = 4;

                WriteLog("Program Starting");
                rc = LaserApi.aLsrCalFind(&LsrBox.hBoxes[0], (BOARDTYPE*)&LsrBox.PortABrdType[0], (BOARDTYPE*)&LsrBox.PortBBrdType[0], &MxBoxes);
                WriteLog("Find returned " + Convert.ToString(rc) + ", boxes = " + Convert.ToString(MxBoxes));
                for (ushort boxsel = 0; boxsel < MxBoxes; boxsel++)
                {
                    uint hBoxes = LsrBox.hBoxes[boxsel];    // MUST change to ulong for 64-bit targets

                    WriteLog("Box " + Convert.ToString(boxsel + 1));
                    WriteLog("  PortA contains: " + Convert.ToString((BOARDTYPE)LsrBox.PortABrdType[boxsel]));
                    WriteLog("  PortB contains: " + Convert.ToString((BOARDTYPE)LsrBox.PortBBrdType[boxsel]));

                    if ((BOARDTYPE)LsrBox.PortABrdType[boxsel] == BOARDTYPE.ALSRCAL_PCAL_BOARD)
                    {
                        LSRINFO DeviceInfo;

                        rc = LaserApi.aLsrCalGetInfo(hBoxes, BOARDINFOTYPE.BRDINFOTYPE_CARDINFO, &DeviceInfo, (LSRINFO*)0);
                        WriteLog("  GetInfo(E1735A) returns: " + Convert.ToString(rc));
                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                            ShowDevDetails(&DeviceInfo.DevInfo, 2);
                    }

                    if ((BOARDTYPE)LsrBox.PortBBrdType[boxsel] == BOARDTYPE.ALSRCAL_COMP_BOARD)
                    {
                        LSRINFO SensTypeInfo, DeviceInfo;

                        rc = LaserApi.aLsrCalGetInfo(hBoxes, BOARDINFOTYPE.BRDINFOTYPE_CARDINFO, (LSRINFO*)0, &DeviceInfo);
                        WriteLog("  GetInfo(E1736A) returns: " + Convert.ToString(rc));
                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                            ShowDevDetails(&DeviceInfo.DevInfo, 2);

                        rc = LaserApi.aLsrCalGetInfo(hBoxes, BOARDINFOTYPE.BRDINFOTYPE_SENSORSTYPE, (LSRINFO*)0, &SensTypeInfo);
                        WriteLog("    GetInfo(SensorType) returns: " + Convert.ToString(rc));
                        WriteLog("      Sensor 1: " + SensTypeInfo.SnsType.Sensor1Type.ToString("x8"));
                        WriteLog("      Sensor 2: " + SensTypeInfo.SnsType.Sensor2Type.ToString("x8"));
                        WriteLog("      Sensor 3: " + SensTypeInfo.SnsType.Sensor3Type.ToString("x8"));
                        WriteLog("      Sensor 4: " + SensTypeInfo.SnsType.Sensor4Type.ToString("x8"));

                        for (uint SnsNdx = 0; SnsNdx < 4; SnsNdx++)
                        {
                            if ((SensTypeInfo.Array[SnsNdx] == 0xE1737A) || (SensTypeInfo.Array[SnsNdx] == 0xE1738A))
                            {
                                rc = LaserApi.aLsrCalGetInfo(hBoxes, (BOARDINFOTYPE)(SnsNdx + 1), (LSRINFO*)0, &DeviceInfo);
                                WriteLog("    GetInfo(Sensor" + Convert.ToString(SnsNdx + 1) + "Info) returns: " + Convert.ToString(rc));
                                if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                                    ShowDevDetails(&DeviceInfo.DevInfo, 4);
                            }
                        }
                    }
                }

                if ((MxBoxes > 0) && ((BOARDTYPE)LsrBox.PortABrdType[0] == BOARDTYPE.ALSRCAL_PCAL_BOARD))
                {
                    hBoxes = LsrBox.hBoxes[0]; // MUST change to ulong for 64-bit targets
                    rc = LaserApi.aLsrCalPcalOpen(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                    WriteLog("Box 1 Pcal Open returns: " + Convert.ToString(rc));
                    if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                    {
                        rc = LaserApi.aLsrCalPcalInit(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                        WriteLog("  Pcal Init returns: " + Convert.ToString(rc));

                        PCAL PcalSetup = new PCAL();
                        rc = LaserApi.aLsrCalPcalGetSettings(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, &PcalSetup);
                        WriteLog("  Pcal Get Settings returns: " + Convert.ToString(rc));
                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                            ShowLsrSetup(ref PcalSetup);

                        POS ZeroPos = new POS();
                        ZeroPos.dPosition = 0.0;
                        rc = LaserApi.aLsrCalPcalResetLaser(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ZeroPos);
                        WriteLog("  Pcal Reset returns: " + Convert.ToString(rc));
                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                        {
                            POS LivePos;
                            float LiveBeam;
                            uint LiveSmpl, Samples = 0;
                            TRIGSTAT LiveTrig;
                            PosData MyData = new PosData();
                            ALSRCALRC rc_async;
                            EventWaitHandle MyEvent = new EventWaitHandle(false, EventResetMode.AutoReset); // created to avoid null pointer, but is not used below (but could be)

                            for (uint i = 0; i < 10; i++) MyData.LsrPos[i] = (i + 1) * 1.11111;

                            rc = LaserApi.aLsrCalPcalSetSampleMode(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, DCSDRVMODE.DCSDM_NEVER, 0, 0, SMPLMODE.SMPL_SW);
                            WriteLog("  Set Smpl Mode returns: " + Convert.ToString(rc));

                            rc = LaserApi.aLsrCalPcalSetTriggering(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, TRIGMODE.TRIG_NONE, TRIGMODE.TRIG_HW_REC, ZeroPos, ZeroPos);
                            WriteLog("  Set Trigger Mode returns: " + Convert.ToString(rc));

                            rc = LaserApi.aLsrCalPcalSetTimeout(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, 60000);
                            WriteLog("  Set Timeout returns: " + Convert.ToString(rc));

                            rc = LaserApi.aLsrCalPcalGetSettings(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, &PcalSetup);
                            WriteLog("  Pcal Get Settings returns: " + Convert.ToString(rc));
                            if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                                ShowLsrSetup(ref PcalSetup);

                            rc = LaserApi.aLsrCalPcalReadPosAsync(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, (POS*)&MyData.LsrPos[0], (POS*)0, 10, &Samples, (void*)MyEvent.SafeWaitHandle.DangerousGetHandle());
                            WriteLog("  Read Async returns: " + Convert.ToString(rc));
                            if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                            {
                                WriteConnectLaser(true);
                                do
                                {
                                    rc = LaserApi.aLsrCalPcalGetCurState(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, &LivePos, (POS*)0, (POS*)0, &LiveBeam, &LiveSmpl, &LiveTrig);
                                    WriteLog(
                                            "  Pos: " + string.Format("{0,12:0.#####0}", LivePos.dPosition) +
                                            " mm, Beam: " + LiveBeam.ToString("##.0") +
                                            "%, Smpls: " + LiveSmpl.ToString() +
                                            ", Trig: " + LiveTrig.ToString()
                                            );
                                    WritePos(LivePos.dPosition, LiveBeam, LiveSmpl, LiveTrig);
                                    Thread.Sleep(1000);
                                }
                                //while ((LiveSmpl < 10) && (Abort == false) && (MyEvent.WaitOne(0) == false));
                                while ((_isRunning == true));
                            }
                            else
                            {
                                WriteConnectLaser(false);
                            }

                            rc = LaserApi.aLsrCalPcalGetAsyncRC(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, &rc_async);
                            WriteLog("  Get Async Result returns: " + Convert.ToString(rc) + ", Async result:" + Convert.ToString(rc_async));
                            for (uint i = 0; i < 10; i++)
                                WriteLog("    Sample " + i.ToString() + " = " + MyData.LsrPos[i].ToString());
                        }
                        else
                        {
                            WriteConnectLaser(false);
                        }

                        rc = LaserApi.aLsrCalPcalClose(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                        WriteLog("  Pcal Close returns: " + Convert.ToString(rc));
                        //hBoxes = 0;
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
            
        }

        unsafe struct Boxes
        {
            public fixed uint hBoxes[4];    // MUST change to ulong for 64-bit targets (both here and in wrap.cs file)
            public fixed ushort PortABrdType[4];
            public fixed ushort PortBBrdType[4];
        }

        unsafe struct PosData
        {
            public fixed double LsrPos[10];
        }

        unsafe void ShowDevDetails(DEVICEINFO* Info, uint IndentSize)
        {
            StringBuilder temp = new StringBuilder();
            for (uint i = 0; i < 16; i++)
            {
                if (Info->SerialNumber[i] == 0) break;
                temp.Append(Convert.ToChar(Info->SerialNumber[i]));
            }

            StringBuilder Indent = new StringBuilder();
            for (uint i = 0; i < 2 + IndentSize; i++) Indent.Append(" ");

            WriteLog(Indent + " PCB Rev: 0x" + Info->PCBRevision.ToString("x8"));
            WriteLog(Indent + "Sldr Opt: 0x" + Info->SolderOption.ToString("x8"));
            WriteLog(Indent + " MCU Rev: 0x" + Info->MCURevision.ToString("x8"));
            WriteLog(Indent + "Serial #: " + temp.ToString());
            WriteLog(Indent + "Mfg Date: 0x" + Info->ManufactureDate.ToString("x8"));
        }

        unsafe void ShowLsrSetup(ref PCAL Setup)
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

        private unsafe void WriteLog(string message)
        {
            lock (_syncQueue)
            {
                _queueLog.Enqueue(new MessageLaserLog()
                {
                    Str = message
                });
                _waitQueueLog.Set();
            }
        }

        private unsafe void WriteConnectLaser(bool isConnect)
        {
            lock (_syncQueue)
            {
                _queueLog.Enqueue(new MessageLaserConnection()
                {
                    IsConnected = isConnect
                });
            }
        }

        private unsafe void WritePos(double pos,float beam,uint smpl,TRIGSTAT trig)
        {
            lock (_syncQueue)
            {
                _queueLog.Enqueue(new MessageLaserPosition()
                {
                    Pos=pos,
                    Beam=beam,
                    Smpl=smpl,
                    Trig=trig
                });
            }
        }

        private unsafe void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcessLaser();
        }

        private unsafe void timer1_Tick(object sender, EventArgs e)
        {
            MessageLaserBase msg = null;
            lock (_syncQueue)
            {
                if (_queueLog.Count > 0)
                    msg = _queueLog.Dequeue();
            }
            if (msg != null)
            {
                if(msg is MessageLaserLog msgLog)
                {
                    listLog.Items.Add(msgLog.Str);
                    while (listLog.Items.Count > 300)
                    {
                        listLog.Items.RemoveAt(0);
                    }
                    listLog.SelectedIndex=listLog.Items.Count-1;
                }
                else if(msg is MessageLaserPosition msgPos)
                {
                    labelPos.Text = string.Format("{0,12:0.#####0}", msgPos.Pos);
                    var beam = msgPos.Beam;
                    if (beam > 100)
                        beam = 100;
                    progressBarBeam.Position = (int)beam;
                }
                else if(msg is MessageLaserConnection msgConnect)
                {
                    if (msgConnect.IsConnected)
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
                        hBoxes = 0;
                    }
                }
            }
        }
    }
}