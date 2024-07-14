using DevExpress.Mvvm.Native;
using KUtilities.TaskExtentions;
using LaserCali.Laser.LaserWrap;
using LaserCali.Services.Laser.Events;
using LaserCali.Services.Laser.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserCali.Services.Laser
{
    public class KLaserService
    {

        public delegate void LogEventHandle(object sender, KLaserLog_EventArgs arg);
        public delegate void LaserResultHandle(object sender, KLaserResult_EventArgs arg);
        public delegate void LaserConnectionsHandle(object sender, KLaserConnections_EventArgs arg);    

        public event LogEventHandle OnLog;
        public event LaserResultHandle OnResult;
        public event LaserConnectionsHandle OnConnections;

        CancellationTokenSource _backgroundTokenSource=new CancellationTokenSource();
        Task _task;
        bool _isConnected = false;
        struct Boxes
        {
            public IntPtr[] hBoxes;    // MUST change to ulong for 64-bit targets (both here and in wrap.cs file)
            public ushort[] PortABrdType;
            public ushort[] PortBBrdType;
        }
        bool _isRequireReset = false;
        KLaserOptions _options;

        public KLaserService()
        {

        }

        public void Run(KLaserOptions option)
        {
            _options=option;
            _backgroundTokenSource = new CancellationTokenSource();
            _task = Task.Run(() => ProcessLaser(_backgroundTokenSource.Token),_backgroundTokenSource.Token);
        }

        public async Task StopAsync()
        {
            _backgroundTokenSource?.Cancel();
            await WaitForTask(_task);
        }

        public void Reset()
        {
            _isRequireReset = true;
        }

        private async Task ProcessLaser(CancellationToken c)
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
            IntPtr hBoxes=IntPtr.Zero;
            POS ZeroPos = new POS();
            int step = 0;
            while(!c.IsCancellationRequested)
            {
                try
                {
                    switch (step)
                    {
                        case 0:
                            {
                                WriteConnectLaser(false);
                                rc = LaserApi.aLsrCalFind(ref LsrBox.hBoxes[0], ref LsrBox.PortABrdType[0], ref LsrBox.PortBBrdType[0], ref MxBoxes);
                                WriteLog("Find returned " + Convert.ToString(rc) + ", boxes = " + Convert.ToString(MxBoxes));

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
                                        rc = LaserApi.aLsrCalPcalGetSettings(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref PcalSetup);
                                        WriteLog("  Pcal Get Settings returns: " + Convert.ToString(rc));
                                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                                            ShowLsrSetup(ref PcalSetup);
                                        
                                        ZeroPos.dPosition = 0.0;
                                        rc = LaserApi.aLsrCalPcalResetLaser(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ZeroPos);
                                        WriteLog("  Pcal Reset returns: " + Convert.ToString(rc));
                                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                                        {
                                            uint Samples = 0;

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
                                                
                                                step++;
                                            }

                                            rc = LaserApi.aLsrCalPcalGetAsyncRC(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref rc_async);
                                            WriteLog("  Get Async Result returns: " + Convert.ToString(rc) + ", Async result:" + Convert.ToString(rc_async));
                                            for (uint i = 0; i < 10; i++)
                                                WriteLog("    Sample " + i.ToString() + " = " + MyData[i].dPosition.ToString());
                                        }
                                    }
                                }
                                await Task.Delay(500, c);
                            }
                            break;
                        case 1:
                            {
                                WriteConnectLaser(true);
                                POS LivePos = new POS();
                                POS posX = new POS();
                                POS posECd = new POS();
                                float LiveBeam = 0;
                                uint LiveSmpl = 0;
                                TRIGSTAT LiveTrig = TRIGSTAT.TRIG_ARMED;
                                rc = LaserApi.aLsrCalPcalGetCurState(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref LivePos, ref posX, ref posECd, ref LiveBeam, ref LiveSmpl, ref LiveTrig);
                                if(rc==ALSRCALRC.ALSRCAL_NO_ERROR)
                                {
                                    WriteLog(
                                        "  Pos: " + string.Format("{0,12:0.#####0}", LivePos.dPosition) +
                                        " mm, Beam: " + LiveBeam.ToString("##.0") +
                                        "%, Smpls: " + LiveSmpl.ToString() +
                                        ", Trig: " + LiveTrig.ToString()
                                        );
                                    WritePos(LivePos.dPosition, LiveBeam, LiveSmpl, LiveTrig);
                                    if (_isRequireReset)
                                    {
                                        _isRequireReset = false;
                                        rc = LaserApi.aLsrCalPcalResetLaser(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ZeroPos);
                                        WriteLog("  Pcal Reset returns: " + Convert.ToString(rc));
                                        rc = LaserApi.aLsrCalPcalSetTriggering(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, TRIGMODE.TRIG_NONE, TRIGMODE.TRIG_HW_REC, ZeroPos, ZeroPos);
                                        WriteLog("  Set Trigger Mode returns: " + Convert.ToString(rc));

                                    }
                                    await Task.Delay(_options.MsDelayGetResult);
                                }
                                else
                                {
                                    // đóng kết nối
                                    rc = LaserApi.aLsrCalPcalClose(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                                    WriteLog("  Pcal Close returns: " + Convert.ToString(rc));
                                    step = 0;// quay kết nối trở lại
                                }
                            }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    WriteConnectLaser(false);
                    await Task.Delay(1000, c);
                    WriteLog($"exception laser task , ex:{ex.Message}");
                }
            }
            if (hBoxes != IntPtr.Zero)
            {
                WriteConnectLaser(false);
                rc = LaserApi.aLsrCalPcalClose(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                WriteLog("  Pcal Close returns: " + Convert.ToString(rc));
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
            if (OnLog != null)
            {
                OnLog(this, new KLaserLog_EventArgs(message));
            }
        }

        private void WriteConnectLaser(bool isConnect)
        {
            if (isConnect != _isConnected)
            {
                _isConnected=isConnect;
                if (OnConnections != null)
                {
                    OnConnections(this,new KLaserConnections_EventArgs(isConnect));
                }
            }
        }

        private void WritePos(double pos, float beam, uint smpl, TRIGSTAT trig)
        {
            if (OnResult != null)
            {
                if (beam > 100)
                    beam = 100;
                if (beam < 0)
                    beam = 0;
                OnResult(this,new KLaserResult_EventArgs(pos, beam, smpl, trig));
            }
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




    }
}
