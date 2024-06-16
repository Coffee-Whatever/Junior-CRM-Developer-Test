using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Junior_CRM_Developer_Test.HRM
{
    public partial class HRProjects : Window
    {
        public static ObservableCollection<Project> Projects = new ObservableCollection<Project>();
        public HRProjects()
        {
            InitializeComponent();
            //Project Generation
            string query = $"SELECT type, start_date, end_date, projects.status, projects.comment, (SELECT full_name FROM employees WHERE id = projects.manager) AS 'Manager' FROM projects JOIN employeeprojectrelations ON projects.id = employeeprojectrelations.projectId JOIN employees ON employeeprojectrelations.employeeId = employees.id;";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Sorry, but something went wrong during project gathering process.\nPlease contact the app administrtor");
                return;
            }

            foreach (DataRow row in result.Item2.Rows)
            {
                Project project = new Project();

                project._Type = row["type"].ToString();
                project._StartDate = Convert.ToDateTime(row["start_date"]);
                project._EndDate = Convert.ToDateTime(row["end_date"]);
                project._Status = row["status"].ToString();
                project._Manager = row["comment"].ToString();
                project._Comment = row["Manager"].ToString();

                Projects.Add(project);
            }
            HRProjectsDataGrid.ItemsSource = Projects;

            query = "SHOW COLUMNS FROM projects WHERE FIELD = 'type';";
            result = MainWindow.DBQuery(query);
            if(result.Item1)
            {
                MessageBox.Show("Sorry, but something went wrong during adjusting project creation form.\nPlease contact the app administrtor");
                return;
            }
            string projectsEnum = result.Item2.Rows[0]["Type"].ToString();
            List<string> projectTypes = projectsEnum.Substring(6, projectsEnum.Length - 8).Split("','").ToList();
            foreach (string type in projectTypes)
            {
                var instance = new ComboBoxItem();
                instance.Content = type;
                ProjectType.Items.Add(instance);
            }

            query = "SELECT id, full_name, subdivision FROM `employees` WHERE position = 'HR Manager' OR id IN (SELECT DISTINCT `manager` FROM `projects`);";
            result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Sorry, but something went wrong during adjusting project creation form.\nPlease contact the app administrtor");
                return;
            }
            foreach(DataRow row in result.Item2.Rows)
            {
                var instance = new ComboBoxItem();
                instance.Content = $"{row["full_name"]} | {row["subdivision"]}";
                instance.Tag = Convert.ToInt32(row["id"]);
                ManagerCombobox.Items.Add(instance);
            }
        }

        private void NewProject(object sender, RoutedEventArgs e)
        {
            Project project = new Project();
            project._Type = (ProjectType.SelectedItem as ComboBoxItem).Content.ToString();
            project._StartDate = startDate.SelectedDate;
            project._EndDate = endDate.SelectedDate;
            project._Manager = (ManagerCombobox.SelectedItem as ComboBoxItem).Content.ToString();
            project._Comment = CommentBox.Text;

            string sd = $"{project._StartDate.Value.Year}-{project._StartDate.Value.Month}-{project._StartDate.Value.Day}";
            string ed = $"{project._EndDate.Value.Year}-{project._EndDate.Value.Month}-{project._EndDate.Value.Day}";
            string query = $"INSERT INTO `projects`(`type`, `start_date`, `end_date`, `manager`, `comment`) VALUES ('{project._Type}','{sd}','{ed}','{(ManagerCombobox.SelectedItem as ComboBoxItem).Tag}','{project._Comment}')";

            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Sorry, but something went wrong during database connection.\nPlease try again and/or contact the app administrtor");
                return;
            }

            Projects.Add(project);
            HRProjectsDataGrid.ItemsSource = null;
            HRProjectsDataGrid.ItemsSource = Projects;
            HRProjectsDataGrid.UpdateLayout();
        }
    }
}
