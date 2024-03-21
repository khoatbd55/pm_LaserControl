using System;
using System.Collections.Generic;
using System.Linq;
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

        [StructLayout(LayoutKind.Explicit, Size = 32)]
        public struct TU_LSRINFO_DEVICE_INFO
        {
            [FieldOffset(0)]
            public ulong PCBRevision;

            [FieldOffset(4)]
            public ulong SolderOption;

            [FieldOffset(8)]
            public ulong MCURevision;

            [FieldOffset(12)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] SerialNumber;

            [FieldOffset(28)]
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

        [DllImport("aLsrCalx64.dll", CallingConvention =CallingConvention.Cdecl)]
        public static extern ushort aLsrCalFind(ref IntPtr[] hDev, ref ushort[] PortABrdType, ref ushort[] PortBBrdType, ref ushort spMxBoxes);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort aLsrCalFind(ref IntPtr hDev, ref ushort PortABrdType, ref ushort PortBBrdType, ref ushort spMxBoxes);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalPcalOpen(IntPtr hDev, ushort SlotSelector);


        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalPcalClose(IntPtr hDev, ushort SlotSelector);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalPcalReadPos(IntPtr hDev, ushort SlotSelector,
                                ref POS pposLaser,ref POS pposEncoder,ulong ulSmplQty,ref ulong pulSmplXfr);

        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalGetVersions(IntPtr hDev, ref byte[] HWversion, ref byte[] DLLversion);


        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalGetInfo(IntPtr hDev, ushort InfoType,ref TU_LSRINFO_DEVICE_INFO pPortAInfo,ref TU_LSRINFO_SENSOR_TYPE pPortBInfo);


        [DllImport("aLsrCalx64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint aLsrCalGetInfo(IntPtr hDev, ushort InfoType, 
                        ref TU_LSRINFO_SENSOR_TYPE pPortAInfo, ref TU_LSRINFO_SENSOR_TYPE pPortBInfo);



    }
}
