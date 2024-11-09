using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Utils
{
    static class Utils
    {

        static string arrayToString(string[] array)
        {
            string result = "";
            foreach (string item in array)
            {
                result += (item + "\n");
            }
            return result;
        }

    }
}
