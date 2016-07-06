using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using ShareYield.Classes;
using FetchAndSaveStockData;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace FetchAndSaveStockData
{
    class Program
    {
        private static List<SharePrice> shares = new List<SharePrice>();
        private static List<Dividend> dividends = new List<Dividend>();

        static void Main(string[] args)
        {
            RefreshHtmlDoc();
            ParseSharePrice();
            ParseDividend();
            SaveToDatabase();
            Console.WriteLine("Completed.");
            Console.ReadLine();
        }

        private static void ParseDividend()
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load("Dividend.txt");
            HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectSingleNode("//section[@id='dividends']/table/tbody").ChildNodes;
            foreach (HtmlNode node in collection)
            {
                string[] line = Array.ConvertAll(node.InnerText.Split(new char[] { '\r', '\n' }), p => p.Trim());
                if (line.Length == 10)
                {
                    dividends.Add(new Dividend { code = line[1], execDate = line[2], period = line[3], amount = ConvertToDollar(line[4]),
                        supp = ConvertToDollar(line[5]), imputation = ConvertToDollar(line[6]), payable = line[7],currency = line[8] });
                }
            }
        }

        private static string ConvertToDollar(string amount)
        {
            return (Double.Parse(amount.Replace("c","")) / 100.0).ToString();
        }

        private static void ParseSharePrice()
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load("SharePrice.txt");
            HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectSingleNode("//section[@id='instruments']/table/tbody").ChildNodes;
            foreach (HtmlNode node in collection)
            {
                string[] line = Array.ConvertAll(node.InnerText.Split(new char[] { '\r', '\n' }), p => p.Trim());
                if (line.Length == 10)
                { 
                    shares.Add(new SharePrice { code = line[2], companyName = line[4], price = line[5] });
                }
            }
        }

        private static void SaveToDatabase()
        {
            string connectionString = @"Data Source=HAZELCORGI\SQLEXPRESS;Initial Catalog=ShareYield;Integrated Security=SSPI";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    SqlCommand cmd = new SqlCommand(sql);
                    cmd.Connection = connection;
                    foreach (SharePrice sp in shares)
                    {
                        if (String.IsNullOrEmpty(sp.code) || String.IsNullOrWhiteSpace(sp.code) || String.IsNullOrEmpty(sp.price) || String.IsNullOrWhiteSpace(sp.price))
                        {
                            continue;
                        }
                        sql = String.Format("UPDATE SharePrice SET Price = {0}, CompanyName = '{1}' where Code = '{2}'", sp.price.Replace("$",""), sp.companyName, sp.code);
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                    sql = "Delete From Dividend";
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    foreach (Dividend div in dividends)
                    {

                        sql = "INSERT INTO [dbo].[Dividend]";
                        sql += "([Code],[ExecDate],[PayDate],[Amount],[DividendPeriod],[Supp],[Imputation],[Currency])";
                        sql += "VALUES('{0}','{1}','{2}',{3},'{4}',{5},{6},'{7}')";
                        sql = String.Format(sql, div.code,div.execDate, div.payable, div.amount, div.period, div.supp, div.imputation, div.currency);
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        if (String.IsNullOrEmpty(div.code) || String.IsNullOrWhiteSpace(div.code))
                        {
                            continue;
                        }
                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Exception caught -- " + ex.ToString());
            } 
        }

        private static void RefreshHtmlDoc()
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Credentials = CredentialCache.DefaultCredentials; 
                Byte[] pageData = webClient.DownloadData("https://www.nzx.com/markets/NZSX/securities");
                string pageHtml = Encoding.UTF8.GetString(pageData);
                using (StreamWriter sw = new StreamWriter("SharePrice.txt"))
                {
                    sw.Write(pageHtml);
                }
                pageData = webClient.DownloadData("https://www.nzx.com/markets/NZSX");
                pageHtml = Encoding.UTF8.GetString(pageData);
                using (StreamWriter sw = new StreamWriter("Dividend.txt"))
                {
                    sw.Write(pageHtml);
                }
            }
            catch (WebException webEx)
            {
                Console.Write(webEx.ToString());
            }
        }

        private class SharePrice
        {
            public string code { get; set; }
            public string companyName { get; set; }
            public string price { get; set; }

            public override string ToString()
            {
                return "Code: " + code + ". Company Name: " + companyName + ". Price: " + price + ".";
            }
        }

        private class Dividend
        {
            public string code { get; set; }
            public string execDate { get; set; }
            public string period { get; set; }
            public string amount { get; set; }
            public string supp { get; set; }
            public string imputation { get; set; }
            public string payable { get; set; }
            public string currency { get; set; }

            public override string ToString()
            {
                return "Code: " + code + ". Execute Date: " + execDate + ". Period: " + period + ". Amount: " + amount + ". Supp: " + supp + ". Imputation: " + imputation 
                    + ". Payable: " + payable + ". Currency: " + currency + ".";
            }
        }
    }
}
