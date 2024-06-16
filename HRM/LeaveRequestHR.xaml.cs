using Junior_CRM_Developer_Test.HRM;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

namespace Junior_CRM_Developer_Test
{
    public partial class LeaveRequestHR : Window
    {
        public LeaveRequestHR()
        {
            InitializeComponent();

            string query = "SHOW COLUMNS FROM leaverequest WHERE FIELD = 'absence_reason';";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("An error accrued when grabbing absence reasons from DB\nPleas contact the app administrator.");
                return;
            }
            string AR = result.Item2.Rows[0].ItemArray[1].ToString();
            //enum('Illness','Holyday','Other') 
            AR = AR.Substring(6, AR.Length - 8);
            //Illness','Holyday','Other
            string[] reasons = AR.Split("\',\'");
            foreach (string reason in reasons)
            {
                ReasonFA.Items.Add(new ComboBoxItem {Content = reason});
            }
        }
        public void Submit(object sender, RoutedEventArgs e)
        {
            var NewInstance = new LeaveRequest();
            NewInstance._Reason = (ReasonFA.SelectedValue as ComboBoxItem).Content.ToString();
            NewInstance._StartDate = StartD.SelectedDate.Value;
            NewInstance._EndDate = EndD.SelectedDate.Value;
            NewInstance._Comment = CommentField.Text;

            //Insert into DB

            string sd = $"{NewInstance._StartDate.Value.Year}-{NewInstance._StartDate.Value.Month}-{NewInstance._StartDate.Value.Day}";
            string ed = $"{NewInstance._EndDate.Value.Year}-{NewInstance._EndDate.Value.Month}-{NewInstance._EndDate.Value.Day}";
            string query = $"INSERT INTO `leaverequest` (`employee`, `absence_reason`, `start_date`, `end_date`, `comment`) VALUES ({HRLeaveRequests.UserId}, '{NewInstance._Reason}', '{sd}', '{ed}', '{NewInstance._Comment}');";
            var result = MainWindow.DBQuery(query);

            query = "SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'crmtest' AND TABLE_NAME = 'leaverequest';";
            DataTable newId = MainWindow.DBQuery(query).Item2;
            if(newId.Rows.Count == 0)
            {
                MessageBox.Show("Something went wrong, check your data and try agian.\nIf the problem persists contact app administator.");
                return;
            }
            NewInstance._Id = Convert.ToInt32(newId.Rows[0].ItemArray[0].ToString());
            HRLeaveRequests.LeaveRequestsHR.Add(NewInstance);

            if (result.Item1)
            {
                MessageBox.Show("Something went wrong, check your data and try agian.\nIf the problem persists contact app administator.");
            }
            else
            {
                MessageBox.Show("Leave request saved correctly.");
                if (this.Owner != null)
                {
                    this.Owner.Activate();
                }
                this.Close();
            }
        }
        public void Cancel(object sender, RoutedEventArgs e)
        {

            if (this.Owner != null)
            {
                this.Owner.Activate();
            }
            this.Close();
        }
    }
}
