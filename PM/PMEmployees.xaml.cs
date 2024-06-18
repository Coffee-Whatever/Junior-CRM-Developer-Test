using Junior_CRM_Developer_Test.EmployeeView;
using Junior_CRM_Developer_Test.HRM;
using Junior_CRM_Developer_Test.PM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class PMEmployees : Window
    {
        public static ObservableCollection<Employee> Employees = new ObservableCollection<Employee>();
        public static List<(int, string)> NameList = new();
        public PMEmployees()
        {
            InitializeComponent(); 
            Employees.Clear();

            var query = "SELECT * FROM `employees`;";
            var result = MainWindow.DBQuery(query);

            query = "SELECT `id`, `full_name` FROM `employees`;";
            var NameQueryResult = MainWindow.DBQuery(query);

            if (result.Item1 || NameQueryResult.Item1)
            {
                MessageBox.Show("Something went wrong. Please try agian.\nIf this issue persists please contact the app administrator.");
                this.Close();
                return;
            }

            foreach (DataRow row in NameQueryResult.Item2.Rows)
            {
                int id = Convert.ToInt32(row["id"].ToString());
                string name = row["full_name"].ToString();
                NameList.Add((id, name));
            }

            foreach (DataRow row in result.Item2.Rows)
            {
                //ROW = `id`, `full_name`, `subdivision`, `position`, `status`, `people_partner`, `3ob`
                var instance = new Employee();
                instance._Id = Convert.ToInt32(row["id"].ToString());
                instance._Name = row["full_name"].ToString();
                instance._Subdivision = row["subdivision"].ToString();
                instance._Position = row["position"].ToString();
                instance._Status = row["status"].ToString();

                int PartnerID = Convert.ToInt32(row["people_partner"].ToString());
                string PartnerName = NameList.Find(x => x.Item1 == PartnerID).Item2;
                instance._Partner = PartnerName;

                instance._OOOB = Convert.ToInt32(row["3ob"].ToString());
                Employees.Add(instance);
            }

            EmployeesDataGrid.ItemsSource = Employees;

        }
        private void AddNewEmployee(object sender, RoutedEventArgs e)
        {
            EmployeeData window = new EmployeeData();
            window.ShowDialog();

            EmployeesDataGrid.ItemsSource = null;
            EmployeesDataGrid.ItemsSource = Employees;
            EmployeesDataGrid.UpdateLayout();
        }
        private void AddToProject(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem == null) return;

            int id = (EmployeesDataGrid.SelectedItem as Employee)._Id;

            AddToProject window = new AddToProject(id);

            window.Owner = this;
            window.Closing += (object sender, CancelEventArgs e) =>
            {
                this.Show();
                this.Activate();
            };
            this.Hide();

            window.Show();
        }
    }
}
