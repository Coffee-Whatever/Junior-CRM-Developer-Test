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
    public partial class SelectApprover : Window
    {
        public SelectApprover()
        {
            InitializeComponent();
            string query = "SELECT id, full_name, subdivision FROM `employees` WHERE position = 'HR Manager' OR id IN (SELECT DISTINCT `manager` FROM `projects`);";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong.");
                return;
            }
            foreach (DataRow row in result.Item2.Rows)
            {
                var approver = new ComboBoxItem {Content = $"{row.ItemArray[1]} | {row.ItemArray[2]}"};
                approver.Tag = row.ItemArray[0];
                Approvers.Items.Add(approver);
            }
        }
        public void Accept(object sender, RoutedEventArgs e)
        {
            int index = Approvers.SelectedIndex;
            if(index == -1)
            {
                MessageBox.Show("Something went wrong. Please try again");
                return;
            }
            int approverId = Convert.ToInt32((Approvers.Items[index] as ComboBoxItem).Tag);
            BaseUser.ApproverId = approverId;
            this.Close();
        }
    }
}
