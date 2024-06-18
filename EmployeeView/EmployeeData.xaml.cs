using Google.Protobuf.WellKnownTypes;
using Junior_CRM_Developer_Test.HRM;
using Microsoft.Win32;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

namespace Junior_CRM_Developer_Test.EmployeeView
{
    public partial class EmployeeData : Window
    {
        public static string HexDecPicture = "";
        public static bool Modify = false;
        public static bool ErrorHappened = false;
        public static Employee ModifiedEmployee { get; set; }

        public EmployeeData()
        {
            InitializeComponent();
            setUp();
            Modify = false;
            ErrorHappened = false;
        }
        public EmployeeData(Employee employee)
        {
            InitializeComponent();
            setUp();
            Modify = true;
            ErrorHappened = false;
            FEName.Text = employee._Name;
            int i = 0;
            foreach(var sub in Subdivision.Items)
            {
                var sub2 = sub as ComboBoxItem;
                if(sub2.Content.ToString() == employee._Subdivision)
                {
                    Subdivision.SelectedIndex = i;
                }
                i++;
            }
            i = 0;
            foreach(var sub in Position.Items)
            {
                var sub2 = sub as ComboBoxItem;
                if(sub2.Content.ToString() == employee._Position)
                {
                    Position.SelectedIndex = i;
                }
                i++;
            }
            i = 0;
            foreach(var sub in Partner.Items)
            {
                var sub2 = sub as ComboBoxItem;
                if(sub2.Content.ToString() == $"{employee._Partner} | {employee._Subdivision}")
                {
                   Partner.SelectedIndex = i;
                }
                i++;
            }
            ModifiedEmployee = employee;
        }
        public void setUp()
        {
            string query = "SHOW COLUMNS FROM employees where FIELD = 'subdivision' OR FIELD = 'position';";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Couldn't query the database.\nPlease contact the app administrator");
                return;
            }

            //enum('Admin','HR','Dev','Sales')
            //enum(' + ') = 8 chars, that's why there's a - 8 at length 
            string temp = result.Item2.Rows[0]["Type"].ToString();
            List<string> subdivisionOptions = temp.Substring(6, temp.Length - 8).Split("','").ToList();
            temp = result.Item2.Rows[1]["Type"].ToString();
            List<string> positionOptions = temp.Substring(6, temp.Length - 8).Split("','").ToList();

            foreach (string so in subdivisionOptions)
            {
                var stuff = new ComboBoxItem { Content = so };
                Subdivision.Items.Add(stuff);
            }

            foreach (string po in positionOptions)
            {
                var stuff = new ComboBoxItem { Content = po };
                Position.Items.Add(stuff);
            }

            query = "SELECT id, full_name, subdivision FROM `employees`;";
            result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong.");
                return;
            }
            foreach (DataRow row in result.Item2.Rows)
            {
                var PeoplePartner = new ComboBoxItem { Content = $"{row["full_name"]} | {row["subdivision"]}" };
                PeoplePartner.Tag = row["id"];
                Partner.Items.Add(PeoplePartner);
            }
        }
        private void AddAPic(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            dialog.Multiselect = false;
            var result = dialog.ShowDialog();

            if (result.Value == true)
            {
                var stream = dialog.OpenFile();
                byte[] file = new byte[stream.Length];
                stream.Read(file);
                HexDecPicture = BitConverter.ToString(file).Replace("-", "");
                List<string> filename = dialog.FileName.Split("\\").ToList();
                pictureName.Text = $"Selected file:         {filename.Last()}";
            }
            else
            {
                return;
            }
        }
        public void Save(object sender, RoutedEventArgs e)
        {
            if (Modify)
            {
                ModifiedEmployee._Name = FEName.Text;
                ModifiedEmployee._Subdivision = (Subdivision.SelectedValue as ComboBoxItem).Content.ToString();
                ModifiedEmployee._Position = (Position.SelectedValue as ComboBoxItem).Content.ToString();
                ModifiedEmployee._Partner = (Partner.SelectedValue as ComboBoxItem).Content.ToString();

                var query = $"UPDATE `employees` " +
                    $"SET `full_name`='{ModifiedEmployee._Name}',`subdivision`='{ModifiedEmployee._Subdivision}',`position`='{ModifiedEmployee._Position}',`people_partner`={(Partner.SelectedValue as ComboBoxItem).Tag},`picture`=x'{HexDecPicture}' "+ 
                    $"WHERE `id` = {ModifiedEmployee._Id};";
                var result = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Something went wrong");
                    ErrorHappened = true;
                    return;
                }
            }
            else
            {
                var newInstance = new Employee();
                newInstance._Name = FEName.Text;
                newInstance._Subdivision = (Subdivision.SelectedValue as ComboBoxItem).Content.ToString();
                newInstance._Position = (Position.SelectedValue as ComboBoxItem).Content.ToString();
                newInstance._OOOB = 26;
                newInstance._Status = "Active";
                newInstance._Partner = (Partner.SelectedValue as ComboBoxItem).Content.ToString();

                var query = "SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'crmtest' AND TABLE_NAME = 'employees';";
                var result = MainWindow.DBQuery(query);

                newInstance._Id = Convert.ToInt32(result.Item2.Rows[0].ItemArray[0]);

                query = $"INSERT INTO `employees` (`id`, `full_name`, `subdivision`, `position`, `people_partner`, `picture`) VALUES ({newInstance._Id},'{newInstance._Name}','{newInstance._Subdivision}','{newInstance._Position}',{(Partner.SelectedValue as ComboBoxItem).Tag},x'{HexDecPicture}')";
                result = MainWindow.DBQuery(query);
                
                if (result.Item1)
                {
                    MessageBox.Show("Something went wrong");
                    return;
                }

                HREmployees.Employees.Add(newInstance);

            }
            this.Close();
        }
    }
}
