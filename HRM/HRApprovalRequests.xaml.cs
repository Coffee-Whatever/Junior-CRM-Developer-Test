using System.Collections.ObjectModel;
using System.Windows;

namespace Junior_CRM_Developer_Test.HRM
{
    public partial class HRApprovalRequests : Window
    {
        public static ObservableCollection<ApprovalRequest> ApprovalRequests = new ObservableCollection<ApprovalRequest>();
        public static int UserId;
        public HRApprovalRequests()
        {
            InitializeComponent();
        }
        
        public HRApprovalRequests(int id)
        {
            UserId = id;
            InitializeComponent();

            string query = "SELECT `leave_request` AS 'LeaveId', approvalrequest.`status` AS 'ApprovalStatus', approvalrequest.`comment`, absence_reason," +
                " (SELECT full_name FROM employees WHERE id = employee) AS 'Name', start_date, end_date FROM `approvalrequest`" +
                " JOIN leaverequest ON approvalrequest.leave_request = leaverequest.id" +
                $" WHERE approver = {UserId};";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                this.Close();
                return;
            }


        }

        private void Accept(object sender, RoutedEventArgs e)
        {

        }

        private void Reject(object sender, RoutedEventArgs e)
        {

        }
    }
}
