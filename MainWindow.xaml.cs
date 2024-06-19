using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Windows.Documents.Serialization;
using System.ComponentModel;
using Junior_CRM_Developer_Test.Admin;
using Junior_CRM_Developer_Test.HRM;
using Junior_CRM_Developer_Test.PM;
using Junior_CRM_Developer_Test.ManagerShared;

namespace Junior_CRM_Developer_Test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DBQuery("SET GLOBAL max_allowed_packet=33554432;");
        }
        private void SignInParsing(object sender, RoutedEventArgs e)
        {
            string log = login.Text;
            string salt = "";
            DataTable dtable;
            string query = "";
            (bool, DataTable) Qresult;

            query = $"SELECT current_salt FROM `credentials` WHERE login = \"{log}\";";
            Qresult = DBQuery(query);

            if(Qresult.Item1 || Qresult.Item2.Rows.Count == 0)
            {
                MessageBox.Show("Incorrect login or password");
                return;
            }

            dtable = Qresult.Item2;
            salt = dtable.Rows[0]["current_salt"].ToString();

            string pass = GetHashString(password.Password, salt);

            query = $"SELECT CASE `password` WHEN '{pass}' THEN 1 ELSE 0 END isCorrect FROM `credentials` WHERE login = \"{log}\";";
            Qresult = DBQuery(query);
            if(Qresult.Item1) 
            {
                MessageBox.Show("Incorrect login or password");
                return;
            }

            dtable = Qresult.Item2;

            if (dtable.Rows[0].ItemArray[0].ToString() == "0")
            {
                MessageBox.Show("Incorrect login or password");
                return;
            }
            query = $"SELECT access_level, employee_id FROM `credentials` WHERE login = \"{log}\";";

            Qresult = DBQuery(query);
            if(Qresult.Item1)
            {
                MessageBox.Show("Sorry, something went wrong during credentials gathering.\nPlease contact the app administrtor");
                return;
            }
            dtable = Qresult.Item2;

            string access = dtable.Rows[0]["access_level"].ToString();
            switch (access)
            {
                case "base":
                    {
                        BaseUser window = new BaseUser(Convert.ToInt32(dtable.Rows[0]["employee_id"]));
                        window.ShowActivated = true;
                        window.Closing += (object sender, CancelEventArgs e) =>
                        {
                            this.Show();
                            this.Activate();
                        };
                        window.Show();

                        this.Hide();
                    }
                    break;
                case "HRmanagement":
                    {
                        HRMenu window = new HRMenu(Convert.ToInt32(dtable.Rows[0]["employee_id"]));
                        window.ShowActivated = true;
                        window.Closing += (object sender, CancelEventArgs e) =>
                        {
                            this.Show();
                            this.Activate();
                        };
                        window.Show();

                        this.Hide();
                    }
                    break;
                case "Pmanagement":
                    {
                        PMMenu window = new PMMenu(Convert.ToInt32(dtable.Rows[0]["employee_id"]));
                        window.ShowActivated = true;
                        window.Closing += (object sender, CancelEventArgs e) =>
                        {
                            this.Show();
                            this.Activate();
                        };
                        window.Show();

                        this.Hide();
                    }
                    break;
                case "full":
                    {
                        AddCredentials window = new AddCredentials();
                        window.ShowActivated = true;
                        window.Closing += (object sender, CancelEventArgs e) =>
                        {
                            this.Show();
                            this.Activate();
                        };
                        try
                        {
                            window.Show();
                            this.Hide();
                        }
                        catch (Exception ex)
                        {
                            this.Show();
                        }
                    }
                    break;
            }
        }

        public static (bool, DataTable) DBQuery(string query)
        {
            // (didErrorHappen, ResultIfNoError)
            try
            {
                string connetionstring = "Server=localhost;Database=crmtest;Uid=root;Convert Zero Datetime=True";
                MySqlConnection conn = new MySqlConnection(connetionstring);
                conn.Open();

                MySqlDataAdapter dtb = new MySqlDataAdapter();
                dtb.SelectCommand = new MySqlCommand(query, conn);
                DataTable dtable = new DataTable();
                dtb.Fill(dtable);
                return (false, dtable);
            } catch (Exception ex)
            {
                return (true, new DataTable());
            }
        }
        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        public static string GetHashString(string inputString, string salt)
        {
            //string salt = RandomNumberGenerator.GetString("1234567890!@#$%^&*()qwertyuiopasdfghjklzxcvbnm,.<>:;[]{}", 20);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(String.Concat(inputString, salt)))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}