using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.Models
{
    internal enum ALSRCAL_BOARD:uint
    {
        ALSRCAL_COMP_BOARD=0,
        ALSRCAL_PCAL_BOARD,
        ALSRCAL_TEST_BOARD,
        ALSRCAL_NO_BOARD,
        ALSRCAL_UNKNOWN_BOARD,
    }
}
