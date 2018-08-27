using System;
using System.Data;
using System.IO;
using System.Linq;

namespace LF_XML_To_Excel
{
    class 轉出EXCEL
    {
        //static string strSavePath = @"D:\GETEXCLE\";
        static string strSavePath = @"\\192.168.0.156\great giant\部門空間\EDI_File\";
        public string StrStyle { get; set; }
        SystemLog sl = new SystemLog();
        public bool ERP格式(string 處理路徑)
        {
            //var savePath = Directory.GetFiles(strPath);
            string tempFolder = System.IO.Path.GetTempPath(); // Get folder 
            bool bcheck = true;
            
            if (!string.IsNullOrEmpty(處理路徑))
            {
                try
                {
                    DataSet ds = new DataSet();
                    string strCreateDate = File.GetLastWriteTime(處理路徑).ToString("MMdd");
                    //透過DataSet的ReadXml方法來讀取Xmlreader資料
                    ds.ReadXml(處理路徑);
                    //建立DataTable並將DataSet中的第0個Table資料給DataTable
                    DataTable dt = ds.Tables["ColorSize"];
                    //轉換數量類型
                    dt.Columns.Add("數量", typeof(int), "Convert(Quantity,'System.Int32')");
                    DataTable dt1 = ds.Tables["PrePack"];
                    DataTable dt2 = ds.Tables["Shipment"];
                    DataTable dt3 = ds.Tables["Item"];
                    DataTable dt4 = ds.Tables["ePM_VerContent"];
                    string str客戶名稱 = dt4.Rows[0]["BuyerName"].ToString();
                    str客戶名稱= (str客戶名稱.IndexOf("KO") == 0) ? "KHS" : "BLK";
                    string str版次 = dt4.Rows[0]["ePMVerNo"].ToString();
                    if(int.Parse(str版次) > 1)
                        str客戶名稱 += @"\新版次" ;
                    StrStyle = dt3.Rows[0]["ItemNo"].ToString();
                    DataTable dtColor = new DataTable(), dtSize = new DataTable();
                    dtColor = dt.DefaultView.ToTable(true, new string[] { "Color" });
                    dtSize = dt.DefaultView.ToTable(true, new string[] { "Size" });

                    DataTable TempTable = new DataTable();
                    TempTable.Columns.Add("訂單Style");
                    TempTable.Columns.Add("客戶PO");
                    TempTable.Columns.Add("目的地代號");
                    TempTable.Columns.Add("PO別");
                    TempTable.Columns.Add("Style");
                    TempTable.Columns.Add("FOB單價");
                    TempTable.Columns.Add("客戶交期");
                    TempTable.Columns.Add("顏色(英文)");

                    //if (dtSize.Rows.Count > 0)
                    //    for (int iSizeCount = 0; iSizeCount < dtSize.Rows.Count; iSizeCount++)
                    //    {
                    //        TempTable.Columns.Add(dtSize.Rows[iSizeCount][0].ToString().ToUpper().Trim());
                    //    }
                    if (dtSize.Rows.Count > 0)
                        for (int iSizeCount = 0; iSizeCount < dtSize.Rows.Count; iSizeCount++)
                        {
                            TempTable.Columns.Add(dtSize.Rows[iSizeCount][0].ToString().ToUpper().Trim());
                            //string str尺寸轉換 = 尺寸轉換(str客戶名稱, dtSize, iSizeCount);
                            //TempTable.Columns.Add(str尺寸轉換);
                        }

                    for (int i顏色數量 = 0; i顏色數量 < dtColor.Rows.Count; i顏色數量++)
                    {
                        DataRow row;
                        row = TempTable.NewRow();
                        //----不考慮多單價，所有item都會相同
                        row["訂單Style"] = dt3.Rows[0]["ItemNo"];
                        row["FOB單價"] = dt3.Rows[0]["ItemUnitPriceTotal"];
                        //----
                        row["客戶PO"] = dt2.Rows[0]["ShipmentBuyerOrderNo"];
                        row["客戶交期"] = dt2.Rows[0]["ShipmentDeliveryDate"];
                        row["顏色(英文)"] = dtColor.Rows[i顏色數量][0];
                        for (int iSizeCount = 0; iSizeCount < dtSize.Rows.Count; iSizeCount++)
                        {
                            object obtest;
                            obtest = dt.Compute("sum(數量)", "Color = '" + dtColor.Rows[i顏色數量][0].ToString() + "' and Size = '" + dtSize.Rows[iSizeCount][0] + "'");

                            row[dtSize.Rows[iSizeCount][0].ToString()] = obtest.ToString().ToUpper().Trim();
                        }
                        TempTable.Rows.Add(row);
                    }
                    if (TempTable.Columns.Count > 0)
                        for (int i = 0; i < TempTable.Columns.Count; i++)
                        {
                            //if (TempTable.Columns[i].ColumnName.IndexOf(" (") > 0)
                            //{
                            //    //刪除多餘尺寸說明
                            //    TempTable.Columns[i].ColumnName = TempTable.Columns[i].ColumnName.Substring(0, TempTable.Columns[i].ColumnName.IndexOf(" ("));
                            //}
                            //置換size
                            if (i > 7)
                            {
                                string str尺寸轉換 = "";
                                using (var db = new GGFEntities())
                                {

                                    if (TempTable.Columns[i].ColumnName.ToString().ToUpper().Trim().IndexOf(" (") > 0)
                                        str尺寸轉換 = TempTable.Columns[i].ColumnName.ToString().ToUpper().Trim().Substring(0, TempTable.Columns[i].ColumnName.ToString().ToUpper().Trim().IndexOf(" ("));
                                    else
                                        str尺寸轉換 = TempTable.Columns[i].ColumnName.ToString().ToUpper().Trim();
                                    var x = db.LFSize.Where(c => c.EDISize.ToUpper() == str尺寸轉換 && c.Cus_id == str客戶名稱).ToList();
                                    foreach (var Size in x)
                                    {
                                        //有抓到資料覆蓋
                                        str尺寸轉換 = Size.ERPSize;
                                    }
                                    TempTable.Columns[i].ColumnName = str尺寸轉換;
                                }
                            }
                            
                        }
                    DataTableToExcel xx = new DataTableToExcel();

                    xx.ExcelWithNPOI(TempTable, strSavePath + str客戶名稱 + @"\款號" + dt3.Rows[0]["ItemNo"] + "版次" + str版次 +"_PO_"+ dt2.Rows[0]["ShipmentBuyerOrderNo"].ToString() + "_" + strCreateDate + "_ERP格式.xlsx");
                }
                catch (Exception ex)
                {
                    sl.ErrorLog(ex, "ERP格式");
                    //ErrorLog(ex,"ERP格式");
                    bcheck = false;
                }
            }
            return bcheck;
        }

