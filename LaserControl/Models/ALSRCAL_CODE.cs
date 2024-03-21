using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.Models
{
    internal enum ALSRCAL_CODE
    {
        ALSRCAL_NO_ERROR,         // No error
        ALSRCAL_HANDLE_ERROR,     // Invalid window or system handle
        ALSRCAL_THREAD_ERROR,     // Error Creating Async Thread
        ALSRCAL_BOARDTYPE_ERROR,  // Incorrect board type for operation

        ALSRCAL_COMP_SENSOR_ERROR,// Sensor Reading Out of Range
        ALSRCAL_COMP_BUSY_ERROR,  // Compensator board busy
        ALSRCAL_COMP_BADMTS,      // Invalid Material Sensor Select value

        ALSRCAL_INVAL_ACCUMRATE,  // Invalid Ext. Res. Accumulation Rate
        ALSRCAL_INVAL_ASMDUR,     // Invalid Auto Sample Mode Duration
        ALSRCAL_INVAL_ASMEPS,     // Invalid Auto Sample Mode Epsilon
        ALSRCAL_INVAL_ASMINC,     // Invalid Auto Sample Mode Increment
        ALSRCAL_INVAL_CAL,        // Invalid Optics Calibration factor
        ALSRCAL_INVAL_DCSDMODE,   // Invalid Daisy Chain Sample Drive Mode
        ALSRCAL_INVAL_EXTRESMD,   // Invalid ext resolution mode
        ALSRCAL_INVAL_HYST,       // Invalid Encoder hysteresis value
        ALSRCAL_INVAL_INPUT,      // Invalid Encoder input type
        ALSRCAL_INVAL_LAMBDA,     // Invalid lambda value
        ALSRCAL_INVAL_OPTICS,     // Invalid optics
        ALSRCAL_INVAL_PRESCL,     // Invalid Encoder pre-scale value
        ALSRCAL_INVAL_RES,        // Invalid Encoder resolution value
        ALSRCAL_INVAL_SMPL_INTRVL,// Invalid TB sampling interval
        ALSRCAL_INVAL_SMPLMODE,   // Invalid sample mode
        ALSRCAL_INVAL_TCN,        // Invalid Total Compensation Number
        ALSRCAL_INVAL_TRIGMODE,   // Invalid trigger mode
        ALSRCAL_INVAL_UNITS,      // Invalid units

        ALSRCAL_AQB_ERROR,        // A quad B error
        ALSRCAL_DATA_OVERFLOW,    // Timebase data overflow
        ALSRCAL_ER_ERROR,         // Extended Resolution measurement error
        ALSRCAL_INIT_ERROR,       // Error Initializing 10887 board
        ALSRCAL_LSR_OFF,          // Laser off or warming up
        ALSRCAL_LSR_OVERFLOW,     // Laser Counter Overflow
        ALSRCAL_MEAS_INPROCESS,   // data collection in process
        ALSRCAL_MEAS_LOL,         // Measure Loss-of-lock occurred
        ALSRCAL_NO_OPTICS,        // No optics in laser path
        ALSRCAL_OPENNED,          // 10887 already opened
        ALSRCAL_REF_LOL,          // Reference Loss-of-lock occurred
        ALSRCAL_SLEW_ERROR,       // Slew error
        ALSRCAL_TBG_ERROR,        // Timebase error
        ALSRCAL_UNARMED,          // 10887 not armed for measurement
        ALSRCAL_UNOPENNED,        // 10887 Must be opened first

        ALSRCAL_CANCELED,         // Operation aborted/canceled by user
        ALSRCAL_MEMALLOCATE_ERROR,// no memory for internal data
        ALSRCAL_NULL_POINTER,     // Un-initialized pointer passed
        ALSRCAL_RANGE_ERROR,      // input parameter out of range
        ALSRCAL_SIMULATED_WARNING,// sw simulation of hw operation
        ALSRCAL_TIMEOUT,          // Operation timed out
        ALSRCAL_TIMEOUT_DATA,     // Timeout Value out of range
        ALSRCAL_UNSUPPORTED_ERROR,// hw operation not supported

        ALSRCAL_APIDLL_ERROR,     // Internal DLL Error - GetLastError()
        ALSRCAL_BASEWDM_ERROR,    // other windows errors
    }
}
