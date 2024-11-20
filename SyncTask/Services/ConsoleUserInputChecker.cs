using SyncTask.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Services
{
    internal class ConsoleUserInputChecker : IUserInputChecker
    {
        public bool UserPressedKey()
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                return true;
            }
            return false;
        }

    }    
}
