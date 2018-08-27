using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;

namespace NPOITest
{
    class DatatableToExcel
    {
        public void ExcelWithNPOI(DataTable dt, string extension)
        {
            IWorkbook workbook;
            workbook = new XSSFWorkbook();
            ISheet sheet1;
            if (dt.TableName != string.Empty)
            {
                sheet1 = workbook.CreateSheet(dt.TableName);
            }
            else
            {
                sheet1 = workbook.CreateSheet("Sheet1");
            }

            //make a header row
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row1.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }

            //loops through data
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                }
            }
            FileStream sw = File.Create("test.xlsx");
            workbook.Write(sw);
            sw.Close();
            //using (var exportData = new MemoryStream())
            //{
            //    HttpContext.Current.Response.Clear();
            //    workbook.Write(exportData);
            //    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", DateTime.Now.ToString("yyyymmdd") + ".xlsx"));
            //    HttpContext.Current.Response.BinaryWrite(exporrtData.ToArray());

            //    HttpContext.Current.Response.End();
            //}
        }
    }
}
