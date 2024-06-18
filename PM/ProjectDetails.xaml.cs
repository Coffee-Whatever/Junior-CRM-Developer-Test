using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace Junior_CRM_Developer_Test.PM
{
    public partial class ProjectDetails : Window
    {
        public bool isModified = false;
        public bool error = false;
        private Project copy;
        public ProjectDetails()
        {
            isModified = false;
            error = false;
            InitializeComponent();
            SetUp();
        }
        public ProjectDetails(Project projectObject)
        {
            copy = projectObject;
            isModified = true;
            error = false;
            InitializeComponent();
            SetUp(true);
            int i = 0;
            foreach(ComboBoxItem item in Type.Items)
            {
                if(item.Content.ToString() == projectObject._Type) 
                {
                    Type.SelectedIndex = i;
                    break;
                }
                i++;
            }
            i = 0;
            foreach(ComboBoxItem item in Status.Items)
            {
                if(item.Content.ToString() == projectObject._Status) 
                {
                    Status.SelectedIndex = i;
                    break;
                }
                i++;
            }
            i = 0;
            foreach(ComboBoxItem item in Manager.Items)
            {
                if(item.Content.ToString() == projectObject._Manager && item.Tag.ToString() == projectObject._ManagerId) 
                {
                    Manager.SelectedIndex = i;
                    break;
                }
                i++;
            }
            Comment.Text = projectObject._Comment;
            StartDate.SelectedDate = projectObject._StartDate;
            EndDate.SelectedDate = projectObject._EndDate;
        }

        private void SetUp(bool setM = false)
        {

            string query = "SHOW COLUMNS FROM projects WHERE FIELD = 'type' OR FIELD = 'status';";
            var result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                MessageBox.Show("Something went wrong during data collection.\nPlease try again and/or contact the app administrator.");
            }

            string type = result.Item2.Rows[0]["Type"].ToString();
            string status = result.Item2.Rows[1]["Type"].ToString();

            List<string> types = type.Substring(6, type.Length - 8).Split("','").ToList();
            List<string> statuses = status.Substring(6, status.Length - 8).Split("','").ToList(); // aparently the correct plural form of status is statuses, sounds weird but alright.

            foreach (string instance in types)
            {
                var combobox = new ComboBoxItem {
                    Content = instance 
                };

                Type.Items.Add(combobox);
            }
            foreach (string instance in statuses)
            {
                var combobox = new ComboBoxItem{
                    Content = instance
                };
                Status.Items.Add(combobox);
            }

            query = "SELECT `id`, `full_name` FROM `employees`;";
            result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                MessageBox.Show("Something went wrong during data collection.\nPlease try again and/or contact the app administrator.");
            }
            foreach (DataRow row in result.Item2.Rows)
            {
                var combobox = new ComboBoxItem{ 
                    Content = $"{row["full_name"]}",
                    Tag = Convert.ToInt32(row["id"])
                };
                Manager.Items.Add(combobox);
            }
        }
        private void Update(object sender, RoutedEventArgs e)
        {
            if(isModified)
            {
                Project instance = copy;
                instance._Type = (Type.SelectedItem as ComboBoxItem).Content.ToString();
                instance._Manager = (Manager.SelectedItem as ComboBoxItem).Content.ToString();
                instance._ManagerId = (Manager.SelectedItem as ComboBoxItem).Tag.ToString();
                instance._Status = (Status.SelectedItem as ComboBoxItem).Content.ToString();
                instance._Comment = Comment.Text;
                instance._StartDate = StartDate.SelectedDate.Value;
                instance._EndDate = EndDate.SelectedDate.Value;

                string sd = $"{instance._StartDate.Year}-{instance._StartDate.Month}-{instance._StartDate.Day}";
                string ed = $"{instance._EndDate.Year}-{instance._EndDate.Month}-{instance._EndDate.Day}";

                var query = $"UPDATE `projects` SET `type`='{instance._Type}',`start_date`='{instance._StartDate}',`end_date`='{instance._EndDate}',`manager`={instance._ManagerId},`comment`='{instance._Comment}',`status`='{instance._Status}' WHERE `id`={instance._Id}";
                var result = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Sorry, but something went wrong.\nPlease contact the app administrtor.");
                    error = true;
                    this.Close();
                    return;
                }

                var index = PMProjects.Projects.IndexOf(copy);
                PMProjects.Projects[index] = instance;
            }
            else
            {
                Project instance = new Project();
                instance._Type = (Type.SelectedItem as ComboBoxItem).Content.ToString();
                instance._Manager = (Manager.SelectedItem as ComboBoxItem).Content.ToString();
                instance._ManagerId = (Manager.SelectedItem as ComboBoxItem).Tag.ToString();
                instance._Status = (Status.SelectedItem as ComboBoxItem).Content.ToString();
                instance._Comment = Comment.Text;
                instance._StartDate = StartDate.SelectedDate.Value;
                instance._EndDate = EndDate.SelectedDate.Value;

                string query = "SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'crmtest' AND   TABLE_NAME   = 'projects';";
                var result = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Sorry, but something went wrong.\nPlease contact the app administrtor.");
                    error = true;
                    this.Close();
                    return;
                }
                string sd = $"{instance._StartDate.Year}-{instance._StartDate.Month}-{instance._StartDate.Day}";
                string ed = $"{instance._EndDate.Year}-{instance._EndDate.Month}-{instance._EndDate.Day}";

                query = $"INSERT INTO `projects`(`id`, `type`, `start_date`, `end_date`, `manager`, `comment`, `status`) VALUES ({instance._Id},'{instance._Type}','{sd}','{ed}',{instance._ManagerId},'{instance._Comment}','{instance._Status}')";
                result = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Sorry, but something went wrong.\nPlease contact the app administrtor.");
                    error = true;
                    this.Close();
                    return;
                }

                instance._Id = Convert.ToInt32(result.Item2.Rows[0][0]);
                PMProjects.Projects.Add(instance);
            }
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
