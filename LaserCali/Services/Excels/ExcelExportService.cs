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

namespace LaserCali.Services.Excels
{
    public class ExcelExportService
    {
        public delegate void ExceptionOccurEventHandle(object sender, Exception e);
        public delegate void ExportCompleteEventHandel(object sender, string e);

        public event ExceptionOccurEventHandle OnExceptionOccur;
        public event ExportCompleteEventHandel OnExportComplete;

        public async Task Export(List<LaserValueModel> listData,string duongDan)
        {
            await Task.Run(() =>
            {
                try
                {
                    app obj = new app();
                    obj.Application.Workbooks.Add(Type.Missing);
                    obj.Columns.ColumnWidth = 20;
                    //int row = 10;
                    string fontName = "Times New Roman";
                    int fontSizeTenTruong = 13;
                    Excel.Application xlApp = new Excel.Application();
                    object misValue = System.Reflection.Missing.Value;
                    Workbook wb = obj.Workbooks.Add(misValue);

                    Worksheet ws = (Worksheet)wb.Worksheets[1];

                    //Tạo Ô Số Thiết bị số 
                    Range col_range = ws.get_Range("A1", "A1");//Cột A dòng 2 và dòng 3
                    col_range.Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
                    col_range.Merge();
                    col_range.Font.Size = fontSizeTenTruong;
                    col_range.Font.Name = fontName;
                    col_range.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    col_range.Value2 = "STT";
                    col_range.ColumnWidth = 10;

                    //Tạo Ô ID thẻ
                    col_range = ws.get_Range("B1", "B1");//Cột B dòng 2 và dòng 3
                    col_range.Merge();
                    col_range.Font.Size = fontSizeTenTruong;
                    col_range.Font.Name = fontName;
                    col_range.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    col_range.Value2 = "Laser(mm)";
                    col_range.ColumnWidth = 15;
                    col_range.Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);

                    //Tạo Ô Loại Thr
                    col_range = ws.get_Range("C1", "C1");//Cột B dòng 2 và dòng 3
                    col_range.Merge();
                    col_range.Font.Size = fontSizeTenTruong;
                    col_range.Font.Name = fontName;
                    col_range.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    col_range.Value2 = "EUT(mm)";
                    col_range.ColumnWidth = 20;
                    col_range.Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);

                    //Tạo Ô Tháng
                    col_range = ws.get_Range("D1", "D1");//Cột B dòng 2 và dòng 3
                    col_range.Merge();
                    col_range.Font.Size = fontSizeTenTruong;
                    col_range.Font.Name = fontName;
                    col_range.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    col_range.Value2 = "T Material (°C)";
                    col_range.ColumnWidth = 20;
                    col_range.Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);

                    //Tạo Ô Chỉ số đầu tháng 
                    col_range = ws.get_Range("E1", "E1");//Cột C dòng 2 và dòng 3
                    col_range.Merge();
                    col_range.Font.Size = fontSizeTenTruong;
                    col_range.Font.Name = fontName;
                    col_range.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    col_range.ColumnWidth = 20;
                    col_range.Value2 = "Tmt(°C)";
                    col_range.Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);

                    //Tạo Ô Thời gian của chỉ số đầu tháng 
                    col_range = ws.get_Range("F1", "F1");//Cột D->E của  dòng 2 
                    col_range.Merge();
                    col_range.Font.Size = fontSizeTenTruong;
                    col_range.Font.Name = fontName;
                    col_range.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    col_range.ColumnWidth = 20;
                    col_range.Value2 = "RHmt(%)";
                    col_range.Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);

                    //Tạo Ô chỉ số chỉ số của chỉ số đầu tháng
                    col_range = ws.get_Range("G1", "G1");//Ô D3
                    col_range.Font.Size = fontSizeTenTruong;
                    col_range.Font.Name = fontName;
                    col_range.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    col_range.Value2 = "Pressure (hPa)";
                    col_range.ColumnWidth = 20;
                    col_range.Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);

                    BorderAround(ws.get_Range("A1", "G" + (listData.Count + 1).ToString()));


                    for (int i = 0; i < (listData.Count); i++)
                    {
                        obj.Cells[i + 2, 1] = i + 1;
                        obj.Cells[i + 2, 2] = listData[i].Laser.ToString("F6");
                        obj.Cells[i + 2, 3] = listData[i].EUT.ToString("F4");
                        obj.Cells[i + 2, 4] = listData[i].TMaterial.ToString("F2");
                        obj.Cells[i + 2, 5] = listData[i].Tmt.ToString("F2");
                        obj.Cells[i + 2, 6] = listData[i].RH.ToString("F2");
                        obj.Cells[i + 2, 7] = listData[i].Pressure.ToString("F2");//;
                    }

                    string startRange = "A1";
                    string endRange = "A" + (listData.Count + 1).ToString();
                    Excel.Range currentRange = (Excel.Range)ws.get_Range(startRange, endRange);
                    currentRange.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    obj.ActiveWorkbook.SaveCopyAs(duongDan);
                    obj.ActiveWorkbook.Saved = true;
                    if (this.OnExportComplete != null)
                    {
                        this.OnExportComplete(this, duongDan);
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
