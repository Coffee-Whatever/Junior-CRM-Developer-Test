using System;
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

namespace Junior_CRM_Developer_Test.PM
{
    /// <summary>
    /// Interaction logic for AddToProject.xaml
    /// </summary>
    public partial class AddToProject : Window
    {
        public int employeeId;
        public AddToProject(int employeeId)
        {
            InitializeComponent();
            this.employeeId = employeeId;

            string query = "SELECT `id`, `type`, (SELECT `full_name` FROM `employees` WHERE `id` = `projects`.`manager`) as 'manager' FROM `projects` WHERE `status` = 'Active';";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong, please try again later.\nIf this issue persist please contact the app administrator");
                this.Close();
                return;
            }

            foreach(DataRow row in result.Item2.Rows)
            {
                var project = new ComboBoxItem();
                project.Content = $"{row["type"]} | {row["manager"]}";
                project.Tag = row["id"].ToString();

                Projects.Items.Add(project);
            }
        }
        public void Save(object sender, EventArgs e)
        {
            int projectId = Convert.ToInt32((Projects.SelectedItem as ComboBoxItem).Tag);
            string query = $"INSERT INTO `employeeprojectrelations` (`employeeId`, `projectId`) VALUES ({employeeId},{projectId});";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong, please try again later.\nIf this issue persist please contact the app administrator");
                return;
            }

            MessageBox.Show("Employee assigned to project.");
            this.Close();
        }
        public void Cancel(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
