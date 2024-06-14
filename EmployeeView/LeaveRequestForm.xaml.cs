﻿using MySql.Data.MySqlClient;
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
            var result = DBQuery(query);
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

            index = values.Id;
            //If reason isn't found, IndexOf returns -1, which in .SelectedIndex means no selected value
            ReasonFA.SelectedIndex = reasonList.IndexOf(values._Reason);
            StartD.SelectedDate = Convert.ToDateTime(values._StartDate.Value.ToString());
            EndD.SelectedDate = Convert.ToDateTime(values._EndDate.Value.ToString());
            CommentField.Text = values._Comment;
        }
        public static (bool, DataTable) DBQuery(string query)
        {
            // (didErrorHappen, ResultIfNoError)
            try
            {
                string connetionstring = "Server=localhost;Database=crmtest;Uid=root;Convert Zero Datetime=True";
                MySqlConnection conn = new MySqlConnection(connetionstring);
                conn.Open();

                MySqlDataAdapter dtb = new MySqlDataAdapter();
                dtb.SelectCommand = new MySqlCommand(query, conn);
                DataTable dtable = new DataTable();
                dtb.Fill(dtable);
                return (false, dtable);
            }
            catch (Exception ex)
            {
                return (true, new DataTable());
            }
        }
        public void Submit(object sender, RoutedEventArgs e)
        {
            int id = (this.Owner as BaseUser).indexOfId(index);

            if (id == -1)
            {
                var NewInstance = new LeaveRequest();
                NewInstance._Reason = ReasonFA.SelectedValue.ToString();
                NewInstance._StartDate = new DateOnly(StartD.SelectedDate.Value.Year, StartD.SelectedDate.Value.Month, StartD.SelectedDate.Value.Day);
                NewInstance._EndDate = new DateOnly(EndD.SelectedDate.Value.Year, EndD.SelectedDate.Value.Month, EndD.SelectedDate.Value.Day);
                NewInstance._Comment = CommentField.Text;

                (this.Owner as BaseUser).LeaveRequests.Add(NewInstance);

                //Insert into DB
                string query = $"INSERT INTO `leaverequest` (`employee`, `absence_reason`, `start_date`, `end_date`, `comment`) VALUES ({BaseUser.UserId}, '{NewInstance._Reason}', '{NewInstance._StartDate}', '{NewInstance._EndDate}', '{NewInstance._Comment}');";
                var result = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Something went wrong, check your data and try agian.\nIf the problem persists contact app administator.");
                }
                else
                {
                    MessageBox.Show("Leave request saved correctly.");
                    this.Close();
                }
            }
            else
            {
                (this.Owner as BaseUser).LeaveRequests[id]._Reason = ReasonFA.SelectedValue.ToString();
                (this.Owner as BaseUser).LeaveRequests[id]._StartDate = new DateOnly(StartD.SelectedDate.Value.Year, StartD.SelectedDate.Value.Month, StartD.SelectedDate.Value.Day);
                (this.Owner as BaseUser).LeaveRequests[id]._EndDate = new DateOnly(EndD.SelectedDate.Value.Year, EndD.SelectedDate.Value.Month, EndD.SelectedDate.Value.Day);
                (this.Owner as BaseUser).LeaveRequests[id]._Comment = CommentField.Text;
                var tempCopy = (this.Owner as BaseUser).LeaveRequests[id];
                //Update DB
                string query = $"UPDATE `leaverequest` SET `absence_reason`='{tempCopy._Reason}',`start_date`='{tempCopy._StartDate}',`end_date`='{tempCopy._EndDate}',`comment`='{tempCopy._Comment}' WHERE 1";
                var result = MainWindow.DBQuery(query);
                if (result.Item1)
                {
                    MessageBox.Show("Something went wrong, check your data and try agian.\nIf the problem persists contact app administator.");
                }
                else
                {
                    MessageBox.Show("Leave request updated correctly.");
                    this.Close();
                }
            }
        }
        public void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
