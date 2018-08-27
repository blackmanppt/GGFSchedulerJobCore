using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;

namespace LF_XML_To_Excel
{
    class DataTableToExcel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt">資料Table</param>
        /// <param name="儲存路徑">儲存路徑</param>
        public void ExcelWithNPOI(DataTable dt, string 儲存路徑)
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
            if (File.Exists(儲存路徑))
            {
                int icheck = 1;
                while (File.Exists(儲存路徑.Substring(0, 儲存路徑.Length - 5) + icheck.ToString("000") + ".xlsx"))
                {
                    icheck++;
                }
                儲存路徑 = 儲存路徑.Substring(0, 儲存路徑.Length - 5) + icheck.ToString("000") + ".xlsx";
            }

            FileStream sw = File.Create(儲存路徑);
            try
            {
                workbook.Write(sw);
            }
            catch (Exception ex)
            {
                SystemLog sl = new SystemLog();
                sl.ErrorLog(ex, "DataTableToExcel：excel生成失敗");
            }
            finally
            {
                sw.Close();
            }
        }
    }
}

