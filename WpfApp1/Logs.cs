using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FormApp
{
    internal class Logs
    {
        private SqlConnection connectToDatabase = new SqlConnection();
        private SqlConnection openDatabase = new SqlConnection();
        private string connectionString = Connect.ConnString;

        private String msg;
        private int typLogu;
        string format = "yyyy-MM-dd HH:mm:ss";

        public Logs()
        {
            msg = "Default Log message";
        }

        public void log(int typLogu, [Optional] String Name, [Optional] String Surname, [Optional] int Age) 
        {
            openDatabase.ConnectionString = connectionString;
            connectToDatabase.ConnectionString = connectionString;
            connectToDatabase.Open();
            checkLogAge();
            switch (typLogu) 
            {
                case 0:
                    msg = "*-------PROGRAM ZAPNUT-------*";
                    SqlCommand startLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    startLog.ExecuteNonQuery();
                    break;
                case 1:
                    msg = "*-------PROGRAM VYPNUT-------*";
                    SqlCommand endLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    endLog.ExecuteNonQuery();
                    break;
                case 2:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age.ToString() + "] byl úspěšně vložen";
                    SqlCommand insertLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    insertLog.ExecuteNonQuery();
                    break;
                case 3:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age.ToString() + "] se nepovedlo vložit";
                    SqlCommand insertErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    insertErrLog.ExecuteNonQuery();
                    break;
                case 4:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age.ToString() + "] se úspěšně smazal";
                    SqlCommand deleteLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    deleteLog.ExecuteNonQuery();
                    break;
                case 5:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age.ToString() + "] se nepovedlo smazat";
                    SqlCommand deleteErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    deleteErrLog.ExecuteNonQuery();
                    break;
                default:
                    MessageBox.Show("Unsupported Log type - " + typLogu + ", (VALID LOGS - 0,1,2,3,4,5)", "Invalid Log type", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
            connectToDatabase.Close();
        }

        public void checkLogAge() 
        {
            openDatabase.Open();
            SqlCommand check = new SqlCommand("SELECT TOP(100) Date, Text FROM dbo.Praxe_test_logs ORDER BY Date", connectToDatabase);
            SqlDataReader reader = check.ExecuteReader();
            while (reader.Read()) //přečte každej řádek ze selectu check
            {
                if (DateTime.Now.Year - DateTime.Parse(reader[0].ToString()).Year >= 3) //kontroluji zda se v databázi nachází záznam starší než 3 roky
                {
                    DateTime dt = DateTime.Parse(reader[0].ToString());
                    SqlCommand deleteOld = new SqlCommand("DELETE FROM dbo.Praxe_test_logs WHERE Date = '" + dt.ToString(format) + "'", openDatabase);
                    deleteOld.ExecuteNonQuery(); //pokud ano smažu ho
                }
            }
            reader.Close();
            openDatabase.Close();
        }
    }
}
