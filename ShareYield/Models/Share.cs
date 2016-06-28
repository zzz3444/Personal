using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYield.Objects
{
    class Share
    {
        [ColumnName("Code")]
        public string shareCode { get; set; }
        [ColumnName("Company Name")]
        public string companyName { get; set; }
        [ColumnName("Ex Dividend Date")]
        public string exDividendDate { get; set; }
        [ColumnName("Pay Dividend Date")]
        public string payDividendDate { get; set; }
        [ColumnName("Amount")]
        public string amount { get; set; }
        [ColumnName("Franking")]
        public string franking { get; set; }
        [ColumnName("Price")]
        public string price { get; set; }
        [ColumnName("Yield")]
        public string yield { get; set; }

    }
}