        private static string 尺寸轉換(string str客戶名稱, DataTable dtSize, int iSizeCount)
        {
            string str尺寸轉換 = "";
            using (var db = new GGFEntities())
            {
                if (dtSize.Rows[iSizeCount][0].ToString().ToUpper().Trim().IndexOf(" (") > 0)
                {
                    str尺寸轉換 = dtSize.Rows[iSizeCount][0].ToString().ToUpper().Trim().Substring(0, dtSize.Rows[iSizeCount][0].ToString().ToUpper().Trim().IndexOf(" ("));
                }
                else
                    str尺寸轉換 = dtSize.Rows[iSizeCount][0].ToString().ToUpper().Trim();
                var x = db.LFSize.Where(c => c.EDISize.ToUpper() == str尺寸轉換 && c.Cus_id == str客戶名稱).ToList();
                
                foreach (var Size in x)
                {
                    //有抓到資料覆蓋
                    str尺寸轉換 = Size.ERPSize;
                }
            }

            return str尺寸轉換;
        }

        public bool 匯入格式(string 處理路徑)
        {
            string tempFolder = System.IO.Path.GetTempPath(); // Get folder 
            bool bcheck = true;
            if (!string.IsNullOrEmpty(處理路徑))
            {
                try
                {
                    DataSet ds = new DataSet();
                    string strCreateDate=File.GetCreationTime(處理路徑).ToString("MMdd");
                    //透過DataSet的ReadXml方法來讀取Xmlreader資料
                    ds.ReadXml(處理路徑);
                    //建立DataTable並將DataSet中的第0個Table資料給DataTable
                    DataTable dt = ds.Tables["ColorSize"];
                    //轉換數量類型
                    dt.Columns.Add("數量", typeof(int), "Convert(Quantity,'System.Int32')");
                    DataTable dt1 = ds.Tables["PrePack"];
                    DataTable dt2 = ds.Tables["Shipment"];
                    DataTable dt3 = ds.Tables["Item"];
                    DataTable dt4 = ds.Tables["ePM_VerContent"];
                    string str客戶名稱 = dt4.Rows[0]["BuyerName"].ToString();
                    str客戶名稱 = (str客戶名稱.IndexOf("KO") == 0) ? "KHS" : "BLK";
                    string str版次 = dt4.Rows[0]["ePMVerNo"].ToString();
                    if (int.Parse(str版次) > 1)
                        str客戶名稱 += @"\新版次";
                    DataTable dtColor = new DataTable(), dtSize = new DataTable();
                    dtColor = dt.DefaultView.ToTable(true, new string[] { "Color" });
                    dtSize = dt.DefaultView.ToTable(true, new string[] { "Size" });
                    DataTable TempTable = new DataTable();
                    TempTable.Columns.Add("訂單Style");
                    TempTable.Columns.Add("客戶PO");
                    TempTable.Columns.Add("目的地代號");
                    TempTable.Columns.Add("PO別");
                    TempTable.Columns.Add("Style");
                    TempTable.Columns.Add("顏色(英文)");
                    TempTable.Columns.Add("Size");
                    TempTable.Columns.Add("數量");
                    TempTable.Columns.Add("FOB單價");
                    TempTable.Columns.Add("DC_Date");
                    TempTable.Columns.Add("客戶交期(起)");
                    TempTable.Columns.Add("客戶交期(迄)");
                    TempTable.Columns.Add("客戶交期");



                    for (int i顏色數量 = 0; i顏色數量 < dtColor.Rows.Count; i顏色數量++)
                    {
                        for (int iSizeCount = 0; iSizeCount < dtSize.Rows.Count; iSizeCount++)
                        {
                            DataRow row;
                            row = TempTable.NewRow();
                            //----不考慮多單價，所有item都會相同
                            row["訂單Style"] = dt3.Rows[0]["ItemNo"];
                            row["FOB單價"] = dt3.Rows[0]["ItemUnitPriceTotal"];
                            //----
                            row["客戶PO"] = dt2.Rows[0]["ShipmentBuyerOrderNo"];
                            row["客戶交期"] = dt2.Rows[0]["ShipmentDeliveryDate"];
                            row["顏色(英文)"] = dtColor.Rows[i顏色數量][0];

                            //row["Size"] = (dtSize.Rows[iSizeCount][0].ToString().IndexOf(" (") > 0) ? dtSize.Rows[iSizeCount][0].ToString().Substring(0, dtSize.Rows[iSizeCount][0].ToString().IndexOf(" (")) : dtSize.Rows[iSizeCount][0];
                            string str尺寸轉換 = 尺寸轉換(str客戶名稱, dtSize, iSizeCount);
                            row["Size"] = str尺寸轉換;
                            object obtest;
                            obtest = dt.Compute("sum(數量)", "Color = '" + dtColor.Rows[i顏色數量][0].ToString() + "' and Size = '" + dtSize.Rows[iSizeCount][0] + "'");
                            row["數量"] = obtest.ToString().ToUpper().Trim();
                            if (string.IsNullOrEmpty( obtest.ToString()))
                            {
                                continue;
                            }
                            TempTable.Rows.Add(row);
                        }
                    }
                    DataTableToExcel xx = new DataTableToExcel();
                    xx.ExcelWithNPOI(TempTable, strSavePath + str客戶名稱 + @"\款號" + dt3.Rows[0]["ItemNo"]+"版次"+ str版次+"_PO_" + dt2.Rows[0]["ShipmentBuyerOrderNo"].ToString() + "_"+ strCreateDate + "_匯入格式.xlsx");
                }
                catch (Exception ex)
                {
                    
                    sl.ErrorLog(ex, "匯入格式");
                    //ErrorLog(ex, "匯入格式");
                    bcheck = false;
                }
            }
            return bcheck;
        }

        private static void ErrorLog(Exception ex,string 程式名稱)
        {
            using (var conn = new GGFEntities())
            {
                using (var transaction = conn.Database.BeginTransaction())
                { 
                    ERROR_LOG error = new ERROR_LOG
                    {
                        ERROR_LOG1 = ex.ToString(),
                        ERROR_PROGRAM = "LF XML to Excel 程式，程式名稱："+ 程式名稱,
                        CREATED_BY = "Program",
                        CREATEDATE = DateTime.Now
                    };
                    conn.ERROR_LOG.Add(error);
                    conn.SaveChanges();
                    transaction.Commit();
                }
            }
        }
    }
}
