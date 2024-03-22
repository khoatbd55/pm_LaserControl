using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.Api
{
    internal class LaserApi
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct POS
        {
            [FieldOffset(0)]
            public double dPosition;

            [FieldOffset(0)]
            public Int64 llPosition;

            [FieldOffset(0)]
            public long lRawPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TU_LSRINFO_DEVICE_INFO
        {
            public ulong PCBRevision;

            public ulong SolderOption;

            public ulong MCURevision;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] SerialNumber;

            public ulong ManufactureDate;
        }

        [StructLayout(LayoutKind.Explicit, Size = 32)]
        public struct TU_LSRINFO_SENSOR_TYPE
        {
            [FieldOffset(0)]
            public ulong Sensor1Type;

            [FieldOffset(4)]
            public ulong Sensor2Type;

            [FieldOffset(8)]
            public ulong Sensor3Type;

            [FieldOffset(12)]
            public ulong Sensor4Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PCAL
        {
            public ulong version;  // version number of API DLL
            public ulong timeout;  // in msecs
            public double lambda;  // in nanometers (neg to change dir sense)
            public double deadpath;// in current IO units (0 for angular & straightness)
            public double tcn;     // current total compensation number being used
            public double stcal;   // cal factor for LR (~10.0) or SR(~1.0) straightness optic
            public double aocal;   // cal factor for angular optics ~32.61 mm
            public ushort optics;  // optic type being used
            public ushort units;    // IO units for Pos (Laser & Encoder), Deadpath, and Preset values
            public double preset;  // in current units with current TCN and optics factors applied

            public ushort extresmode;  // extended resolution mode, default value is EXT_RES_NONE (winrt has copy too)
            public ushort accumrate;    // accumulation rate for extended resolution
            public double numvals;         // number of values to accumulate

            public ushort samplemode;    // sample mode, default value is SMPL_SW
            public ushort smpl_dcsdm;  // when to drive daisy-chain sample line
            public int smpl_swexten;      // enable SW sample to drive daisy-chain sample line
            public ulong tbgntrvl;         // number of 1us intervals between samples

            public double asm_incrment;    // auto sample mode increment (in iounits)
            public double asm_epsilon;     // auto sample mode epsilon (in iounits)
            public ulong asm_duration;     // auto sample mode duration count

            public ushort starttrig;     // start data collection trigger mode
            public ushort stoptrig;      // stop data collection trigger mode
            public double starttrigpos;    // start trigger position
            public double stoptrigpos;     // stop trigger position

            public ushort ncdr_inp; // encoder input type
            public double ncdr_res;   // encoder resolution (in ncdr_unit units)
            public ushort ncdr_unit;   // encoder scale units (mm or inch only)
            public byte ncdr_hyst;    // encoder hysteresis setting
            public ushort ncdr_scl;     // encoder prescale factor (hw set to this value - 1)
            public double ncdr_pre;   // encoder preset
            public int ncdr_swl;     // sample encoder with laser flag

        }

        [DllImport("aLsrCalx64.dll", CallingConvention =CallingConvention.Cdecl)]
        public static extern ushort aLsrCalFind(ref IntPtr[] hDev, ref ushort[] PortABrdType, ref ushort[] PortBBrdType, ref ushort spMxBoxes);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort aLsrCalFind(ref IntPtr hDev, ref ushort PortABrdType, ref ushort PortBBrdType, ref ushort spMxBoxes);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalPcalOpen(IntPtr hDev, ushort SlotSelector);


        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalPcalClose(IntPtr hDev, ushort SlotSelector);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalPcalInit(IntPtr hDev, ushort SlotSelector);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalPcalReadPos(IntPtr hDev, ushort SlotSelector,
                                ref POS pposLaser,ref POS pposEncoder,ulong ulSmplQty,ref ulong pulSmplXfr);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalGetVersions(IntPtr hDev, ref byte[] HWversion, ref byte[] DLLversion);


        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalGetInfo(IntPtr hDev, ushort InfoType,ref TU_LSRINFO_DEVICE_INFO pPortAInfo,ref TU_LSRINFO_DEVICE_INFO pPortBInfo);


        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalGetInfo(IntPtr hDev, ushort InfoType, 
                        ref TU_LSRINFO_SENSOR_TYPE pPortAInfo, ref TU_LSRINFO_SENSOR_TYPE pPortBInfo);


        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalPcalGetSettings(IntPtr hDev, ushort SlotSelector, ref PCAL ppcal);




    }
}
