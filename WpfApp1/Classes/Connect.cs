using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormApp.Classes
{
    internal class Connect
    {
        private static string connString =
            "Data Source=BC7SQ1K2;" +
            "Initial Catalog=ADF_IT;" +
            "Integrated Security=true;";

        public static string ConnString
        {
            get { return connString; }
            set { connString = value; }
        }
    }
}
