using PDPU;
using System;

namespace DX
{
    class DxCommandText : CommandText
    {
        //工作模式
        public const UInt32 ImageMode = 0x33;
        public const UInt32 EmptyReadMode = 0x55;
        public const UInt32 backupMode = 0xaa;

        //CCD状态
        public const UInt32 CcdStateNormal = 0x05;
        public const UInt32 CcdStateFault = 0x0a;

        //导星窗口大小
        public const UInt32 WindowSize60x60 = 0x0a;
        public const UInt32 WindowSize15x15 = 0x05;

        //导星窗口数量
        public const UInt32 WindowNum1 = 0x01;
        public const UInt32 WindowNum2 = 0x02;
        public const UInt32 WindowNum3 = 0x03;
        public const UInt32 WindowNum4 = 0x04;
    }
}
