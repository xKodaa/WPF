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

        public void log(Log logType, [Optional] String Name, [Optional] String Surname, [Optional] String Age) 
        {
            openDatabase.ConnectionString = connectionString;
            connectToDatabase.ConnectionString = connectionString;
            connectToDatabase.Open();
            checkLogAge();
            switch (logType) 
            {
                case Log.START:
                    msg = "*-------PROGRAM ZAPNUT-------*";
                    SqlCommand startLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    startLog.ExecuteNonQuery();
                    break;
                case Log.END:
                    msg = "*-------PROGRAM VYPNUT-------*";
                    SqlCommand endLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    endLog.ExecuteNonQuery();
                    break;
                case Log.INSERT_SUCCESS:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] byl úspěšně vložen";
                    SqlCommand insertSuccessLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    insertSuccessLog.ExecuteNonQuery();
                    break;
                case Log.INSERT_FAILURE:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se nepodařilo vložit";
                    SqlCommand insertErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    insertErrLog.ExecuteNonQuery();
                    break;
                case Log.INSERT_NAME_FAILURE:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se nepovedlo vložit, pole *Name* nevyplněno";
                    SqlCommand insertNameFormatErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    insertNameFormatErrLog.ExecuteNonQuery();
                    break;
                case Log.INSERT_SURNAME_FAILURE:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se nepovedlo vložit, pole *Surname* nevyplněno";
                    SqlCommand insertSurnameFormatErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    insertSurnameFormatErrLog.ExecuteNonQuery();
                    break;
                case Log.INSERT_AGE_FAILURE:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se nepovedlo vložit, špatný formát proměnné *Age*";
                    SqlCommand insertAgeFormatErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    insertAgeFormatErrLog.ExecuteNonQuery();
                    break;
                case Log.DELETE_SUCCESS:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se úspěšně smazal";
                    SqlCommand deleteSuccessLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    deleteSuccessLog.ExecuteNonQuery();
                    break;
                case Log.DELETE_FAILURE:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se nepodařilo smazat";
                    SqlCommand deleteErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    deleteErrLog.ExecuteNonQuery();
                    break;
                case Log.DELETE_AGE_FAILURE:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se nepovedlo smazat, špatný formát proměnné *Age*";
                    SqlCommand deleteAgeFormatErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    deleteAgeFormatErrLog.ExecuteNonQuery();
                    break;
                case Log.DELETE_NAME_FAILURE:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se nepovedlo smazat, pole *Name* nevyplněno";
                    SqlCommand deleteNameFormatErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    deleteNameFormatErrLog.ExecuteNonQuery();
                    break;
                case Log.DELETE_SURNAME_FAILURE:
                    msg = "Záznam [" + Name + ", " + Surname + ", " + Age + "] se nepovedlo smazat, pole *Surname* nevyplněno";
                    SqlCommand deleteSurnameFormatErrLog = new SqlCommand("INSERT INTO dbo.Praxe_test_logs VALUES ('" + DateTime.Now.ToString(format) + "', '" + msg + "')", connectToDatabase);
                    deleteSurnameFormatErrLog.ExecuteNonQuery();
                    break;
                default:
                    MessageBox.Show("Unsupported Log type (?? *_* ??) ", "Invalid Log type", MessageBoxButton.OK, MessageBoxImage.Error);
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
