using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
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
using System.Xml.Linq;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Junior_CRM_Developer_Test
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        string login;
        public ObservableCollection<Project> Projects { get; set; } = new();
        public Menu(string login)
        {
            this.login = login;
            InitializeComponent();
        }
        private void PassedSignIn()
        {
            try
            {
                string connetionstring = "Server=localhost;Database=crmtest;Uid=root;";
                MySqlConnection conn = new MySqlConnection(connetionstring);
                conn.Open();

                string query = $"SELECT access_level FROM `credentials` WHERE login = \"{this.login}\";";

                MySqlDataAdapter dtb = new MySqlDataAdapter();
                dtb.SelectCommand = new MySqlCommand(query, conn);
                DataTable dtable = new DataTable();
                dtb.Fill(dtable);

                string access = dtable.Rows[0].ItemArray[0].ToString();
                if (access == "base")
                {
                    BaseUser();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect login or password");
            }
        }
        public void BaseUser()
        {
            {
                string connetionstring = "Server=localhost;Database=crmtest;Uid=root;";
                MySqlConnection conn = new MySqlConnection(connetionstring);
                conn.Open();

                string query = $"SELECT * FROM projects" +
                    $"JOIN employeeprojectrelations ON projects.id = employeeprojectrelations.projectId" +
                    $"JOIN employees ON employeeprojectrelations.employeeId = employees.id" +
                    $"WHERE employees.id = {0};";

                MySqlDataAdapter dtb = new MySqlDataAdapter();
                dtb.SelectCommand = new MySqlCommand(query, conn);
                DataTable dtable = new DataTable();
                dtb.Fill(dtable);
                foreach (DataRow row in dtable.Rows)
                {
                    Project project = new Project();
                    project.Type = row.ItemArray[1].ToString();
                    project.StartDate = Convert.ToDateTime(row.ItemArray[2]);
                    project.EndDate = Convert.ToDateTime(row.ItemArray[3]);
                    project.Manager = row.ItemArray[4].ToString();
                    project.Status = row.ItemArray[6].ToString();
                    project.Comment = row.ItemArray[5].ToString();
                }
            }
            {
                Border border = new Border();
                border.Margin = new Thickness(10);
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = Application.Current.TryFindResource("AccentBrush") as Brush;

                DataGrid dataGrid = new DataGrid();

                Binding myBinding = new Binding("ItemsSource");
                myBinding.Source = Projects;

                dataGrid.SetBinding(DataGrid.ItemsSourceProperty, myBinding);

                border.Child = dataGrid;
                Main.Children.Add(border);
            }
            {

            }
        }
        public class Project
        {
            public string Type { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Manager { get; set; }
            public string Status { get; set; }
            public string Comment { get; set; }
        }
    }
}
