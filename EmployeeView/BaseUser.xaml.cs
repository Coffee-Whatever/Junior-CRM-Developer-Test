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
        public static ObservableCollection<LeaveRequest> LeaveRequests { get; set; } = new();
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
                    foreach (DataRow row in result .Item2.Rows)
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
                    ProjectsDataGrid.ItemsSource = Projects;
                }
            }
            {
                //Leave Request Creation
                string query = $"SELECT * FROM `leaverequest` WHERE employee = {id};";
                var result  = MainWindow.DBQuery(query);
                if (result .Item1)
                {
                    MessageBox.Show("Sorry, but something went wrong during your leave request gathering process.\nPlease contact the app administrtor");
                }
                else
                {
                    foreach (DataRow row in result .Item2.Rows)
                    {
                        //`id`, `employee`, `absence_reason`, `start_date`, `end_date`, `comment`, `status`
                        LeaveRequest request = new LeaveRequest();

                        request._Id = Convert.ToInt32(row["id"]);
                        request._Reason = row["absence_reason"].ToString();
                        request._StartDate = Convert.ToDateTime(row["start_date"]);
                        request._EndDate = Convert.ToDateTime(row["end_date"]).Date;
                        request._Comment = row["comment"].ToString();
                        request._Status = row["status"].ToString();

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
            if (LeaveDataGrid.SelectedCells.Count == 0)
            {
                return;
            }
            var selectedObject = LeaveDataGrid.SelectedCells[0].Item as LeaveRequest;
            if (selectedObject._Status == "Cancelled") return;
            int id = indexOfId(selectedObject._Id);
            
            if(id == -1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
                return;
            }

            string query = $"UPDATE `leaverequest` SET `status`='Cancelled' WHERE id={selectedObject._Id};DELETE FROM `approvalrequest` WHERE `leave_request` = {selectedObject._Id};";
            var result = MainWindow.DBQuery(query);
           
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
            }
            else
            {
                LeaveRequests[id]._Status = "Cancelled";
                LeaveDataGrid.ItemsSource = null;
                LeaveDataGrid.ItemsSource = LeaveRequests;
                LeaveDataGrid.UpdateLayout();
                MessageBox.Show("Leave request marked as 'Cancelled' correctly.");
            }
        }
        public void SubmitRequest(object sender, RoutedEventArgs e)
        {
            if(LeaveDataGrid.SelectedCells.Count == 0)
            {
                return;
            }

            var selectedObject = LeaveDataGrid.SelectedCells[0].Item as LeaveRequest;
            int id = LeaveRequests.IndexOf(selectedObject);
            if (selectedObject._Status == "Submitted") return;

            if (id == -1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
                return;
            }

            
            var approverSelection = new SelectApprover();
            approverSelection.ShowActivated = true;
            approverSelection.Owner = this;
            approverSelection.ShowDialog();

            this.Activate();
            string query = $"UPDATE `leaverequest` SET `status`='Submitted' WHERE id={selectedObject._Id};INSERT INTO `approvalrequest` (`approver`, `leave_request`, `comment`) VALUES ({ApproverId},{selectedObject._Id},'{selectedObject._Comment}');";

            var result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                MessageBox.Show("Something went wrong.\nPlease contact app administator.");
            }
            else
            {
                LeaveRequests[id]._Status = "Submitted";
                LeaveDataGrid.ItemsSource = null;
                LeaveDataGrid.ItemsSource = LeaveRequests;
                LeaveDataGrid.UpdateLayout();
                MessageBox.Show("Leave request marked as 'Submitted' correctly.");
            }

        }

        public int indexOfId(int id)
        {
            for (int i = 0; i < LeaveRequests.Count; i++)
            {
                if (LeaveRequests[i]._Id == id) return i;
            }
            return -1;
        }
    }
}
