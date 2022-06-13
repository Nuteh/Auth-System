using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Security.Permissions;
namespace MainProgram
{

    internal class MainProgram
    {
 
        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Auth.RudjerAuth ZavrsniRad = new Auth.RudjerAuth();
              ZavrsniRad.Cmd_title();
              ZavrsniRad.HWIDChecking();

              if (!ZavrsniRad.checker) {

                  Console.WriteLine("Tvoj HWID nije u bazi podataka!");
                return;
              }
              Console.Clear();
              ZavrsniRad.auth();
              Console.WriteLine("Main code to execute... ");
              Console.ReadKey(); 
            
         
        }
    }
}