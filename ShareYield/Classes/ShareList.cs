using ShareYield.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYield.Classes
{
    public class ShareList
    {
        private List<Share> shares = new List<Share>();
        public ShareList()
        {
            //Open SQL connection to fetch information and build shares list
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=HAZELCORGI\SQLEXPRESS;Initial Catalog=ShareYield;Integrated Security=SSPI";
            SqlCommand cmd = new SqlCommand("select * from ShareYield", conn);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Share newShare = new Share();
                    newShare.shareCode = reader["Code"].ToString();
                    newShare.companyName = reader["CompanyName"].ToString();
                    newShare.exDividendDate = reader["ExecDate"].ToString();
                    newShare.payDividendDate = reader["PaymentDate"].ToString();
                    newShare.amount = reader["Amount"].ToString();
                    newShare.price = reader["Price"].ToString();
                    newShare.yield = reader["Yield"].ToString();
                    shares.Add(newShare);
                }

                conn.Close();
            }
        }

        public List<Share> getShares()
        {
            return shares;
        }
    }
}
