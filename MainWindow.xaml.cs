using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Windows.Documents.Serialization;
using System.ComponentModel;

namespace Junior_CRM_Developer_Test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void SignInParsing(object sender, RoutedEventArgs e)
        {
            string log = login.Text;
            string salt = "";
            try
            {
                string connetionstring = "Server=localhost;Database=crmtest;Uid=root;";
                MySqlConnection conn = new MySqlConnection(connetionstring);
                conn.Open();

                string query = $"SELECT current_salt FROM `credentials` WHERE login = \"{log}\";";

                MySqlDataAdapter dtb = new MySqlDataAdapter();
                dtb.SelectCommand = new MySqlCommand(query, conn);
                DataTable dtable = new DataTable();
                dtb.Fill(dtable);
                salt = dtable.Rows[0].ItemArray[0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect login or password");
            }

            string pass = GetHashString(password.Password, salt);
            try
            {
                string connetionstring = "Server=localhost;Database=crmtest;Uid=root;";
                MySqlConnection conn = new MySqlConnection(connetionstring);
                conn.Open();

                string query = $"SELECT CASE `password` WHEN '{pass}' THEN 1 ELSE 0 END isCorrect FROM `credentials` WHERE login = \"{log}\";";

                MySqlDataAdapter dtb = new MySqlDataAdapter();
                dtb.SelectCommand = new MySqlCommand(query, conn);
                DataTable dtable = new DataTable();
                dtb.Fill(dtable);
                if (dtable.Rows[0].ItemArray[0].ToString() == "0")
                {
                    MessageBox.Show("Incorrect login or password");
                }
                else
                {
                    Menu menu = new Menu(log);
                    menu.ShowActivated = true;
                    menu.Closing += (object sender, CancelEventArgs e) =>
                    {
                        this.Show();
                    };
                    menu.Show();

                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect login or password");
            }
        }
        private static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        private static string GetHashString(string inputString, string salt)
        {
            //string salt = RandomNumberGenerator.GetString("1234567890!@#$%^&*()qwertyuiopasdfghjklzxcvbnm,.<>:;[]{}", 20);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(String.Concat(inputString, salt)))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}