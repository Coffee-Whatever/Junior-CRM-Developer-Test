using Junior_CRM_Developer_Test.EmployeeView;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace Junior_CRM_Developer_Test.ManagerShared
{
    public partial class ApprovalRequests : Window
    {
        public static ObservableCollection<ApprovalRequest> RequestsCollection = new ObservableCollection<ApprovalRequest>();
        public static int UserId;
        public ApprovalRequests()
        {
            InitializeComponent();
            RequestsCollection.Clear();
        }
        
        public ApprovalRequests(int id)
        {
            UserId = id;
            InitializeComponent();
            RequestsCollection.Clear();

            string query = "SELECT approvalrequest.id as 'ApproveId', `leave_request` AS 'LeaveId', approvalrequest.`status` AS 'ApprovalStatus', approvalrequest.`comment`, absence_reason," +
                " (SELECT full_name FROM employees WHERE id = employee) AS 'Name', start_date, end_date" +
                " FROM `approvalrequest` JOIN leaverequest ON approvalrequest.leave_request = leaverequest.id" +
                $" WHERE approver = {UserId};";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                this.Close();
                return;
            }

            foreach (DataRow row in result.Item2.Rows)
            {
                ApprovalRequest approvalRequest = new ApprovalRequest();
                approvalRequest._ApproveId = Convert.ToInt32(row["ApproveId"]);
                approvalRequest._LeaveId = Convert.ToInt32(row["LeaveId"]);
                approvalRequest._ApproveStatus = row["ApprovalStatus"].ToString();
                approvalRequest._Comment = row["comment"].ToString();
                approvalRequest._AbscenceReason = row["absence_reason"].ToString();
                approvalRequest._EmployeeName = row["Name"].ToString();
                approvalRequest._StartDate = Convert.ToDateTime(row["start_date"]);
                approvalRequest._EndDate = Convert.ToDateTime(row["end_date"]);

                RequestsCollection.Add(approvalRequest);
            }

            ARDataGrid.ItemsSource = RequestsCollection;
            ARDataGrid.UpdateLayout();
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            var selected = ARDataGrid.SelectedItem as ApprovalRequest;
            if (selected == null) return;
            if (selected._ApproveStatus == "Accepted") return;

            CommentEntry commentEntry = new CommentEntry();
            commentEntry.Closing += (object sender, CancelEventArgs e) =>
            {
                this.Activate();
            };
            commentEntry.ShowDialog();

            string comm = CommentEntry.CommentText;
            if (comm == null)
            {
                return;
            }
            string query = $"START TRANSACTION;SET autocommit=0;";
            var result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                query = "ROLLBACK;SET autocommit=1;";
                MainWindow.DBQuery(query);
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                return;
            }
            query = $"UPDATE `leaverequest` SET `status` = 'Accepted' WHERE `leaverequest`.`id` = {selected._LeaveId}";
            result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                query = "ROLLBACK;SET autocommit=1;";
                MainWindow.DBQuery(query);
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                return;
            }
            int id = RequestsCollection.IndexOf(selected);

            query = $"UPDATE `approvalrequest` SET `status`='Accepted' WHERE `approvalrequest`.`id` = {RequestsCollection[id]._ApproveId};";
            result = MainWindow.DBQuery(query);

            int leaveId = RequestsCollection[id]._LeaveId;
            if (result.Item1)
            {
                query = "ROLLBACK;SET autocommit=1;";
                MainWindow.DBQuery(query);
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                return;
            }
            query = $"SELECT `employee` FROM `leaverequest` WHERE `id` = {leaveId};";
            result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                query = "ROLLBACK;SET autocommit=1;";
                MainWindow.DBQuery(query);
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                return;
            }

            int employee = Convert.ToInt32(result.Item2.Rows[0]["employee"]);
            query = $"SELECT `3ob` FROM `employees` WHERE `id` = {employee};";
            result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                query = "ROLLBACK;SET autocommit=1;";
                MainWindow.DBQuery(query);
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                return;
            }

            int OutOfOfficeBalance = Convert.ToInt32(result.Item2.Rows[0]["3ob"]);
            OutOfOfficeBalance -= countWeekDays(RequestsCollection[id]._StartDate, RequestsCollection[id]._EndDate);

            query = $"UPDATE `employees` SET `3ob`={OutOfOfficeBalance} WHERE `id` = {employee};";
            result = MainWindow.DBQuery(query);

            if (result.Item1)
            {
                query = "ROLLBACK;SET autocommit=1;";
                MainWindow.DBQuery(query);
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                return;
            }
            //everything worked
            //can commit transaction
            query = "COMMIT;SET autocommit=1;";
            result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                query = "ROLLBACK;SET autocommit=1;";
                MainWindow.DBQuery(query);
                MessageBox.Show("Something went wrong during data commit, please try again later.\nPlease contact the app administrator if the issue persists.");
                return;
            }
            RequestsCollection[id]._ApproveStatus = "Accepted";
            RequestsCollection[id]._Comment = comm;

            ARDataGrid.ItemsSource = null;
            ARDataGrid.ItemsSource = RequestsCollection;
            ARDataGrid.UpdateLayout();
        }

        private void Reject(object sender, RoutedEventArgs e)
        {
            var selected = ARDataGrid.SelectedItem as ApprovalRequest;
            if (selected == null) return;
            if (selected._ApproveStatus == "Rejected") return;

            CommentEntry commentEntry = new CommentEntry();
            commentEntry.Closing += (object sender, CancelEventArgs e) =>
            {
                this.Activate();
            };

            string comm = CommentEntry.CommentText;
            if (comm == null)
            {
                return;
            }

            string query = $"UPDATE `leaverequest` SET `status` = 'Rejected', `comment` = '{comm}'  WHERE `leaverequest`.`id` = {selected._LeaveId}";
            var result = MainWindow.DBQuery(query);
            
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong, please try again later.\nPlease contact the app administrator if the issue persists.");
                return;
            }

            int id = RequestsCollection.IndexOf(selected);

            RequestsCollection[id]._ApproveStatus = "Rejected";
            RequestsCollection[id]._Comment = comm;
            ARDataGrid.ItemsSource = null;
            ARDataGrid.ItemsSource = RequestsCollection;
            ARDataGrid.UpdateLayout();
        }
        public int countWeekDays(DateTime d0, DateTime d1)
        {
            int ndays = 1 + Convert.ToInt32((d1 - d0).TotalDays);
            int nsaturdays = (ndays + Convert.ToInt32(d0.DayOfWeek)) / 7;
            return ndays - 2 * nsaturdays
                   - (d0.DayOfWeek == DayOfWeek.Sunday ? 1 : 0)
                   + (d1.DayOfWeek == DayOfWeek.Saturday ? 1 : 0);
        }
    }
}
