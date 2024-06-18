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
            Projects.Clear();
            //Project Generation
            string query = $"SELECT type, start_date, end_date, projects.status, projects.comment, (SELECT full_name FROM employees WHERE id = projects.manager) AS 'Manager' FROM projects;";
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
                project._Manager = row["Manager"].ToString();
                project._Comment = row["comment"].ToString();

                Projects.Add(project);
            }
            HRProjectsDataGrid.ItemsSource = Projects;
        }
    }
}
