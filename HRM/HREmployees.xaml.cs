using Junior_CRM_Developer_Test.EmployeeView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Junior_CRM_Developer_Test.HRM
{
    public partial class HREmployees : Window
    {
        public static ObservableCollection<Employee> Employees = new ObservableCollection<Employee>();
        public static List<(int, string)> NameList = new();
        public HREmployees()
        {
            InitializeComponent();
            Employees.Clear();
            NameList.Clear();

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
        private void ModifyEmployee(object sender, RoutedEventArgs e)
        {
            if(EmployeesDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select a record to modify.");
                return;
            }

            EmployeeData window = new EmployeeData(EmployeesDataGrid.SelectedItem as Employee);
            window.ShowDialog();

            if(EmployeeData.ErrorHappened)
            {
                return;
            }
            else
            {
                int i = 0;
                foreach(Employee emp in Employees)
                {
                    if(emp._Id == EmployeeData.ModifiedEmployee._Id)
                    {
                        break; 
                    }
                    i++;
                }
                Employees[i]._Partner = EmployeeData.ModifiedEmployee._Partner;
                Employees[i]._Position = EmployeeData.ModifiedEmployee._Position;
                Employees[i]._Name = EmployeeData.ModifiedEmployee._Name;
                Employees[i]._Subdivision = EmployeeData.ModifiedEmployee._Subdivision;
            }

            EmployeesDataGrid.ItemsSource = null;
            EmployeesDataGrid.ItemsSource = Employees;
            EmployeesDataGrid.UpdateLayout();
        }
        private void DeactivateEmployee(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedCells.Count == 0)
            {
                return;
            }
            Employee selectedEmployee = EmployeesDataGrid.SelectedCells[0].Item as Employee;
            if(selectedEmployee._Status == "Inactive")
            {
                return;
            }
            int id = Employees.IndexOf(selectedEmployee);
            if (id == -1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
                return;
            }

            string query = $"UPDATE `employees` SET `status`='Inactive' WHERE `id` = {selectedEmployee._Id};";
            var result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                MessageBox.Show("Something went wrong during database update.\nPlease contact app administator.");
                return;
            }

            Employees[id]._Status = "Inactive";
            EmployeesDataGrid.ItemsSource = null;
            EmployeesDataGrid.ItemsSource = Employees;
            EmployeesDataGrid.UpdateLayout();
            MessageBox.Show("Employee marked as 'Inactive' correctly.");
        }
        private void ActivateEmployee(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedCells.Count == 0)
            {
                return;
            }

            Employee selectedEmployee = EmployeesDataGrid.SelectedCells[0].Item as Employee;
            if (selectedEmployee._Status == "Active")
            {
                return;
            }
            int id = Employees.IndexOf(selectedEmployee);
            if (id == -1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
                return;
            }

            string query = $"UPDATE `employees` SET `status`='Active' WHERE `id` = {selectedEmployee._Id};";
            var result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                MessageBox.Show("Something went wrong during database update.\nPlease contact app administator.");
                return;
            }

            Employees[id]._Status = "Active";
            EmployeesDataGrid.ItemsSource = null;
            EmployeesDataGrid.ItemsSource = Employees;
            EmployeesDataGrid.UpdateLayout();
            MessageBox.Show("Employee marked as 'Active' correctly.");
        }
    }
}
