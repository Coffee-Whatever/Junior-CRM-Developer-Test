using MySql.Data.MySqlClient;
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;

namespace Junior_CRM_Developer_Test
{
    /// <summary>
    /// Interaction logic for BaseUser.xaml
    /// </summary>
    public partial class BaseUser : Window
    {
        public ObservableCollection<Project> Projects { get; set; } = new();
        public ObservableCollection<LeaveRequest> LeaveRequests { get; set; } = new();
        public static int UserId;
        public static int ApproverId;
        public BaseUser()
        {
            InitializeComponent();
        }
        public BaseUser(int id)
        {
            InitializeComponent();
            UserId = id;
            GatherData(id);
        }
        public void GatherData(int id)
        {
            {
                //Project Generation
                string query = $"SELECT type, start_date, end_date, projects.status, projects.comment, (SELECT full_name FROM employees WHERE id = projects.manager) AS 'Manager' FROM projects JOIN employeeprojectrelations ON projects.id = employeeprojectrelations.projectId JOIN employees ON employeeprojectrelations.employeeId = employees.id WHERE employees.id = {id};";
                var result  = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Sorry, but something went wrong during your project gathering process.\nPlease contact the app administrtor");
                }
                else
                {
                    DateTime temp;
                    foreach (DataRow row in result .Item2.Rows)
                    {
                        Project project = new Project();

                        project._Type = row.ItemArray[0].ToString();
                        temp = Convert.ToDateTime(row.ItemArray[1]);
                        project._StartDate = new DateOnly(temp.Year, temp.Month, temp.Day);
                        temp = Convert.ToDateTime(row.ItemArray[2]);
                        project._EndDate = new DateOnly(temp.Year, temp.Month, temp.Day);
                        project._Status = row.ItemArray[3].ToString();
                        project._Manager = row.ItemArray[5].ToString();
                        project._Comment = row.ItemArray[4].ToString();

                        Projects.Add(project);
                    }
                    ProjectsDataGrid.ItemsSource = Projects;
                }
            }
            {
                //Leave Request Creation
                //Project Generation
                string query = $"SELECT * FROM `leaverequest` WHERE employee = {id};";
                var result  = MainWindow.DBQuery(query);
                if (result .Item1)
                {
                    MessageBox.Show("Sorry, but something went wrong during your leave request gathering process.\nPlease contact the app administrtor");
                }
                else
                {
                    DateTime temp;
                    foreach (DataRow row in result .Item2.Rows)
                    {
                        LeaveRequest request = new LeaveRequest();

                        request.Id = Convert.ToInt32(row.ItemArray[0]);
                        request._Reason = row.ItemArray[2].ToString();
                        temp = Convert.ToDateTime(row.ItemArray[3]);
                        request._StartDate = new DateOnly(temp.Year, temp.Month, temp.Day);
                        temp = Convert.ToDateTime(row.ItemArray[4]).Date;
                        request._EndDate = new DateOnly(temp.Year, temp.Month, temp.Day);
                        request._Comment = row.ItemArray[5].ToString();
                        request._Status = row.ItemArray[6].ToString();

                        LeaveRequests.Add(request);
                    }
                    LeaveDataGrid.ItemsSource = LeaveRequests;
                }
            }
        }

        public void CreateNewLeaveRequest(object sender, RoutedEventArgs e)
        {
            var Form = new LeaveRequestForm();
            Form.ShowActivated = true;
            Form.Owner = this;
            Form.Show();
        }
        public void EditSelectedRequest(object sender, RoutedEventArgs e)
        {
            var selectedObject = LeaveDataGrid.SelectedCells[0].Item as LeaveRequest;
            var Form = new LeaveRequestForm();
            Form.LoadData(selectedObject);
            Form.ShowActivated = true;
            Form.Owner = this;
            Form.Show();
        }
        public void CancleRequest(object sender, RoutedEventArgs e)
        {
            var selectedObject = LeaveDataGrid.SelectedCells[0].Item as LeaveRequest;
            if (selectedObject._Status == "Cancelled") return;
            int id = indexOfId(selectedObject.Id);
            
            if(id == -1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
                return;
            }
            else
            {
                LeaveRequests[id]._Status = "Cancelled";
                LeaveDataGrid.ItemsSource = null;
                LeaveDataGrid.ItemsSource = LeaveRequests;
                LeaveDataGrid.UpdateLayout();
            }

            string query = $"UPDATE `leaverequest` SET `status`='Cancelled' WHERE id={selectedObject.Id};DELETE FROM `approvalrequest` WHERE `leave_request` = {selectedObject.Id};";
            var result = MainWindow.DBQuery(query);
           
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
            }
            else
            {
                MessageBox.Show("Leave request marked as 'Cancelled' correctly.");
            }
        }
        public void SubmitRequest(object sender, RoutedEventArgs e)
        {
            var selectedObject = LeaveDataGrid.SelectedCells[0].Item as LeaveRequest;
            int id = indexOfId(selectedObject.Id);
            if (selectedObject._Status == "Submitted") return;

            if (id == -1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
                return;
            }
            else
            {
                LeaveRequests[id]._Status = "Submitted";
                LeaveDataGrid.ItemsSource = null;
                LeaveDataGrid.ItemsSource = LeaveRequests;
                LeaveDataGrid.UpdateLayout();
            }

            
            var approverSelection = new SelectApprover();
            approverSelection.ShowActivated = true;
            approverSelection.Owner = this;
            approverSelection.ShowDialog();

            string query = $"UPDATE `leaverequest` SET `status`='Submitted' WHERE id={selectedObject.Id};INSERT INTO `approvalrequest` (`approver`, `leave_request`, `comment`) VALUES ({ApproverId},'{selectedObject.Id}','{selectedObject._Comment}');";

            var result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
            }
            else
            {
                MessageBox.Show("Leave request marked as 'Submitted' correctly.");
            }

        }

        public int indexOfId(int id)
        {
            for (int i = 0; i < LeaveRequests.Count; i++)
            {
                if (LeaveRequests[i].Id == id) return i;
            }
            return -1;
        }
    }
}
