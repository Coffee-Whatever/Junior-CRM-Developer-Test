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
    public partial class LeaveRequestForm : Window
    {
        int index = -1;
        List<string> reasonList;
        public LeaveRequestForm()
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
            reasonList = reasons.ToList();
            foreach (string reason in reasons)
            {
                ReasonFA.Items.Add(new ComboBoxItem {Content = reason});
            }
        }
        public void LoadData(LeaveRequest values)
        {

            index = values._Id;
            //If reason isn't found, IndexOf returns -1, which in .SelectedIndex means no selected value
            ReasonFA.SelectedIndex = reasonList.IndexOf(values._Reason);
            StartD.SelectedDate = Convert.ToDateTime(values._StartDate.ToString());
            EndD.SelectedDate = Convert.ToDateTime(values._EndDate.ToString());
            CommentField.Text = values._Comment;
        }
        public void Submit(object sender, RoutedEventArgs e)
        {
            int id = (this.Owner as BaseUser).indexOfId(index);
            if (ReasonFA.SelectedItem == null) return;
            if (StartD.SelectedDate == null) return;
            if (EndD.SelectedDate == null) return;

            if (id == -1)
            {
                var NewInstance = new LeaveRequest();
                NewInstance._Reason = (ReasonFA.SelectedValue as ComboBoxItem).Content.ToString();
                NewInstance._StartDate = StartD.SelectedDate.Value;
                NewInstance._EndDate = EndD.SelectedDate.Value;
                NewInstance._Comment = CommentField.Text;

                //Insert into DB

                string sd = $"{NewInstance._StartDate.Year}-{NewInstance._StartDate.Month}-{NewInstance._StartDate.Day}";
                string ed = $"{NewInstance._EndDate.Year}-{NewInstance._EndDate.Month}-{NewInstance._EndDate.Day}";
                string query = $"INSERT INTO `leaverequest` (`employee`, `absence_reason`, `start_date`, `end_date`, `comment`) VALUES ({BaseUser.UserId}, '{NewInstance._Reason}', '{sd}', '{ed}', '{NewInstance._Comment}');";
                var result = MainWindow.DBQuery(query);

                query = "SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'crmtest' AND TABLE_NAME = 'leaverequest';";
                DataTable newId = MainWindow.DBQuery(query).Item2;
                if(newId.Rows.Count == 0)
                {
                    MessageBox.Show("Something went wrong, check your data and try agian.\nIf the problem persists contact app administator.");
                    return;
                }
                NewInstance._Id = Convert.ToInt32(newId.Rows[0].ItemArray[0].ToString()) - 1;
                BaseUser.LeaveRequests.Add(NewInstance);

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
            else
            {
                BaseUser.LeaveRequests[id]._Reason = ReasonFA.SelectedValue.ToString();
                BaseUser.LeaveRequests[id]._StartDate = StartD.SelectedDate.Value;
                BaseUser.LeaveRequests[id]._EndDate = EndD.SelectedDate.Value;
                BaseUser.LeaveRequests[id]._Comment = CommentField.Text;
                var tempCopy = BaseUser.LeaveRequests[id];
                //Update DB
                string sd = $"{tempCopy._StartDate.Year}-{tempCopy._StartDate.Month}-{tempCopy._StartDate.Day}";
                string ed = $"{tempCopy._EndDate.Year}-{tempCopy._EndDate.Month}-{tempCopy._EndDate.Day}";
                string query = $"UPDATE `leaverequest` SET `absence_reason`='{tempCopy._Reason}',`start_date`='{sd}',`end_date`='{ed}',`comment`='{tempCopy._Comment}' WHERE id = {tempCopy._Id}";
                var result = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Something went wrong, check your data and try agian.\nIf the problem persists contact app administator.");
                }
                else
                {
                    MessageBox.Show("Leave request updated correctly.");
                    if(this.Owner != null)
                    {
                        this.Owner.Activate();
                    }
                    this.Close();
                }
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
