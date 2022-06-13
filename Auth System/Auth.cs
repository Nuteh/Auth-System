using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Auth
{
 
    public class Encryption // Dekripcija
    {
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;       
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
    public class RudjerAuth {


        public static string? HWIDLIST;
        public static string? HWID;
        public static string z7raf3 = "2a97nhcy5bam24dsef2s4be35dd95npa";
        public bool checker = false;
        string? username, password, password1;
        // IP address
        string CreateRandomPassword(int length = 30)
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }
        string GetPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }
        // IP address

        public void auth()
        {
            Console.Clear();
            Console.WriteLine("\n 1 | Login\n 2 | Register\n 3 | Forgot password");
            int a = Convert.ToInt32(Console.ReadLine());
            switch (a)
            {

                case 1:

                    Login(0);
                    break;
                case 2:
                    Register(0);
                    break;
                case 3:
                    ForgotPassword(0);
                    break;
                default:
                    auth();
                    break;

            }
        }
        public void Login(int i) // Login
        {
            bool user = false;
            Console.Clear();
            if (i == 1) Console.WriteLine("Invalid username or password!");
            if (i == 2) Console.WriteLine("Successfuly registered!");
            if (i == 404) Console.WriteLine("Invalid username or passowrd!");
            Console.WriteLine("Username: ");
            username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            password = Auth.Encryption.EncryptString(Auth.RudjerAuth.z7raf3, pass);
            // Console.WriteLine("\n" + pass + "\n" + password); leaked password
            // checking username
            try
            { 
                string cs = @"server=localhost;userid=root;password=;database=zavrsni";

                using var con = new MySqlConnection(cs);
                con.Open();

                string sql = "SELECT username FROM users";
                using var cmd = new MySqlCommand(sql, con);

                using MySqlDataReader rdr = cmd.ExecuteReader();
                Thread.Sleep(5000);
                while (rdr.Read())
                {
                    if (rdr.GetString(0) == username)
                    {
                        //username in database
                        rdr.Dispose();
                        string sql2 = "SELECT password FROM users WHERE username = \'" + username + "\';";
                        using var cmd2 = new MySqlCommand(sql2, con);
                        using MySqlDataReader cmd3 = cmd2.ExecuteReader();
                        while (cmd3.Read() && !user)
                        {
                            // password check
                            if (cmd3.GetString(0) == password) {
                                user = true;
                                Console.Clear();
                                return;
                                cmd3.Close();
                            }

                        }

                    }

                }
                if (!user) Login(404);
                if (user) return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                System.Environment.Exit(0);
            }


        }

        public void Register(int i) //Register
        {
            Console.Clear();
            if (i == 1) Console.WriteLine("Passwords are different! Try again!");
            if (i == 3) Console.WriteLine("User already exists!");
            if (i == 4) Console.WriteLine("Password must be between 7 and 25 characters long!");
            Console.WriteLine("Username: ");
            username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var pass = string.Empty;
            var pass1 = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
           password = Auth.Encryption.EncryptString(Auth.RudjerAuth.z7raf3, pass);

            Console.WriteLine("\nRepeat passowrd: ");
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass1.Length > 0)
                {
                    Console.Write("\b \b");
                    pass1 = pass1[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass1 += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            password1 = Auth.Encryption.EncryptString(Auth.RudjerAuth.z7raf3, pass1);
            if (pass != pass1) {
                Console.Clear();

                Register(1);

            }
            if (pass.Length < 7 && pass1.Length > 25) Register(4);
            try
            {
                string cs = @"server=localhost;userid=root;password=;database=zavrsni";

                using var conn = new MySqlConnection(cs);
                conn.Open();
                using var ippp = new MySqlCommand();
                ippp.Connection = conn;
                string sql2 = "SELECT * FROM users";
                using var cmd2 = new MySqlCommand(sql2, conn);

                using MySqlDataReader rdr = cmd2.ExecuteReader();



                Thread.Sleep(5000);
                while (rdr.Read())
                {
                    if (rdr.GetString(0) == username)
                    {

                        Register(3);
                    }
                }
                rdr.Close();
                var sql = "INSERT INTO users(username,password) VALUES(@username, @password)";
                using var cmd3 = new MySqlCommand(sql, conn);

                cmd3.Parameters.AddWithValue("@username", username);
                cmd3.Parameters.AddWithValue("@password", password);
                cmd3.Prepare();
                cmd3.ExecuteNonQuery();

                string sql3 = "CREATE TABLE  " + username + " (IP varchar(50), RECOVERY varchar(100));";
                using var dtb = new MySqlCommand(sql3, conn);
                using MySqlDataReader rdr2 = dtb.ExecuteReader();
                rdr2.Close();
                string rplank = CreateRandomPassword();
                string rr =Encryption.EncryptString(z7raf3, rplank);
                ippp.CommandText = "INSERT INTO " + username + "(IP,RECOVERY) VALUE(\'" + GetPublicIP() + "\', \'" + /*Auth.Encryption.EncryptString(Auth.RudjerAuth.z7raf3, rr)*/ rr + "\' )";
                ippp.ExecuteNonQuery();
                Console.Clear();
                Console.WriteLine("Your recovery password: " + rplank);
                Console.WriteLine("Write CONFIRM when u saved it!");
                Console.ReadLine();
                Login(2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                System.Environment.Exit(0);
            }

        }

        public void ForgotPassword(int a)
        {
            Console.Clear();
            if (a == 2) Console.WriteLine("Wrong recovery code!");
            bool iff = false;
            try
            {
              
                Console.WriteLine("Username: ");
                string us = Console.ReadLine();
                Console.WriteLine("Recovery password: ");
                string rp = Encryption.EncryptString(z7raf3,Console.ReadLine());
                string cs = @"server=localhost;userid=root;password=;database=zavrsni";

                using var con = new MySqlConnection(cs);
                con.Open();
                string sql = "SELECT RECOVERY FROM " + us;
                using var cmd = new MySqlCommand(sql, con);
                Console.WriteLine("good");

                using MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {

                    if ( rdr.GetString(0) == rp)
                    {
                        Console.WriteLine("Change your password!");
                        Console.ReadKey();
                        iff = true;
                        rdr.Close();
                        RecoverPassword(0);
                    }
                }
                if (!iff) ForgotPassword(2);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                System.Environment.Exit(0);
            }

            void RecoverPassword(int a)
            {
                Console.Clear();
                if (a == 2) Console.WriteLine("Passwords are different!");

                Console.WriteLine("New password: ");
                string b = Console.ReadLine();
                Console.WriteLine("Repeat password: ");
                string c = Console.ReadLine();
                if (b != c) RecoverPassword(2);

            }
        }
        public void Cmd_title()
        {

            string getOSInfo()
            {
                //Get Operating system information.
                OperatingSystem os = Environment.OSVersion;
                //Get version information about the os.
                Version vs = os.Version;

                //Variable to hold our return value
                string operatingSystem = "";

                if (os.Platform == PlatformID.Win32Windows)
                {
                    //This is a pre-NT version of Windows
                    switch (vs.Minor)
                    {
                        case 0:
                            operatingSystem = "95";
                            break;
                        case 10:
                            if (vs.Revision.ToString() == "2222A")
                                operatingSystem = "98SE";
                            else
                                operatingSystem = "98";
                            break;
                        case 90:
                            operatingSystem = "Me";
                            break;
                        default:
                            break;
                    }
                }
                else if (os.Platform == PlatformID.Win32NT)
                {
                    switch (vs.Major)
                    {
                        case 3:
                            operatingSystem = "NT 3.51";
                            break;
                        case 4:
                            operatingSystem = "NT 4.0";
                            break;
                        case 5:
                            if (vs.Minor == 0)
                                operatingSystem = "2000";
                            else
                                operatingSystem = "XP";
                            break;
                        case 6:
                            if (vs.Minor == 0)
                                operatingSystem = "Vista";
                            else if (vs.Minor == 1)
                                operatingSystem = "7";
                            else if (vs.Minor == 2)
                                operatingSystem = "8";
                            else
                                operatingSystem = "8.1";
                            break;
                        case 10:
                            operatingSystem = "10";
                            break;
                        default:
                            break;
                    }
                }
                //Make sure we actually got something in our OS check
                //We don't want to just return " Service Pack 2" or " 32-bit"
                //That information is useless without the OS version.
                if (operatingSystem != "")
                {
                    //Got something.  Let's prepend "Windows" and get more info.
                    operatingSystem = "Windows " + operatingSystem;
                    //See if there's a service pack installed. č#
                    if (os.ServicePack != "")
                    {
                        //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                        operatingSystem += " " + os.ServicePack;
                    }
                    //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                    //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
                }
                //Return the information we've gathered.
                return operatingSystem;
            }

            Console.Title = System.Security.Principal.WindowsIdentity.GetCurrent().Name + " - Windows: " + getOSInfo();


        }
        public void HWIDChecking()
        {
            bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
            {
                try
                {
                    url ??= CultureInfo.InstalledUICulture switch
                    {
                        { Name: var n } when n.StartsWith("fa") => 
                            "http://www.aparat.com",
                        { Name: var n } when n.StartsWith("zh") => 
                            "http://www.baidu.com",
                        _ =>
                            "http://www.gstatic.com/generate_204",
                    };

                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.KeepAlive = false;
                    request.Timeout = timeoutMs;
                    using (var response = (HttpWebResponse)request.GetResponse())
                        return true;

                }
                catch
                {
                    return false;
                }
            }
            if (CheckForInternetConnection())
            {
                Console.WriteLine("Connected to internet, redirecting...");
                Thread.Sleep(500);
                Console.Clear();
                        
                Console.WriteLine("Connecting to Auth Base server...");
                try {
                    string cs = @"server=localhost;userid=root;password=;database=zavrsni";

                    using var con = new MySqlConnection(cs);
                    con.Open();
                    Console.WriteLine("Getting HWID ...");
                   
                    HWID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
                     Console.WriteLine("Normal: " + Encryption.DecryptString(z7raf3, Encryption.EncryptString(z7raf3,HWID)));
                     Console.WriteLine("Encrypted: " + Encryption.EncryptString(z7raf3, HWID));
                    Console.WriteLine("Checking HWID...");
                   
                
                }

                catch
                {
                    Console.Clear();
                    Console.WriteLine("Unable to connect to the Auth Base server.");
                    Console.WriteLine("Press Any key to continue...");
                    Console.ReadKey();
                    System.Environment.Exit(0);
                }

                try
                {
                    string cs = @"server=localhost;userid=root;password=;database=zavrsni";

                    using var con = new MySqlConnection(cs);
                    con.Open();

                    Console.WriteLine($"MySQL version : {con.ServerVersion}");
                    string sql = "SELECT * FROM hwid";
                    using var cmd = new MySqlCommand(sql, con);

                    using MySqlDataReader rdr = cmd.ExecuteReader();
                    Thread.Sleep(5000);
                    while (rdr.Read())
                    {
                       if(rdr.GetString(0) == Encryption.EncryptString(z7raf3, HWID))
                        {

                            checker = true;
                            Console.WriteLine("Tvoj HWID je u bazi podataka!");
                            cmd.Dispose();
                            return;
                        } 
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Failed to connect to the internet !");
                Console.WriteLine("Press Any key to close...");
                Console.ReadKey();
                Console.Clear();
                System.Environment.Exit(0);
            }

        }
     
    }
    internal class Program
    {
        

    }
}

