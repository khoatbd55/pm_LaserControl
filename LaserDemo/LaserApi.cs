using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LaserDemo
{
    #region aLsrCal_enumerations
    public enum ALSRCALRC : uint
    {
        ALSRCAL_NO_ERROR = 0, // No error
        ALSRCAL_HANDLE_ERROR, // Invalid window or system handle
        ALSRCAL_THREAD_ERROR, // Error Creating Async / Message Thread
        ALSRCAL_BOARDTYPE_ERROR, // Incorrect board type for operation
        ALSRCAL_COMP_SENSOR_ERROR, // Sensor Reading Out of Range
        ALSRCAL_COMP_BUSY_ERROR, // Compensator board busy
        ALSRCAL_COMP_BADMTS, // Invalid Material Sensor Select value
        ALSRCAL_INVAL_ACCUMRATE, // Invalid Ext. Res. Accumulation Rate
        ALSRCAL_INVAL_ASMDUR, // Invalid Auto Sample Mode Duration
        ALSRCAL_INVAL_ASMEPS, // Invalid Auto Sample Mode Epsilon
        ALSRCAL_INVAL_ASMINC, // Invalid Auto Sample Mode Increment
        ALSRCAL_INVAL_CAL, // Invalid Optics Calibration factor
        ALSRCAL_INVAL_DCSDMODE, // Invalid Daisy Chain Sample Drive Mode
        ALSRCAL_INVAL_EXTRESMD, // Invalid ext resolution mode
        ALSRCAL_INVAL_HYST, // Invalid Encoder hysteresis value
        ALSRCAL_INVAL_INPUT, // Invalid Encoder input type
        ALSRCAL_INVAL_LAMBDA, // Invalid lambda value
        ALSRCAL_INVAL_OPTICS, // Invalid optics
        ALSRCAL_INVAL_PRESCL, // Invalid Encoder pre-scale value
        ALSRCAL_INVAL_RES, // Invalid Encoder resolution value
        ALSRCAL_INVAL_SMPL_INTRVL, // Invalid TB sampling interval
        ALSRCAL_INVAL_SMPLMODE, // Invalid sample mode
        ALSRCAL_INVAL_TCN, // Invalid Total Compensation Number
        ALSRCAL_INVAL_TRIGMODE, // Invalid trigger mode
        ALSRCAL_INVAL_UNITS, // Invalid units
        ALSRCAL_AQB_ERROR, // A quad B error
        ALSRCAL_DATA_OVERFLOW, // Timebase data overflow
        ALSRCAL_ER_ERROR, // Extended Resolution meas error
        ALSRCAL_INIT_ERROR, // Error Initializing axis module
        ALSRCAL_LSR_OFF, // Laser off or warming up
        ALSRCAL_LSR_OVERFLOW, // Laser Counter Overflow
        ALSRCAL_MEAS_INPROCESS, // data collection in process
        ALSRCAL_MEAS_LOL, // Measure Loss-of-lock occurred
        ALSRCAL_NO_OPTICS, // No optics in laser path
        ALSRCAL_OPENNED, // axis module already opened
        ALSRCAL_REF_LOL, // Reference Loss-of-lock occurred
        ALSRCAL_SLEW_ERROR, // Slew error
        ALSRCAL_TBG_ERROR, // Timebase error
        ALSRCAL_UNARMED, // axis not armed for measurement
        ALSRCAL_UNOPENNED, // axis module must be opened first
        ALSRCAL_CANCELED, // Operation canceled by user
        ALSRCAL_MEMALLOCATE_ERROR, // no memory for internal data
        ALSRCAL_NULL_POINTER, // Un-initialized pointer passed
        ALSRCAL_RANGE_ERROR, // input parameter out of range
        ALSRCAL_SIMULATED_WARNING, // sw simulation of hw operation
        ALSRCAL_TIMEOUT, // Operation timed out
        ALSRCAL_TIMEOUT_DATA, // Timeout Value out of range
        ALSRCAL_UNSUPPORTED_ERROR, // hw operation not supported
        ALSRCAL_APIDLL_ERROR, // Internal DLL Error - GetLastError()
        ALSRCAL_BASEWDM_ERROR, // other windows errors
    };

    //public const int ALSRCAL_MSGBASE = WM_USER;
    public enum ALSRCAL_PUBLIC_MSG : int
    {
        RSM_DATAAVAIL = 0x0400, // Laser pos. data available
        RSM_DATAEND, // Data acquisition completed
        RSM_PCALCANCEL, // operation cancelled
        RSM_PCALERROR, // hardware error occurred
    };

    public enum ACCUMRATE : ushort
    {
        ACCUM_RATE_HIGH = 0,    // Accumulation rate = ½ ref frequency
        ACCUM_RATE_LOW,         // Accumulation rate = ¼ ref frequency
    }

    public enum BOARDTYPE : ushort
    {
        ALSRCAL_COMP_BOARD = 0,
        ALSRCAL_PCAL_BOARD,
        ALSRCAL_TEST_BOARD,
        ALSRCAL_NO_BOARD,
        ALSRCAL_UNKNOWN_BOARD,
    };

    public enum BOARDINFOTYPE : ushort
    {
        BRDINFOTYPE_CARDINFO = 0, // gets DevInfo for specified card
        BRDINFOTYPE_SENSOR1INFO, // gets DevInfo for sensor on port 1
        BRDINFOTYPE_SENSOR2INFO, // … 2
        BRDINFOTYPE_SENSOR3INFO, // … 3
        BRDINFOTYPE_SENSOR4INFO, // … 4
        BRDINFOTYPE_SENSORSTYPE // gets SnsType info for comp board
    };

    public enum COMPSEL : ushort
    {
        COMP_SEL_MT1 = 1,
        COMP_SEL_MT2,
        COMP_SEL_MT3,
    };

    public enum DCSDRVMODE : ushort
    {
        DCSDM_NEVER = 0, // Do not drive this line
        DCSDM_ALWAYS, // Always drive this line
        DCSDM_TRIGGERED // Only drive it if Trigger==TRIG_ACTIVE
    };

    public enum ENCINPUT : ushort
    {
        ENC_INPUT_AQB = 0, // Encoder input = A quad B
        ENC_INPUT_UPDN, // Encoder input = up/down counts
    };

    public enum EXTRESMODE : ushort
    {
        EXT_RES_NONE = 0, // Ext. resolution mode = none
        EXT_RES_AVG, // Ext. resolution mode = average
        EXT_RES_INT // Ext. resolution mode = internal
    };

    public enum LEDSELECT : ushort
    {
        ALSRCAL_ONLINE_LED = 2, // COM LED for E173x
        ALSRCAL_ERROR_LED, // R/W LED for E173x
    };

    public enum LEDSELECT_E3X : ushort
    {
        ALSRCAL_E3XRDY_LED = 0, // RDY LED for E173x
        ALSRCAL_E3XHS_LED, // H.S. LED for E173x
        ALSRCAL_E3XCOM_LED, // COM LED for E173x
        ALSRCAL_E3XRW_LED, // R/W LED for E173x (use this one)
    };

    public enum OPTICS : ushort
    {
        OP_LINEAR = 0, // Linear Optics
        OP_PLANE_MIRROR, // Plane Mirror Optics
        OP_HIGH_RES, // High Resolution Optics
        OP_ANGULAR, // Angular Optics
        OP_STRAIGHTNESS // Straightness optics
    };

    public enum SLOTSELECT : ushort
    {
        ALSRCAL_PORT_A = 0, // on right when viewed from ream panel
        ALSRCAL_PORT_B, // on left when viewed from rear panel
    };

    public enum SMPLMODE : ushort
    {
        SMPL_SW = 0, // Software initiated sampling
        SMPL_RECBTN, // Record button initiated sampling
        SMPL_EXT, // External PCAL initiated sampling
        SMPL_TB, // Timebase initiated sampling
        SMPL_AUTO, // Automatic Position initiated sampling
        SMPL_ENCODER, // Encoder initiated sampling
        SMPL_AQBPIN8, // A quad B Cable pin-8 sampling
        SMPL_EXTHDR, // External Higher Data Rate (>20Hz)
    };

    public enum TCNMTSSEL : ushort
    {
        TCN_MTxxx = 0, // no Mat Temp Sensors used for TCN calculation
        TCN_MTxx1, // include reading of MT1 only
        TCN_MTx2x, // include reading of MT2 only
        TCN_MTx21, // ...
        TCN_MT3xx,
        TCN_MT3x1,
        TCN_MT32x,
        TCN_MT321,
    };

    public enum TRIGMODE : ushort
    {
        TRIG_NONE = 0, // No Trigger qualification
        TRIG_SW_REC, // Trigger on S/W record button
        TRIG_HW_REC, // Trigger on H/W record button
        TRIG_POS_LT, // Trigger on position < specified value
        TRIG_POS_GT // Trigger on position > specified value
    };

    public enum TRIGSTAT : ushort
    {
        TRIG_ARMED = 0, // Driver testing for start condition
        TRIG_ACTIVE, // Driver testing for stop condition
        TRIG_DONE // Driver not looking for any trigger
    };

    public enum UNITS : ushort
    {
        UNITS_LIN_MM = 0, // Millimeters
        UNITS_LIN_IN, // Inches
        UNITS_ANG_DEG, // Degrees
        UNITS_ANG_ARC, // Arc seconds
        UNITS_ANG_RAD, // Micro-Radians
        UNITS_RAW // Units of raw lambda
    };

    #endregion aLsrCal_enumerations

    #region aLsrCal_structures
    [StructLayout(LayoutKind.Sequential)]
    public struct DEVICEINFO
    {
        public uint PCBRevision; // MSB=Major Rev ... LSB=Debug Rev
        public uint SolderOption; // ditto
        public uint MCURevision; // ditto
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] SerialNumber; // 15-char (8-bit) null terminated c string
        public uint ManufactureDate; // #days since 12/30/1899
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SENSORTYPE
    {
        public uint Sensor1Type; // All contain one of following:
        public uint Sensor2Type; // 0x00000000: no sensor,
        public uint Sensor3Type; // 0x00E1737A: E1737A Mat Sensor
        public uint Sensor4Type; // 0x00E1738A: E1738A Air Sensor
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct LSRINFO
    {
        [FieldOffset(0)]
        public DEVICEINFO DevInfo; // for most InfoType selectors

        [FieldOffset(0)]
        public SENSORTYPE SnsType; // only 'SENSORTYPE InfoType selector

        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Array;    // to allow for loop iteration over contents
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PCAL
    {
        public uint version; // internal DLL version number ("reversed" string format, eg. "A.1.23" = 0x41313233)
        public uint timeout; // timeout duration in ms, default = DEFAULT_TIMEOUT
        public double lambda; // laser head wavelength (in nm), default = LAMBDA_5517B
        public double deadpath; // deadpath in current IO units
        public double tcn; // total compensation number, default = STANDARD_AIR
        public double stcal; // straightness optics calibration number, default = 1.0
        public double aocal; // angular optics factor (in mm), default value is 32.61
        public OPTICS optics; // optics used, default value is OP_LINEAR
        public UNITS units; // io units for results, etc.
        public double preset; // laser preset
        public EXTRESMODE extresmode; // extended resolution mode, default value is EXT_RES_NONE (5529 only)
        public ACCUMRATE accumrate; // accumulation rate for extended resolution (5529 only)
        public double numvals; // # of values to accumulate (5529 only)
        public SMPLMODE samplemode; // sample mode, default value is SMPL_SW
        public DCSDRVMODE smpl_dcsdm; // when to drive daisy-chain sample line
        public uint smpl_swexten; // enable SW sample to drive daisy-chain sample line
        public uint tbgntrvl; // number of 10us intervals between samples
        public double asm_incrment; // auto sample mode increment value
        public double asm_epsilon; // auto sample mode epsilon value
        public uint asm_duration; // auto sample mode duration value
        public TRIGMODE starttrig; // start data collection trigger mode
        public TRIGMODE stoptrig; // stop data collection trigger mode
        public double starttrigpos; // start trigger position
        public double stoptrigpos; // stop trigger position
        public ENCINPUT ncdr_inp; // encoder input type
        public double ncdr_res; // encoder resolution (in ncdr_unit units)
        public UNITS ncdr_unit; // encoder scale units (mm or inch only)
        public ushort ncdr_hyst; // encoder hysteresis setting
        public ushort ncdr_scl; // encoder prescale factor (hw set to this value - 1)
        public double ncdr_pre; // encoder preset
        public uint ncdr_swl; // sample encoder with laser flag
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct POS
    {
        [FieldOffset(0)] public long llPosition; // 64-bit integer format
        [FieldOffset(0)] public double dPosition; // 64-bit floating point format
        [FieldOffset(0)] public int lRawPos; // 32-bit integer format
    }
    #endregion aLsrCal_structures

    public class LaserApi
    {
        #region aLsrCal_general
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalBlink(IntPtr hDev, LEDSELECT LEDSelector);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalFind(ref IntPtr hBoxes, ref ushort PortABrdType, ref ushort PortBBrdType, ref ushort spMxBoxes);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalGetVersions(IntPtr hDev, ref byte HWversion, ref byte DLLversion);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalClose(IntPtr hDev, SLOTSELECT SlotSelector);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalInit(IntPtr hDev, SLOTSELECT SlotSelector);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalOpen(IntPtr hDev, SLOTSELECT SlotSelector);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetTimeout(IntPtr hDev, SLOTSELECT SlotSelector, uint ulInterval);
        #endregion aLsrCal_general

        #region aLsrCal_comp
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompSelfCal(IntPtr hDev, SLOTSELECT SlotSelector);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompMux_w(IntPtr hDev, SLOTSELECT SlotSelector, ushort mux);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompADC_r(IntPtr hDev, SLOTSELECT SlotSelector, ref ushort value);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompVolt_r(IntPtr hDev, SLOTSELECT SlotSelector, ref float volts);

        // NOTE - next two routines may take up to ~0.5 sec to return
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompSampleV_r(IntPtr hDev, SLOTSELECT SlotSelector, ref float volts);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompMuxSampleV_r(IntPtr hDev, SLOTSELECT SlotSelector, ushort mux, ref float volts);

        // NOTE - next seven routines may take up to ~0.5 sec to return
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompAirTempC_r(IntPtr hDev, SLOTSELECT SlotSelector, ref float degC);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompAirTempF_r(IntPtr hDev, SLOTSELECT SlotSelector, ref float degF);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompAirPresMM_r(IntPtr hDev, SLOTSELECT SlotSelector, ref float mmHg);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompAirPresIN_r(IntPtr hDev, SLOTSELECT SlotSelector, ref float inHg);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompAirHumid_r(IntPtr hDev, SLOTSELECT SlotSelector, ref float percent);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompMatTempC_r(IntPtr hDev, SLOTSELECT SlotSelector, COMPSEL sensor, ref float degC);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompMatTempF_r(IntPtr hDev, SLOTSELECT SlotSelector, COMPSEL sensor, ref float degF);

        // NOTE - next two routines take ~0.5 sec to return (~0.8 sec for 55292 box)
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompAirCMM_r(IntPtr hDev, ushort SlotSelector, ref float pValues);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompAirFIN_r(IntPtr hDev, ushort SlotSelector, ref float pValues);

        // NOTE - next routine takes > 1 second to return - best if run in own thread.
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompTCN(IntPtr hDev, SLOTSELECT SlotSelector, float CoefExpC, TCNMTSSEL mts_sel, ref double TCN);

        // NOTE - next two routines take ~0.5 * (1 + # MatSensors) sec to return (>1 sec for 55292 box)
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompTCNcmm(IntPtr hDev, ushort SlotSelector, ref float pFloatArray, ushort mts_sel, ref double pTCN);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalCompTCNfin(IntPtr hDev, ushort SlotSelector, ref float pFloatArray, ushort mts_sel, ref double pTCN);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double
        aLsrCalCompTCNmet(float atC, float apMM, float rhP, float mtC, float CoefExpC);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double
        aLsrCalCompTCNeng(float atF, float apIN, float rhP, float mtF, float CoefExpF);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float
        aLsrCalCompApABSmet(float bapMM, float altM);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float
        aLsrCalCompApABSeng(float bapIN, float altFt);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float
        aLsrCalCompV2TF(float volts);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float
        aLsrCalCompV2TC(float volts);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float
        aLsrCalCompV2PIN(float volts);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float
        aLsrCalCompV2PMM(float volts);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float
        aLsrCalCompV2RHP(float volts);
        #endregion aLsrCal_comp

        #region aLsrCal_pcal
        // Calibrator Setup Routines
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalGetSettings(IntPtr hDev, SLOTSELECT SlotSelector, ref PCAL ppcal);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetCalFactor(IntPtr hDev, SLOTSELECT SlotSelector, double stcal, double aocal);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetDeadpath(IntPtr hDev, SLOTSELECT SlotSelector, double dDeadpath);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetLambda(IntPtr hDev, SLOTSELECT SlotSelector, double dLambda);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetOptics(IntPtr hDev, SLOTSELECT SlotSelector, OPTICS optics);

        //[DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern ALSRCALRC
        //aLsrCalPcalSetRcrdFunction(IntPtr hDev, SLOTSELECT SlotSelector, void* callback, uint callbackparam);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetSampleMode(IntPtr hDev, SLOTSELECT SlotSelector, DCSDRVMODE dcsdrvmode, uint swextdrv,
                    uint ncdrwlsr, SMPLMODE smplmode);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetTCN(IntPtr hDev, SLOTSELECT SlotSelector, double dTCN);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetTimebaseInterval(IntPtr hDev, SLOTSELECT SlotSelector, uint ulInterval);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetTriggering(IntPtr hDev, SLOTSELECT SlotSelector, TRIGMODE trigmodeStart,
                    TRIGMODE trigmodeStop, POS posStartTrig, POS posStopTrig);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetUnits(IntPtr hDev, SLOTSELECT SlotSelector, UNITS units);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetupAutoSample(IntPtr hDev, SLOTSELECT SlotSelector, double increment,
                    double epsilon, uint duration);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetupEncoder(IntPtr hDev, SLOTSELECT SlotSelector, ENCINPUT encinput, double dResolution,
                    UNITS units, uint ulPrescale, uint ulHysteresis);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalSetupExtRes(IntPtr hDev, SLOTSELECT SlotSelector, EXTRESMODE extresmode,
                    ACCUMRATE accumrate, uint ulNVA);

        // Calibrator Operation Routines
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalAbort(IntPtr hDev, SLOTSELECT SlotSelector);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalGetAsyncRC(IntPtr hDev, SLOTSELECT SlotSelector, ref ALSRCALRC returncode);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalGetCurState(IntPtr hDev, SLOTSELECT SlotSelector, ref POS pposLaser, ref POS pposLsrXRes,
                    ref POS pposEncoder, ref float pulBeamStrength, ref uint pulSamples, ref TRIGSTAT pTrigstat);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalReadPos(IntPtr hDev, SLOTSELECT SlotSelector, ref POS pposLaser, ref POS pposEncoder,
                    uint ulSmplQty, ref uint pulSmplXfr);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalReadPosAsync(IntPtr hDev, SLOTSELECT SlotSelector, ref POS pposLaser, ref POS pposEncoder,
                    uint ulSmplQty, ref uint pulSmplXfr, IntPtr hNotify);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalRecord(IntPtr hDev, SLOTSELECT SlotSelector);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalResetEncoder(IntPtr hDev, SLOTSELECT SlotSelector, POS preset);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalResetLaser(IntPtr hDev, SLOTSELECT SlotSelector, POS preset);

        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalPcalTriggerArm(IntPtr hDev, SLOTSELECT SlotSelector);

        // Calibrator Information Routines
        [DllImport("aLsrCalApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ALSRCALRC
        aLsrCalGetInfo(IntPtr hDev, BOARDINFOTYPE InfoType, ref LSRINFO pPortAInfo, ref LSRINFO pPortBInfo);
        #endregion aLsrCal_pcal
    }
}
