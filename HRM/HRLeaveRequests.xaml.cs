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
    public partial class HRLeaveRequests : Window
    {
        public static ObservableCollection<LeaveRequest> LeaveRequestsHR = new ObservableCollection<LeaveRequest>();
        public static int UserId;
        public HRLeaveRequests()
        {
            InitializeComponent();
        }
        public HRLeaveRequests(int userId)
        {
            InitializeComponent();
            UserId = userId;

            string query = $"SELECT * FROM `leaverequest`;";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Sorry, but something went wrong during your leave request gathering process.\nPlease contact the app administrtor");
            }
            else
            {
                foreach (DataRow row in result.Item2.Rows)
                {
                    //`id`, `employee`, `absence_reason`, `start_date`, `end_date`, `comment`, `status`
                    LeaveRequest request = new LeaveRequest();

                    request._Id = Convert.ToInt32(row["id"]);
                    request._Reason = row["absence_reason"].ToString();
                    request._StartDate = Convert.ToDateTime(row["start_date"]);
                    request._EndDate = Convert.ToDateTime(row["end_date"]).Date;
                    request._Comment = row["comment"].ToString();
                    request._Status = row["status"].ToString();

                    LeaveRequestsHR.Add(request);
                }
                LeaveDataGrid.ItemsSource = LeaveRequestsHR;
            }
        }
        public void CreateNewLeaveRequest(object sender, RoutedEventArgs e)
        {
            var Form = new LeaveRequestForm();
            Form.ShowActivated = true;
            Form.Owner = this;
            Form.Show();
        }
        public int indexOfId(int i)
        {
            return -1;
        }
    }
}
