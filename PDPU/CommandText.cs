using System;

namespace PDPU
{
    public class CommandText
    {
        //成像类型
        public const UInt32 ImageFullFrame = 0xcc;
        public const UInt32 ImageWindows = 0xaa;

        //LVDS主备份
        public const UInt32 ImageMasterState = 0xaa;
        public const UInt32 ImageSlaveState = 0x55;

        //FPGA主备份
        public const UInt32 FpgaMasterState = 0xaa;
        public const UInt32 FpgaSlaveState = 0x55;

        //秒脉冲检测
        public const UInt32 PpsNormal = 0xaa;
        public const UInt32 PpsFault = 0x55;

        //清空模式
        public const UInt32 FullClear = 0x55;
        public const UInt32 NoClear = 0xaa;

        //增益
        public const UInt32 GainFirstGrade = 0x03;
        public const UInt32 GainSecondGrade = 0x05;
        public const UInt32 GainThirdGrade = 0x0a;

        //积分速度
        public const UInt32 IntegrateFirstGrade = 0x03;
        public const UInt32 IntegrateSecondGrade = 0x05;
        public const UInt32 IntegrateThirdGrade = 0x0a;
        public const UInt32 IntegrateFourthGrade = 0x0c;

        //读出通道
        public const UInt32 LeftChanel = 0x03;
        public const UInt32 RightChanel = 0x05;
        public const UInt32 LeftRightChanel = 0x0a;

        //像元读出速率
        public const UInt32 Pixel200K = 0x33;
        public const UInt32 Pixel150K = 0x55;
        public const UInt32 Pixel100K = 0xaa;
        public const UInt32 Pixel70K = 0xcc;

        //ADC选择指令
        public const UInt32 Adc1 = 0x03;
        public const UInt32 Adc2 = 0x05;
        public const UInt32 Adc1Adc2 = 0x0a;
    }
}
