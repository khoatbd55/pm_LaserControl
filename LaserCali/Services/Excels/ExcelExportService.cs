using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app = Microsoft.Office.Interop.Excel.Application;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using LaserCali.Models.Views;
using LaserCali.Models.Export;
using LaserCali.Services.Config;

namespace LaserCali.Services.Excels
{
    public class ExcelExportService
    {
        public delegate void ExceptionOccurEventHandle(object sender, Exception e);
        public delegate void ExportCompleteEventHandel(object sender, string e);

        public event ExceptionOccurEventHandle OnExceptionOccur;
        public event ExportCompleteEventHandel OnExportComplete;

        public async Task Export(List<LaserValueModel> listData,DutInformation_Model dut, string destinationFilePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    string sourceFilePath = AppDomain.CurrentDomain.BaseDirectory+ @"\Template.xlsx"; // Đường dẫn đến tệp tin nguồn
                    // Khởi tạo ứng dụng Excel
                    Excel.Application excelApp = new Excel.Application();
                    excelApp.Visible = false; // Tắt hiển thị ứng dụng Excel

                    // Mở tệp tin nguồn
                    Excel.Workbook sourceWorkbook = excelApp.Workbooks.Open(sourceFilePath);

                    // Tạo bản sao của tệp tin nguồn
                    sourceWorkbook.SaveCopyAs(destinationFilePath);
                    sourceWorkbook.Close();

                    // Mở tệp tin bản sao
                    Excel.Workbook destinationWorkbook = excelApp.Workbooks.Open(destinationFilePath);
                    Excel.Worksheet worksheet1 = destinationWorkbook.Worksheets[1] as Excel.Worksheet;

                    {
                        // thông tin DUT
                        var cell = worksheet1.Range["I5"];
                        cell.Value = dut.Name;
                        cell = worksheet1.Range["E6"];
                        cell.Value = dut.Model;
                        cell = worksheet1.Range["R6"];
                        cell.Value = dut.Serial;
                        cell = worksheet1.Range["I7"];
                        cell.Value = dut.Manufacturer;
                        cell = worksheet1.Range["R8"];
                        cell.Value = dut.Range;
                        cell = worksheet1.Range["R9"];
                        cell.Value = dut.Resolution;
                        cell = worksheet1.Range["M10"];
                        cell.Value = dut.Grade;
                    }
                    

                    if(listData.Count>1)
                    {
                        for(int i=0;i<listData.Count-1;i++)
                        {
                            Excel.Range sourceRange = worksheet1.Range["AC5:BD5"];
                            Excel.Range targetRange = worksheet1.Range[$"AC{6+i}:BD{6+i}"];
                            sourceRange.Copy(targetRange);
                        }    
                    }

                    var cfg = LaserConfigService.ReadConfig();

                    for (int i=0;i<listData.Count;i++)
                    {
                        int rowIndex = 5+i;
                        // Chèn hình ảnh vào tệp tin bản sao
                        var cell = worksheet1.Range[$"AC{rowIndex}"]; // bơm gió 0-9 bar
                        cell.Value = i+1;
                        cell = worksheet1.Range[$"AE{rowIndex}"]; // áp suất 0-9 bar
                        cell.Value = listData[i].Laser.ToString($"F{cfg.LaserValueResolution}");

                        cell = worksheet1.Range[$"AJ{rowIndex}"];
                        cell.Value = listData[i].DUT.ToString($"F1");

                        cell = worksheet1.Range[$"AO{rowIndex}"];
                        cell.Value = listData[i].TMaterial.ToString($"F3");

                        cell = worksheet1.Range[$"AS{rowIndex}"];
                        cell.Value = listData[i].Tmt.ToString($"F1");

                        cell = worksheet1.Range[$"AW{rowIndex}"];
                        cell.Value = listData[i].RH.ToString($"F1");

                        cell = worksheet1.Range[$"BA{rowIndex}"];
                        cell.Value = listData[i].Pressure.ToString($"F1");
                    }

                    Excel.Worksheet worksheet3 = destinationWorkbook.Worksheets[3] as Excel.Worksheet;
                    if (listData.Count > 1)
                    {
                        for (int i = 0; i < listData.Count - 1; i++)
                        {
                            Excel.Range sourceRange = worksheet3.Range["A4:AE4"];
                            Excel.Range targetRange = worksheet3.Range[$"A{5 + i}:AE{5 + i}"];
                            sourceRange.Copy(targetRange);
                        }
                    }

                    // Lưu và đóng tệp tin bản sao
                    destinationWorkbook.Save();
                    destinationWorkbook.Close();

                    // Đóng ứng dụng Excel
                    excelApp.Quit();
                    if (this.OnExportComplete != null)
                    {
                        this.OnExportComplete(this, destinationFilePath);
                    }
                }
                catch (Exception ex)
                {
                    if (this.OnExceptionOccur != null)
                    {
                        this.OnExceptionOccur(this, ex);
                    }
                }

            });

        }
        private void BorderAround(Range range)
        {
            Borders borders = range.Borders;
            borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            borders.Color = System.Drawing.Color.Black;
            borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlInsideHorizontal].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlDiagonalUp].LineStyle = XlLineStyle.xlLineStyleNone;
            borders[XlBordersIndex.xlDiagonalDown].LineStyle = XlLineStyle.xlLineStyleNone;
        }

    }
}
