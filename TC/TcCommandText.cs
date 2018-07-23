using PDPU;
using System;

namespace TC
{
    class TcCommandText : CommandText
    {
        //工作模式
        public const UInt32 StandBy = 0x11;
        public const UInt32 Observing = 0x22;
        public const UInt32 Calibration = 0x33;
        public const UInt32 FocusAdjusting = 0x44;
        public const UInt32 BakeOut = 0x55;
        public const UInt32 Saa = 0x66;

        //开窗大小
        public const UInt32 WindowFrame = 0xcccc;
    }
}
