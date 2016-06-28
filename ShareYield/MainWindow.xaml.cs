using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShareYield.Objects;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.ComponentModel;
using ShareYield.Controls;
using ShareYield.Models;

namespace ShareYield
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double investment = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (investmentText.Text.Length > 0)
            {
                investment = Double.Parse(investmentText.Text);
            }

            //Process dividend amount list
            string shareSheet = @"C:\Users\FelixXingyao\Desktop\Share\dividend.xlsx";
            //string shareSheet = @"E:\\Share\\dividend.xlsx";
            DataTable dividendTable = new SpreadSheet(shareSheet, "").CurrentSpreadSheet;
            DataColumn[] dividendKeys = new DataColumn[1];
            dividendKeys[0] = dividendTable.Columns[0];
            dividendTable.PrimaryKey = dividendKeys;

            //Process share price list
            string priceSheet = @"C:\Users\FelixXingyao\Desktop\Share\sharePrice.xlsx";
            //string priceSheet = @"E:\\Share\\sharePrice.xlsx";
            DataTable priceTable = new SpreadSheet(priceSheet, "").CurrentSpreadSheet;
            DataColumn[] priceKeys = new DataColumn[1];
            priceKeys[0] = priceTable.Columns[0];
            priceTable.PrimaryKey = priceKeys;

            //Combine two lists
            DataTable combinedTable = FactoryDataTableParser.combineDataTables(dividendTable, priceTable, investment);

            Share[] shares;
            FactoryDataTableParser.FillShareList(out shares, combinedTable);

            dataGrid.ItemsSource = shares;
        }

        

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var desc = e.PropertyDescriptor as PropertyDescriptor;
            var att = desc.Attributes[typeof(ColumnNameAttribute)] as ColumnNameAttribute;
            if (att != null)
            {
                e.Column.Header = att.Name;
            }
        }
    }

    public class ColumnNameAttribute : System.Attribute
    {
        public ColumnNameAttribute(string Name) { this.Name = Name; }
        public string Name { get; set; }
    }
}
