using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYield.Models
{
    class SpreadSheet
    {
        private DataTable currentSpreadSheet;
        public SpreadSheet(string filePath, string SheetName)
        {
            string strConn = string.Empty;
            if (string.IsNullOrEmpty(SheetName)) { SheetName = "Sheet1"; }
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists) { throw new Exception("The file does not exist."); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
            }
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select * from [{0}$]", SheetName), cnnxls);
            var ds = new DataSet();


            oda.Fill(ds, "spreadsheet");
            currentSpreadSheet = ds.Tables["spreadsheet"];
        }

        public DataTable CurrentSpreadSheet
        {
            get
            {
                return currentSpreadSheet;
            }

            set
            {
                currentSpreadSheet = value;
            }
        }
    }
}
