using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserWrap
{
    internal class Program
    {
        //static void ShowDevDetails(DEVICEINFO Info, uint IndentSize)
        //{
        //    StringBuilder temp = new StringBuilder();
        //    for (uint i = 0; i < 16; i++)
        //    {
        //        if (Info->SerialNumber[i] == 0) break;
        //        temp.Append(Convert.ToChar(Info->SerialNumber[i]));
        //    }

        //    StringBuilder Indent = new StringBuilder();
        //    for (uint i = 0; i < 2 + IndentSize; i++) Indent.Append(" ");

        //    Console.WriteLine(Indent + " PCB Rev: 0x" + Info->PCBRevision.ToString("x8"));
        //    Console.WriteLine(Indent + "Sldr Opt: 0x" + Info->SolderOption.ToString("x8"));
        //    Console.WriteLine(Indent + " MCU Rev: 0x" + Info->MCURevision.ToString("x8"));
        //    Console.WriteLine(Indent + "Serial #: " + temp.ToString());
        //    Console.WriteLine(Indent + "Mfg Date: 0x" + Info->ManufactureDate.ToString("x8"));
        //}

        static void ShowLsrSetup(ref PCAL Setup)
        {
            string Indent = "    ";
            Console.WriteLine(Indent + "    Version: " + Setup.version.ToString("x8"));
            Console.WriteLine(Indent + "    Timeout: " + Setup.timeout.ToString());
            Console.WriteLine(Indent + "     Lambda: " + Setup.lambda.ToString());
            Console.WriteLine(Indent + "   DeadPath: " + Setup.deadpath.ToString());
            Console.WriteLine(Indent + "        TCN: " + Setup.tcn.ToString());
            Console.WriteLine(Indent + "      STcal: " + Setup.stcal.ToString());
            Console.WriteLine(Indent + "      AOcal: " + Setup.aocal.ToString());
            Console.WriteLine(Indent + "      Optic: " + Setup.optics.ToString());
            Console.WriteLine(Indent + "      Units: " + Setup.units.ToString());
            Console.WriteLine(Indent + "     Preset: " + Setup.preset.ToString());
            Console.WriteLine(Indent + "  XRes Mode: " + Setup.extresmode.ToString());
            Console.WriteLine(Indent + " Accum Rate: " + Setup.accumrate.ToString());
            Console.WriteLine(Indent + "    NumVals: " + Setup.numvals.ToString());
            Console.WriteLine(Indent + " Smpl  Mode: " + Setup.samplemode.ToString());
            Console.WriteLine(Indent + " Smpl DCsdm: " + Setup.smpl_dcsdm.ToString());
            Console.WriteLine(Indent + " Smpl DCden: " + Setup.smpl_swexten.ToString());
            Console.WriteLine(Indent + "   TB Ntrvl: " + Setup.tbgntrvl.ToString());
            Console.WriteLine(Indent + "    ASM Inc: " + Setup.asm_incrment.ToString());
            Console.WriteLine(Indent + "    ASM Eps: " + Setup.asm_epsilon.ToString());
            Console.WriteLine(Indent + "    ASM Dur: " + Setup.asm_duration.ToString());
            Console.WriteLine(Indent + " Strt Tmode: " + Setup.starttrig.ToString());
            Console.WriteLine(Indent + " Stop Tmode: " + Setup.stoptrig.ToString());
            Console.WriteLine(Indent + "  Strt Tpos: " + Setup.starttrigpos.ToString());
            Console.WriteLine(Indent + "  Stop Tpos: " + Setup.stoptrigpos.ToString());
            Console.WriteLine(Indent + "Ncdr   Type: " + Setup.ncdr_inp.ToString());
            Console.WriteLine(Indent + "Ncdr    Res: " + Setup.ncdr_res.ToString());
            Console.WriteLine(Indent + "Ncdr   Unit: " + Setup.ncdr_unit.ToString());
            Console.WriteLine(Indent + "Ncdr   Hyst: " + Setup.ncdr_hyst.ToString());
            Console.WriteLine(Indent + "Ncdr PreScl: " + Setup.ncdr_scl.ToString());
            Console.WriteLine(Indent + "Ncdr PreSet: " + Setup.ncdr_pre.ToString());
            Console.WriteLine(Indent + "Ncdr    SwL: " + Setup.ncdr_swl.ToString());
        }

        struct Boxes
        {
            public IntPtr[] hBoxes;    // MUST change to ulong for 64-bit targets (both here and in wrap.cs file)
            public ushort[] PortABrdType;
            public ushort[] PortBBrdType;
        }

        static void Main()
        {
            ALSRCALRC rc;
            Boxes LsrBox = new Boxes()
            {
                hBoxes=new IntPtr[4],
                PortABrdType=new ushort[4],
                PortBBrdType=new ushort[4]
            };
            ushort MxBoxes = 4;

            Console.WriteLine("Program Starting");
            rc = LaserApi.aLsrCalFind(ref LsrBox.hBoxes[0],ref LsrBox.PortABrdType[0], ref LsrBox.PortBBrdType[0], ref MxBoxes);
            Console.WriteLine("Find returned " + Convert.ToString(rc) + ", boxes = " + Convert.ToString(MxBoxes));
            //for (ushort boxsel = 0; boxsel < MxBoxes; boxsel++)
            //{
            //    IntPtr hBoxes = LsrBox.hBoxes[boxsel];    // MUST change to ulong for 64-bit targets

            //    Console.WriteLine("Box " + Convert.ToString(boxsel + 1));
            //    Console.WriteLine("  PortA contains: " + Convert.ToString((BOARDTYPE)LsrBox.PortABrdType[boxsel]));
            //    Console.WriteLine("  PortB contains: " + Convert.ToString((BOARDTYPE)LsrBox.PortBBrdType[boxsel]));

            //    //if ((BOARDTYPE)LsrBox.PortABrdType[boxsel] == BOARDTYPE.ALSRCAL_PCAL_BOARD)
            //    //{
            //    //    LSRINFO DeviceInfo;

            //    //    rc = LaserApi.aLsrCalGetInfo(hBoxes, BOARDINFOTYPE.BRDINFOTYPE_CARDINFO, ref DeviceInfo, (LSRINFO*)0);
            //    //    Console.WriteLine("  GetInfo(E1735A) returns: " + Convert.ToString(rc));
            //    //    if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
            //    //        ShowDevDetails(&DeviceInfo.DevInfo, 2);
            //    //}

            //    //if ((BOARDTYPE)LsrBox.PortBBrdType[boxsel] == BOARDTYPE.ALSRCAL_COMP_BOARD)
            //    //{
            //    //    LSRINFO SensTypeInfo, DeviceInfo;

            //    //    rc = LaserApi.aLsrCalGetInfo(hBoxes, BOARDINFOTYPE.BRDINFOTYPE_CARDINFO, (LSRINFO*)0, &DeviceInfo);
            //    //    Console.WriteLine("  GetInfo(E1736A) returns: " + Convert.ToString(rc));
            //    //    if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
            //    //        ShowDevDetails(&DeviceInfo.DevInfo, 2);

            //    //    rc = LaserApi.aLsrCalGetInfo(hBoxes, BOARDINFOTYPE.BRDINFOTYPE_SENSORSTYPE, (LSRINFO*)0, &SensTypeInfo);
            //    //    Console.WriteLine("    GetInfo(SensorType) returns: " + Convert.ToString(rc));
            //    //    Console.WriteLine("      Sensor 1: " + SensTypeInfo.SnsType.Sensor1Type.ToString("x8"));
            //    //    Console.WriteLine("      Sensor 2: " + SensTypeInfo.SnsType.Sensor2Type.ToString("x8"));
            //    //    Console.WriteLine("      Sensor 3: " + SensTypeInfo.SnsType.Sensor3Type.ToString("x8"));
            //    //    Console.WriteLine("      Sensor 4: " + SensTypeInfo.SnsType.Sensor4Type.ToString("x8"));

            //    //    for (uint SnsNdx = 0; SnsNdx < 4; SnsNdx++)
            //    //    {
            //    //        if ((SensTypeInfo.Array[SnsNdx] == 0xE1737A) || (SensTypeInfo.Array[SnsNdx] == 0xE1738A))
            //    //        {
            //    //            rc = LaserApi.aLsrCalGetInfo(hBoxes, (BOARDINFOTYPE)(SnsNdx + 1), (LSRINFO*)0, &DeviceInfo);
            //    //            Console.WriteLine("    GetInfo(Sensor" + Convert.ToString(SnsNdx + 1) + "Info) returns: " + Convert.ToString(rc));
            //    //            if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
            //    //                ShowDevDetails(&DeviceInfo.DevInfo, 4);
            //    //        }
            //    //    }
            //    //}
            //}

            if ((MxBoxes > 0) && ((BOARDTYPE)LsrBox.PortABrdType[0] == BOARDTYPE.ALSRCAL_PCAL_BOARD))
            {
                IntPtr hBoxes = LsrBox.hBoxes[0]; // MUST change to ulong for 64-bit targets

                rc = LaserApi.aLsrCalPcalOpen(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                Console.WriteLine("Box 1 Pcal Open returns: " + Convert.ToString(rc));
                if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                {
                    rc = LaserApi.aLsrCalPcalInit(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                    Console.WriteLine("  Pcal Init returns: " + Convert.ToString(rc));

                    PCAL PcalSetup = new PCAL();
                    rc = LaserApi.aLsrCalPcalGetSettings(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref PcalSetup);
                    Console.WriteLine("  Pcal Get Settings returns: " + Convert.ToString(rc));
                    if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                        ShowLsrSetup(ref PcalSetup);

                    POS ZeroPos = new POS();
                    ZeroPos.dPosition = 0.0;
                    rc = LaserApi.aLsrCalPcalResetLaser(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ZeroPos);
                    Console.WriteLine("  Pcal Reset returns: " + Convert.ToString(rc));
                    if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                    {
                        POS LivePos=new POS();
                        float LiveBeam=0;
                        uint LiveSmpl=0, Samples = 0;
                        TRIGSTAT LiveTrig=TRIGSTAT.TRIG_ARMED;
                        ALSRCALRC rc_async=ALSRCALRC.ALSRCAL_APIDLL_ERROR;
                        bool Abort = false;
                        EventWaitHandle MyEvent = new EventWaitHandle(false, EventResetMode.AutoReset); // created to avoid null pointer, but is not used below (but could be)

                        

                        rc = LaserApi.aLsrCalPcalSetSampleMode(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, DCSDRVMODE.DCSDM_NEVER, 0, 0, SMPLMODE.SMPL_SW);
                        Console.WriteLine("  Set Smpl Mode returns: " + Convert.ToString(rc));

                        rc = LaserApi.aLsrCalPcalSetTriggering(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, TRIGMODE.TRIG_NONE, TRIGMODE.TRIG_HW_REC, ZeroPos, ZeroPos);
                        Console.WriteLine("  Set Trigger Mode returns: " + Convert.ToString(rc));

                        rc = LaserApi.aLsrCalPcalSetTimeout(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, 60000);
                        Console.WriteLine("  Set Timeout returns: " + Convert.ToString(rc));

                        rc = LaserApi.aLsrCalPcalGetSettings(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref PcalSetup);
                        Console.WriteLine("  Pcal Get Settings returns: " + Convert.ToString(rc));
                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                            ShowLsrSetup(ref PcalSetup);

                        POS[] MyData = new POS[10];
                        for (uint i = 0; i < 10; i++)
                            MyData[i].dPosition = (i + 1) * 1.11111;
                        POS pposEncoder=new POS();
                        rc = LaserApi.aLsrCalPcalReadPosAsync(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref MyData[0], ref pposEncoder, 10, ref Samples,MyEvent.SafeWaitHandle.DangerousGetHandle());
                        Console.WriteLine("  Read Async returns: " + Convert.ToString(rc));
                        if (rc == ALSRCALRC.ALSRCAL_NO_ERROR)
                        {
                            Console.WriteLine("  *** PRESS 'a' to Abort or 'r' to record a Pos value ***");
                            do
                            {
                                POS posX = new POS();
                                POS posECd = new POS();
                                rc = LaserApi.aLsrCalPcalGetCurState(hBoxes, SLOTSELECT.ALSRCAL_PORT_A, ref LivePos, ref posX, ref posECd, ref LiveBeam, ref LiveSmpl, ref LiveTrig);
                                Console.WriteLine(
                                        "  Pos: " + string.Format("{0,12:0.#####0}", LivePos.dPosition) +
                                        " mm, Beam: " + LiveBeam.ToString("##.0") +
                                        "%, Smpls: " + LiveSmpl.ToString() +
                                        ", Trig: " + LiveTrig.ToString()
                                        );
                                Thread.Sleep(500);

                                if (Console.KeyAvailable)
                                {
                                    ConsoleKeyInfo cki = Console.ReadKey();
                                    switch ((char)cki.Key)
                                    {
                                        case 'a':
                                        case 'A':
                                            rc = LaserApi.aLsrCalPcalAbort(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                                            Console.WriteLine(" Abort returns: " + Convert.ToString(rc));
                                            Abort = true;
                                            break;

                                        case 'r':
                                        case 'R':
                                            rc = LaserApi.aLsrCalPcalRecord(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                                            Console.WriteLine(" Record returns: " + Convert.ToString(rc));
                                            break;
                                    }
                                }

                                Thread.Sleep(500);
                            }
                            //while ((LiveSmpl < 10) && (Abort == false) && (MyEvent.WaitOne(0) == false));
                            while ((Abort == false));
                        }

                        rc = LaserApi.aLsrCalPcalGetAsyncRC(hBoxes, SLOTSELECT.ALSRCAL_PORT_A,ref rc_async);
                        Console.WriteLine("  Get Async Result returns: " + Convert.ToString(rc) + ", Async result:" + Convert.ToString(rc_async));
                        for (uint i = 0; i < 10; i++)
                            Console.WriteLine("    Sample " + i.ToString() + " = " + MyData[i].dPosition.ToString());
                    }

                    rc = LaserApi.aLsrCalPcalClose(hBoxes, SLOTSELECT.ALSRCAL_PORT_A);
                    Console.WriteLine("  Pcal Close returns: " + Convert.ToString(rc));
                }
            }
        }
    }
}
