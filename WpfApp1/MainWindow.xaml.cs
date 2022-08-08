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
        private DataTable dataTable = new DataTable("Praxe_test"); //vytváření prostoru pro data
        private string connectionString = Connect.ConnString; //do proměnné je uložen string s parametry pro připojení do SQL databáze z třídy Connect.cs
        private int age;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized; //fullscreen
            //naplnění combo-boxu Stringy
            optionComboBox.Items.Add("Search");
            optionComboBox.Items.Add("Insert");
            optionComboBox.Items.Add("Delete");
            optionComboBox.SelectedIndex = 0; //nastavení výchozí hodnoty na nultej index (search)

            //-----------------------------------------------------------------ZKOUŠKA PŘIPOJENÍ-------------------------------------------------------------//
            try
            {
                connectToDatabase.ConnectionString = connectionString; //přiřazení connection stringu
                connectToDatabase.Open(); //otevření databáze pro komunikaci
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); //vypíše error log
                connectToDatabase.Close(); //zavře databázi
            }
            //----------------------------------------------------------------/ZKOUŠKA PŘIPOJENÍ-------------------------------------------------------------//
            connectToDatabase.Close();
            refresh(); //chci prvně naplnit formulář daty, ale později jsem si na to udělal metodu, takže si ji tady rovnou volám
        }


        //-----------------------------------------------------------------REAL TIME SEARCHING---------------------------------------------------------------//
        private void searchText(object sender, TextChangedEventArgs e)
        {
           if (optionComboBox.Text.Equals("Search")) //event se spouští použe pokud je v comboboxu hodnota "Search"
           {
                if (textBoxName.IsSelectionActive) //hledání podle text-boxu Name
                {
                    SqlCommand searchQuery = new SqlCommand("SELECT * FROM dbo.Praxe_test WHERE Name LIKE '" + textBoxName.Text + "%'", connectToDatabase);
                    fillForm(searchQuery);
                }
                if (textBoxSurname.IsSelectionActive) //hledání podle text-boxu Surname
                {
                    SqlCommand searchQuery = new SqlCommand("SELECT * FROM dbo.Praxe_test WHERE Surname LIKE '" + textBoxSurname.Text + "%'", connectToDatabase);
                    fillForm(searchQuery);
                }
                if (textBoxAge.IsSelectionActive) //hledání podle text-boxu Age
                {
                    SqlCommand searchQuery = new SqlCommand("SELECT * FROM dbo.Praxe_test WHERE Age LIKE '" + textBoxAge.Text + "%'", connectToDatabase);
                    fillForm(searchQuery);
                }
           }

        }
        //----------------------------------------------------------------/REAL TIME SEARCHING---------------------------------------------------------------//


        //---------------------------------------------------------------METODY PRO PRÁCI S DATY-------------------------------------------------------------//
        private void insertData() //metoda která vkládá data do databáze
        {
            connectToDatabase.Open();
            try
            {
                age = Int32.Parse(textBoxAge.Text);
            } catch (FormatException ex)
            {
                MessageBox.Show("Prosím zadávejte validní hodnoty! [-Věk musí být číslo]", "Input format Error", MessageBoxButton.OK, MessageBoxImage.Error);
                connectToDatabase.Close(); //nechci aby se mi do databáze cokoliv vložilo pokud je špatný input
                return; //aby se neprovedl zbytek kódu v metodě
            }

            if (!textBoxName.Text.Equals("") && !textBoxSurname.Text.Equals("")) //pokud Name a Surname není prázdný String
            {
                SqlCommand insertQuery = new SqlCommand("INSERT INTO dbo.Praxe_test VALUES('" + textBoxName.Text + "','" + textBoxSurname.Text + "'," +
                   age + ")", connectToDatabase);
                int sucessful = insertQuery.ExecuteNonQuery();

                if (sucessful < 0 || sucessful == 0) //ověřovací if
                {
                    MessageBox.Show("Záznam se nepodařilo vložit", "Task failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Záznam se úspěšně vložil", "Task successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vyplňte prosím všechna data!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            connectToDatabase.Close();
        }

        private void deleteData() //metoda která maže data z databáze
        {
            connectToDatabase.Open();
            try
            {
                age = Int32.Parse(textBoxAge.Text);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Prosím zadávejte validní hodnoty! [-Věk musí být číslo]", "Input format Error", MessageBoxButton.OK, MessageBoxImage.Error);
                connectToDatabase.Close(); //nechci aby se mi do databáze cokoliv vložilo pokud je špatný input
                return; //aby se neprovedl zbytek kódu v metodě
            }

            if (!textBoxName.Text.Equals("") && !textBoxSurname.Text.Equals("")) //pokud Name a Surname není prázdný String
            {
                SqlCommand deleteQuery = new SqlCommand("DELETE FROM dbo.Praxe_test WHERE Name LIKE '" + textBoxName.Text + "' AND Surname LIKE '" + textBoxSurname.Text
                  + "' AND Age = " + age, connectToDatabase);
                int sucessful = deleteQuery.ExecuteNonQuery();

                if (sucessful < 0 || sucessful == 0) //ověřovací if
                {
                    MessageBox.Show("Záznam se nepodařilo smazat", "Task failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Záznam se úspěšně smazal", "Task successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vyplňte prosím všechna data!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            connectToDatabase.Close();
        }
        //--------------------------------------------------------------/METODY PRO PRÁCI S DATY-------------------------------------------------------------//

        //-------------------------------------------------------------------POMOCNÉ METODY------------------------------------------------------------------//
        private void fillForm(SqlCommand query) //metoda pro naplnění formuláře daty
        {
            connectToDatabase.Open();
            dataTable = new DataTable("Praxe_test"); //vytvářím prostor pro data
            dataAdapter = new SqlDataAdapter(query); //do data adapteru posílám dotaz na výběr určitých dat
            dataAdapter.Fill(dataTable); //data adapter zpracovanými daty naplní data table "Praxe_test"
            dataGrid.DataContext = dataTable; //data table se promítne do formuláře
            connectToDatabase.Close();

        }

        private void refresh() //metoda která refreshne tabulku a zobrazí všechna data
        {
            SqlCommand refreshTable = new SqlCommand("SELECT * FROM dbo.Praxe_test", connectToDatabase);
            fillForm(refreshTable);
        }

        
        private void buttonClick(object sender, RoutedEventArgs e) //metoda která čeká na event po kliknutí na button
        {
            //button volá funkce podle itemu, který je vybrán v combo-boxu
            if (optionComboBox.SelectedIndex == 0) //search
            {   
                clearTextBoxes();
                refresh();
            }
            if (optionComboBox.SelectedIndex == 1) //insert
            {
                insertData();
                clearTextBoxes();
                refresh();
            }
            if (optionComboBox.SelectedIndex == 2) //delete
            {
                deleteData();
                clearTextBoxes();
                refresh();
            }
        }

        private void changeTextOfButton(object sender, SelectionChangedEventArgs e) //metoda která mění text tlačítka podle vybraného itemu v combo-boxu
        {
            if (optionComboBox.SelectedIndex == 0) //search
            {
                buttonOption.Content = "Clear";
            }
            if (optionComboBox.SelectedIndex == 1) //insert
            {
                buttonOption.Content = "Insert";
            }
            if (optionComboBox.SelectedIndex == 2) //delete
            {
                buttonOption.Content = "Delete";
            }
        }

        private void clearTextBoxes() //metoda která vymaže text z text-boxů
        {
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            textBoxAge.Text = "";
        }
        //------------------------------------------------------------------/POMOCNÉ METODY------------------------------------------------------------------//
    }

}
