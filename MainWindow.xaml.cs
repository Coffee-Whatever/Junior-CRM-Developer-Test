﻿using MySql.Data.MySqlClient;
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
            DataTable dtable;
            string query = "";
            (bool, DataTable) Qresult;

            query = $"SELECT current_salt FROM `credentials` WHERE login = \"{log}\";";
            Qresult = DBQuery(query);

            if(Qresult.Item1)
            {
                MessageBox.Show("Incorrect login or password");
                return;
            }

            dtable = Qresult.Item2;
            salt = dtable.Rows[0].ItemArray[0].ToString();

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

            string access = dtable.Rows[0].ItemArray[0].ToString();
            switch (access)
            {
                case "base":
                    {
                        BaseUser window = new BaseUser(Convert.ToInt32(dtable.Rows[0].ItemArray[1]));
                        window.ShowActivated = true;
                        window.Closing += (object sender, CancelEventArgs e) =>
                        {
                            this.Show();
                        };
                        window.Show();

                        this.Hide();
                    }
                    break;
                case "HRmanagement":
                    {

                    }
                    break;
                case "Pmanagement":
                    {

                    }
                    break;
                case "full":
                    {
                        BaseUser window = new BaseUser(Convert.ToInt32(dtable.Rows[0].ItemArray[1]));
                        window.ShowActivated = true;
                        window.Closing += (object sender, CancelEventArgs e) =>
                        {
                            this.Show();
                        };
                        window.Show();

                        this.Hide();
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
            }catch (Exception ex)
            {
                return (true, new DataTable());
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