using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Junior_CRM_Developer_Test.PM
{
    public partial class PMProjects : Window
    {
        public static ObservableCollection<Project> Projects = new ObservableCollection<Project>();
        public PMProjects()
        {
            InitializeComponent();
            Projects.Clear();
            //Project Generation
            string query = $"SELECT id, type, start_date, end_date, projects.status, projects.comment, (SELECT full_name FROM employees WHERE id = projects.manager) AS 'Manager', projects.manager as 'IdM' FROM projects;";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Sorry, but something went wrong during project gathering process.\nPlease contact the app administrtor");
            }
            else
            {
                foreach (DataRow row in result.Item2.Rows)
                {
                    Project project = new Project();

                    project._Id = Convert.ToInt32(row["id"]);
                    project._Type = row["type"].ToString();
                    project._StartDate = Convert.ToDateTime(row["start_date"]);
                    project._EndDate = Convert.ToDateTime(row["end_date"]);
                    project._Status = row["status"].ToString();
                    project._Manager = row["Manager"].ToString();
                    project._Comment = row["comment"].ToString();
                    project._ManagerId = row["IdM"].ToString();
                    Projects.Add(project);
                }
                ProjectsDataGrid.ItemsSource = Projects;
            }
        }
        public void Add(object sender, RoutedEventArgs e)
        {
            ProjectDetails window = new ProjectDetails();
            window.Owner = this;
            window.ShowDialog();

            if (!window.error)
            {
                ProjectsDataGrid.ItemsSource = null;
                ProjectsDataGrid.ItemsSource = Projects;
                ProjectsDataGrid.UpdateLayout();
            }
        }
        public void Update(object sender, RoutedEventArgs e)
        {
            var selected = ProjectsDataGrid.SelectedItem as Project;
            if (selected == null)
            {
                MessageBox.Show("Please select a record to update.");
                return;
            }

            ProjectDetails window = new ProjectDetails(selected);
            window.Owner = this;
            window.ShowDialog();

            if(!window.error)
            {
                ProjectsDataGrid.ItemsSource = null;
                ProjectsDataGrid.ItemsSource = Projects;
                ProjectsDataGrid.UpdateLayout();
            }
        }
        public void Deactivate(object sender, RoutedEventArgs e)
        {
            var selected = ProjectsDataGrid.SelectedItem as Project;

            if (selected == null)
            {
                MessageBox.Show("Please select a record to deactivate.");
                return;
            }
            if (selected._Status == "Inactive") return;

            int projectId = selected._Id;
            string query = $"UPDATE `projects` SET `status`='Inactive' WHERE `id` = {projectId};";
            
            var result = MainWindow.DBQuery(query);
            
            if (result.Item1)
            {
                MessageBox.Show("Sorry, but something went wrong.\nPlease contact the app administrtor.");
                return;
            }

            int id = Projects.IndexOf(selected);
            Projects[id]._Status = "Inactive";

            ProjectsDataGrid.ItemsSource = null;
            ProjectsDataGrid.ItemsSource = Projects;
            ProjectsDataGrid.UpdateLayout();

            MessageBox.Show("Project status updated;");
        }
    }
}
