using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace FormApp
{
    public partial class MainWindow : Window
    {
        private SqlDataAdapter dataAdapter = new SqlDataAdapter(); //komponenta, která přebírá data z databáze a může s nimi pracovat
        private SqlConnection connectToDatabase = new SqlConnection(); //instance pro připojování do databáze
        private string connectionString = Connect.ConnString; //do proměnné je uložen string s parametry pro připojení do SQL databáze z třídy Connect.cs

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized; //fullscreen
            try
            {
                connectToDatabase.ConnectionString = connectionString; //přiřazení connection stringu
                SqlCommand fillDataQuery = new SqlCommand("SELECT * FROM dbo.HD_Stavy", connectToDatabase); //vyberu všechny data kterými poté plním formulář
                fillForm(fillDataQuery);
                fillComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); //vypíše error log
                connectToDatabase.Close(); //zavře databázi
            }
        }
        //----------------------------------------------------------------ŘAZENÍ ZOBRAZENÝCH DAT----------------------------------------------------------------//
        private void fillComboBoxes()
        {

            //do sorting combo boxu přidám jen 2 hodnoty:
            comboBoxSorting.Items.Add("Vzestupně");
            comboBoxSorting.Items.Add("Sestupně");

            connectToDatabase.Open(); //otevírám databázi protože data pro combo box jsou názvy sloupců databáze
            //dotaz co zobrazuje pouze názvy sloupců z tabulky
            SqlCommand getNameOfColumnsQuery = new SqlCommand("SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('dbo.HD_Stavy')", connectToDatabase);
            SqlDataReader dataReader = getNameOfColumnsQuery.ExecuteReader(); //komponenta čte data získaná z dotazu

            while (dataReader.Read()) //čte po řádcích
            {
                //listNazvu.Add(dataReader["name"].ToString());
                comboBox.Items.Add(dataReader["name"].ToString()); //naplnění combo boxu názvy sloupců jako stringy

            }
            connectToDatabase.Close(); //po naplnění combo boxu s názvy sloupců už nepotřebuji otevřenou databázi
        }
        private void checkSort() //metoda pro zjištění jak se budou řadit data
        {
            connectToDatabase.Open();
            string nazevSloupce = comboBox.Text; //do proměnné ukládám aktuální item jako text z combo boxu
            if (comboBoxSorting.Text.Equals("Vzestupně"))
            {
                SqlCommand sortTableQuery = new SqlCommand("SELECT * FROM dbo.HD_Stavy ORDER BY " + nazevSloupce + " ASC", connectToDatabase);
                fillForm(sortTableQuery);
            }
            else
            {
                SqlCommand sortTableQuery = new SqlCommand("SELECT * FROM dbo.HD_Stavy ORDER BY " + nazevSloupce + " DESC", connectToDatabase);
                fillForm(sortTableQuery);
            }
            connectToDatabase.Close();

        }

        private void fillForm(SqlCommand query) //metoda pro naplnění formuláře daty
        {
            DataTable dataTable = new DataTable("Stavy"); //vytvářím prostor pro data
            dataAdapter = new SqlDataAdapter(query); //do data adapteru posílám dotaz na výběr určitých dat
            dataAdapter.Fill(dataTable); //data adapter zpracovanými daty naplní data table "Stavy"
            dataGrid.DataContext = dataTable; //data table se promítne do formuláře
        }

        private void Button_Click(object sender, RoutedEventArgs e) { checkSort(); }

        //---------------------------------------------------------------/ŘAZENÍ ZOBRAZENÝCH DAT-------------------------------------------------------------//


        //-----------------------------------------------------------------REAL TIME SEARCHING---------------------------------------------------------------//
        private void searchText(object sender, TextChangedEventArgs e)
        {
            connectToDatabase.Open();
            SqlCommand searchQuery = new SqlCommand("SELECT * FROM dbo.HD_Stavy WHERE Nazev LIKE '" + textBoxNazev.Text + "%'", connectToDatabase);
            dataAdapter = new SqlDataAdapter(searchQuery);
            DataTable dataTable = new DataTable("Stavy");
            dataAdapter.Fill(dataTable);
            dataGrid.DataContext = dataTable;
            connectToDatabase.Close();
        }
        //----------------------------------------------------------------/REAL TIME SEARCHING---------------------------------------------------------------//


    }

}
