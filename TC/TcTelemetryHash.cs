using PDPU;
using System;
using System.Collections;

namespace TC
{
    class TcTelemetryHash : TelemetryHash
    {
        public TcTelemetryHash()
        {
            Hashtable workModeTable = new Hashtable();
            Hashtable imageTpyeTable = new Hashtable();
            Hashtable exposeTimeTable = new Hashtable();
            Hashtable imageMasterStateTable = new Hashtable();
            Hashtable fpgaMasterStateTable = new Hashtable();
            Hashtable ppsTable = new Hashtable();
            Hashtable clearModeTable = new Hashtable();
            Hashtable gainTable = new Hashtable();
            Hashtable integrateRateTable = new Hashtable();
            Hashtable readchannelTable = new Hashtable();
            Hashtable pixelReadRateTable = new Hashtable();
            Hashtable windowInforTable = new Hashtable();

            //工作模式
            workModeTable.Add(TcCommandText.StandBy, "待机模式");
            workModeTable.Add(TcCommandText.Observing, "成像模式");
            workModeTable.Add(TcCommandText.Calibration, "定标模式");
            workModeTable.Add(TcCommandText.FocusAdjusting, "调焦模式");
            workModeTable.Add(TcCommandText.BakeOut, "Bake-out模式");
            workModeTable.Add(TcCommandText.Saa, "SAA及地球模式");
            //成像类型
            imageTpyeTable.Add(TcCommandText.ImageFullFrame, "全帧");
            imageTpyeTable.Add(TcCommandText.ImageWindows, "开窗");
            //曝光时间
            for (UInt32 i = 1; i <= 100; i++)
            {
                exposeTimeTable.Add(i, i + "s");
            }
            exposeTimeTable.Add((UInt32)0x00, "150ms");
            exposeTimeTable.Add((UInt32)0x77, "200s");
            exposeTimeTable.Add((UInt32)0x88, "300s");
            exposeTimeTable.Add((UInt32)0x99, "400s");
            exposeTimeTable.Add((UInt32)0xaa, "500s");
            exposeTimeTable.Add((UInt32)0xbb, "1000s");
            exposeTimeTable.Add((UInt32)0xcc, "1500s");
            exposeTimeTable.Add((UInt32)0xdd, "2000s");
            exposeTimeTable.Add((UInt32)0xee, "200ms");
            exposeTimeTable.Add((UInt32)0xff, "400ms");
            //图像输出接口主备状态
            imageMasterStateTable.Add(TcCommandText.ImageMasterState, "主份");
            imageMasterStateTable.Add(TcCommandText.ImageSlaveState, "备份");
            //FPAG主备状态
            fpgaMasterStateTable.Add(TcCommandText.FpgaMasterState, "主份");
            fpgaMasterStateTable.Add(TcCommandText.FpgaSlaveState, "备份");
            //秒脉冲检测
            ppsTable.Add(TcCommandText.PpsNormal, "正常");
            ppsTable.Add(TcCommandText.PpsFault, "异常");
            //清空模式
            clearModeTable.Add(TcCommandText.FullClear, "完全清空");
            clearModeTable.Add(TcCommandText.NoClear, "不清空");
            //CCD增益设置
            gainTable.Add(TcCommandText.GainFirstGrade, "1档");
            gainTable.Add(TcCommandText.GainSecondGrade, "2档");
            gainTable.Add(TcCommandText.GainThirdGrade, "3档");
            //积分速度
            integrateRateTable.Add(TcCommandText.IntegrateFirstGrade, "1档");
            integrateRateTable.Add(TcCommandText.IntegrateSecondGrade, "2档");
            integrateRateTable.Add(TcCommandText.IntegrateThirdGrade, "3档");
            integrateRateTable.Add(TcCommandText.IntegrateFourthGrade, "4档");
            //读出通道
            readchannelTable.Add(TcCommandText.LeftChanel, "左通道");
            readchannelTable.Add(TcCommandText.RightChanel, "右通道");
            readchannelTable.Add(TcCommandText.LeftRightChanel, "左右通道");

            //像元读出速率
            pixelReadRateTable.Add(TcCommandText.Pixel200K, "200KHz");
            pixelReadRateTable.Add(TcCommandText.Pixel150K, "150KHz");
            pixelReadRateTable.Add(TcCommandText.Pixel100K, "100KHz");
            pixelReadRateTable.Add(TcCommandText.Pixel70K, "70KHz");

            //探测开窗信息
            windowInforTable.Add(TcCommandText.WindowFrame, "全帧");

            //加入二级哈希表
            hashTable.Add(TcTelemetryPrameterKey.WorkMode, workModeTable);
            hashTable.Add(TcTelemetryPrameterKey.ImageTpye, imageTpyeTable);
            hashTable.Add(TcTelemetryPrameterKey.ExposeTime, exposeTimeTable);
            hashTable.Add(TcTelemetryPrameterKey.ImageMasterSlave, imageMasterStateTable);
            hashTable.Add(TcTelemetryPrameterKey.FpgaMasterSlave, fpgaMasterStateTable);
            hashTable.Add(TcTelemetryPrameterKey.Pps, ppsTable);
            hashTable.Add(TcTelemetryPrameterKey.ClearMode, clearModeTable);
            hashTable.Add(TcTelemetryPrameterKey.Gain, gainTable);
            hashTable.Add(TcTelemetryPrameterKey.IntegrateRate, integrateRateTable);
            hashTable.Add(TcTelemetryPrameterKey.ReadChannel, readchannelTable);
            hashTable.Add(TcTelemetryPrameterKey.PixelReadRate, pixelReadRateTable);
            hashTable.Add(TcTelemetryPrameterKey.WindowsInfor, windowInforTable);
        }
    }
}
