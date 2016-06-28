using ShareYield.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShareYield.Controls
{
    class FactoryDataTableParser
    {
        public static DataTable combineDataTables(DataTable dividendTable, DataTable priceTable, double investment)
        {
            DataTable newTable = new DataTable();
            newTable.Columns.Add(new DataColumn("shareCode", typeof(string)));
            newTable.Columns.Add(new DataColumn("companyName", typeof(string)));
            newTable.Columns.Add(new DataColumn("exDividendDate", typeof(string)));
            newTable.Columns.Add(new DataColumn("payDividendDate", typeof(string)));
            newTable.Columns.Add(new DataColumn("amount", typeof(string)));
            newTable.Columns.Add(new DataColumn("franking", typeof(string)));
            newTable.Columns.Add(new DataColumn("price", typeof(string)));
            newTable.Columns.Add(new DataColumn("yield", typeof(string)));

            for (int i = 0; i < dividendTable.Rows.Count; i++)
            {
                DataRow newRow = newTable.NewRow();
                newRow[0] = dividendTable.Rows[i][0];
                newRow[1] = dividendTable.Rows[i][1];
                newRow[2] = dividendTable.Rows[i][2];
                newRow[3] = dividendTable.Rows[i][3];
                newRow[4] = dividendTable.Rows[i][4];
                newRow[5] = dividendTable.Rows[i][5];
                newRow[6] = findPriceForShare(newRow[0], priceTable);
                newRow[7] = calculateYield(newRow[6], newRow[4], investment);
                newTable.Rows.Add(newRow);
            }
            return newTable;
        }

        private static double calculateYield(object price, object amount, double investment)
        {
            double priceDouble = 0;
            double amountDouble = 0;

            try
            {
                priceDouble = Double.Parse((string)price);
                amountDouble = Double.Parse(((string)amount).Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Exception caught when calculating yield -- {0}",ex.ToString()));
            }

            double fees = 30;
            if (investment > 10000)
            {
                fees = 0.003 * investment;
            }

            double profit = (int)((investment - fees) / priceDouble) * amountDouble;
            return Math.Round(profit / investment * 100, 2);
        }

        private static object findPriceForShare(object shareCode, DataTable priceTable)
        {
            DataRow foundRow = priceTable.Rows.Find((string)shareCode);
            return foundRow[1];
        }

        public static void FillShareList(out Share[] shares, DataTable shareList)
        {
            shares = new Share[shareList.Rows.Count];
            for (int row = 0; row < shareList.Rows.Count; row++)
            {
                if (shareList.Rows[row][2] != null)
                {
                    DateTime execDividendDate = ShareExecuteDate((string)shareList.Rows[row][2]);
                    if (execDividendDate.CompareTo(DateTime.Now) == 1)
                    {
                        shares[row] = new Share()
                        {
                            shareCode = (string)shareList.Rows[row][0],
                            companyName = (string)shareList.Rows[row][1],
                            exDividendDate = execDividendDate.ToString("dd MMM"),
                            payDividendDate = (string)shareList.Rows[row][3],
                            amount = (string)shareList.Rows[row][4],
                            franking = (string)shareList.Rows[row][5],
                            price = (string)shareList.Rows[row][6],
                            yield = shareList.Rows[row][7].ToString(),
                        };
                    }
                }
            }
            shares = shares.Where(s => s != null).OrderByDescending(s => s.yield).ToArray();
        }

        private static DateTime ShareExecuteDate(string execDateString)
        {
            string[] spilts = execDateString.Split(' ');
            string date = "";
            if (spilts[1].Trim().StartsWith("Jan"))
            {
                date = spilts[0] + "/01/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Feb"))
            {
                date = spilts[0] + "/02/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Mar"))
            {
                date = spilts[0] + "/03/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Apr"))
            {
                date = spilts[0] + "/04/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("May"))
            {
                date = spilts[0] + "/05/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Jun"))
            {
                date = spilts[0] + "/06/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Jul"))
            {
                date = spilts[0] + "/07/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Aug"))
            {
                date = spilts[0] + "/08/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Sep"))
            {
                date = spilts[0] + "/09/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Oct"))
            {
                date = spilts[0] + "/10/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Nov"))
            {
                date = spilts[0] + "/11/" + spilts[2];
            }
            else if (spilts[1].Trim().StartsWith("Dec"))
            {
                date = spilts[0] + "/12/" + spilts[2];
            }
            else
            {
                date = spilts[0] + "/12/" + spilts[2];
            }
            DateTime dt = Convert.ToDateTime(date);
            return dt;
        }
    }
}
