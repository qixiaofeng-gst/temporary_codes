using DX;
using PDPU;
using System.Collections;

namespace DXDxCommandText
{
    class DxTelemetryHash : TelemetryHash
    {
        public DxTelemetryHash()
        {
            Hashtable workModeTable = new Hashtable();
            Hashtable imageTpyeTable = new Hashtable();
            Hashtable imageMasterStateTable = new Hashtable();
            Hashtable fpgaMasterStateTable = new Hashtable();
            Hashtable ppsTable = new Hashtable();
            Hashtable clearModeTable = new Hashtable();
            Hashtable pixelReadRateTable = new Hashtable();
            Hashtable gainTable = new Hashtable();

            Hashtable integrateRateTable = new Hashtable();
            Hashtable readchannelTable = new Hashtable();
            
            Hashtable windowInforTable = new Hashtable();
            Hashtable ccdStateTable = new Hashtable();
            Hashtable adcSelectTable = new Hashtable();

            //工作模式
            workModeTable.Add(DxCommandText.ImageMode, "成像");
            workModeTable.Add(DxCommandText.EmptyReadMode, "空读");
            workModeTable.Add(DxCommandText.backupMode, "备用");
            //成像类型
            imageTpyeTable.Add(DxCommandText.ImageFullFrame, "全帧");
            imageTpyeTable.Add(DxCommandText.ImageWindows, "开窗");
            //图像输出接口主备状态
            imageMasterStateTable.Add(DxCommandText.ImageMasterState, "接口1");
            imageMasterStateTable.Add(DxCommandText.ImageSlaveState, "接口2");
            //FPAG主备状态
            fpgaMasterStateTable.Add(DxCommandText.FpgaMasterState, "主份");
            fpgaMasterStateTable.Add(DxCommandText.FpgaSlaveState, "备份");
            //秒脉冲检测
            ppsTable.Add(DxCommandText.PpsNormal, "正常");
            ppsTable.Add(DxCommandText.PpsFault, "异常");
            //清空模式
            clearModeTable.Add(DxCommandText.FullClear, "完全清空");
            clearModeTable.Add(DxCommandText.NoClear, "不清空");
            //CCD增益设置
            gainTable.Add(DxCommandText.GainFirstGrade, "1档");
            gainTable.Add(DxCommandText.GainSecondGrade, "2档");
            gainTable.Add(DxCommandText.GainThirdGrade, "3档");
            //积分速度
            integrateRateTable.Add(DxCommandText.IntegrateFirstGrade, "1档");
            integrateRateTable.Add(DxCommandText.IntegrateSecondGrade, "2档");
            integrateRateTable.Add(DxCommandText.IntegrateThirdGrade, "3档");
            integrateRateTable.Add(DxCommandText.IntegrateFourthGrade, "4档");
            //读出通道
            readchannelTable.Add(DxCommandText.LeftChanel, "左通道");
            readchannelTable.Add(DxCommandText.RightChanel, "右通道");
            readchannelTable.Add(DxCommandText.LeftRightChanel, "左右通道");

            //像元读出速率
            pixelReadRateTable.Add(DxCommandText.Pixel200K, "200KHz");
            pixelReadRateTable.Add(DxCommandText.Pixel150K, "150KHz");
            pixelReadRateTable.Add(DxCommandText.Pixel100K, "100KHz");
            pixelReadRateTable.Add(DxCommandText.Pixel70K, "70KHz");

            //ccd状态
            ccdStateTable.Add(DxCommandText.CcdStateNormal, "正常");
            ccdStateTable.Add(DxCommandText.CcdStateFault, "故障");
            //adc选择
            adcSelectTable.Add(DxCommandText.Adc1, "ADC1");
            adcSelectTable.Add(DxCommandText.Adc2, "ADC2");
            adcSelectTable.Add(DxCommandText.Adc1Adc2, "ADC1+ADC2");
            //开窗大小和数量
            windowInforTable.Add(DxCommandText.WindowSize60x60, "60×60窗口");
            windowInforTable.Add(DxCommandText.WindowSize15x15, "15×15窗口");
            windowInforTable.Add(DxCommandText.WindowNum1, "1个窗");
            windowInforTable.Add(DxCommandText.WindowNum2, "2个窗");
            windowInforTable.Add(DxCommandText.WindowNum3, "3个窗");
            windowInforTable.Add(DxCommandText.WindowNum4, "4个窗");
            //加入二级哈希表
            hashTable.Add(DxTelemetryPrameterKey.WorkMode, workModeTable);
            hashTable.Add(DxTelemetryPrameterKey.ImageTpye, imageTpyeTable);
            hashTable.Add(DxTelemetryPrameterKey.ImageMasterSlave, imageMasterStateTable);
            hashTable.Add(DxTelemetryPrameterKey.FpgaMasterSlave, fpgaMasterStateTable);
            hashTable.Add(DxTelemetryPrameterKey.Pps, ppsTable);
            hashTable.Add(DxTelemetryPrameterKey.ClearMode, clearModeTable);
            hashTable.Add(DxTelemetryPrameterKey.Gain, gainTable);
            hashTable.Add(DxTelemetryPrameterKey.IntegrateRate, integrateRateTable);
            hashTable.Add(DxTelemetryPrameterKey.ReadChannel, readchannelTable);
            hashTable.Add(DxTelemetryPrameterKey.PixelReadRate, pixelReadRateTable);
            hashTable.Add(DxTelemetryPrameterKey.WindowsInfor, windowInforTable);
            hashTable.Add(DxTelemetryPrameterKey.CcdState, ccdStateTable);
            hashTable.Add(DxTelemetryPrameterKey.AdcSelect, adcSelectTable);
        }
    }
}
