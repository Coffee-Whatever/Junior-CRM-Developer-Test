using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Junior_CRM_Developer_Test.Admin
{
    /// <summary>
    /// Interaction logic for AddCredentials.xaml
    /// </summary>
    public partial class AddCredentials : Window
    {
        public AddCredentials()
        {
            InitializeComponent();
            string query = "SELECT `id`, `full_name`, `subdivision` FROM `employees` WHERE `id` NOT IN (SELECT DISTINCT `employee_id` FROM `credentials`);";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact the app administrator");
                this.Close();
                return;
            }
            if(result.Item2.Rows.Count != 0)
            {
                foreach (DataRow row in result.Item2.Rows)
                {
                    var instance = new ComboBoxItem { Content = $"{row["full_name"]} - {row["subdivision"]}" };
                    instance.Tag = row["id"].ToString();
                    Id.Items.Add(instance);
                }

                query = "SHOW COLUMNS FROM credentials WHERE FIELD = 'access_level';";
                result = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Something went wrong.\nPlease contact the app administrator");
                    this.Close();
                    return;
                }
                //enum('base','HRmanagement','Pmanagement','full')
                string temp = result.Item2.Rows[0].ItemArray[1].ToString();
                List<string> accesslevels = temp.Substring(6, temp.Length - 8).Split("','").ToList();
                foreach (var AL in accesslevels)
                {
                    var instance = new ComboBoxItem { Content = AL };
                    instance.Tag = AL;
                    AccessLevel.Items.Add(instance);
                }
            }
            else
            {
                MessageBox.Show("all employees have accounts");
                this.Close(); 
                return;
            }
        }

        private void CreateNewCredentials(object sender, RoutedEventArgs e)
        {
            if (login.Text == null) return;
            if(Id.SelectedItem == null) return;
            if(password.Text == null) return;
            if(AccessLevel.SelectedItem == null) return;

            string Userlogin = login.Text;
            string salt = RandomNumberGenerator.GetString("1234567890!@#$%^&*()qwertyuiopasdfghjklzxcvbnm,.<>:;[]{}", 20);
            string pass = MainWindow.GetHashString(password.Text, salt);
            string UserCredentials = (AccessLevel.SelectedItem as ComboBoxItem).Content.ToString();
            string UserId = (Id.SelectedItem as ComboBoxItem).Tag.ToString();

            string query = "INSERT INTO `credentials`(`employee_id`, `login`, `password`, `current_salt`, `access_level`) " +
                $"VALUES ({UserId},'{Userlogin}','{pass}','{salt}','{UserCredentials}')";

            var result = MainWindow.DBQuery(query);
            if(result.Item1)
            {
                MessageBox.Show("Something went wrong.\nPlease check your data and try again.\nIf the issue persists please contact the app administrator");
            }

            MessageBox.Show("User credentials added correctly.");
            AccessLevel.SelectedItem = null;
            Id.SelectedItem = null;
            login.Text = string.Empty;
            password.Text = string.Empty;
        }
    }
}
