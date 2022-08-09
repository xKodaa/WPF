using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FormApp
{
    internal class Superhero
    {
        private SqlConnection connectToDatabase = new SqlConnection();
        private string connectionString = Connect.ConnString;

        public string name { get; set; }
        public string surname { get; set; }
        public int age { get; set; }

        List<Superhero> listOfSuperheroes = new List<Superhero>();

        public Superhero ([Optional] string name, [Optional] string surname, [Optional] int age)
        {
            this.name = name;
            this.surname = surname;
            this.age = age;
        }

        public void fillListWithSuperheores()
        {

            connectToDatabase.ConnectionString = connectionString;
            connectToDatabase.Open();
            SqlCommand select = new SqlCommand("SELECT * FROM dbo.Praxe_test", connectToDatabase);
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                string name = reader[1].ToString();
                string surname = reader[2].ToString();
                int age = (int)reader[3];
                listOfSuperheroes.Add(new Superhero(name, surname, age));
            }
            reader.Close();
            connectToDatabase.Close();
            printSuperheroes();
        }

        public void printSuperheroes()
        {
            foreach(Superhero superhero in listOfSuperheroes) //Vypíše obejkty typu superhero do konzole ve výstupu
            {
                Trace.WriteLine("Superhero: [" + superhero.name + ", " + superhero.surname + ", " + superhero.age + "]");
            }
        }

    }
}
