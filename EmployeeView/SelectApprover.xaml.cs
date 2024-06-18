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
            string query = "SELECT `employees`.`id`, `employees`.`full_name`, `employees`.`subdivision` FROM `employees` JOIN `credentials` ON `employees`.`id` = `credentials`.`employee_id` WHERE `credentials`.`access_level` = 'Pmanagement' OR `credentials`.`access_level` = 'HRmanagement';";
            var result = MainWindow.DBQuery(query);
            if (result.Item1)
            {
                MessageBox.Show("Something went wrong.");
                return;
            }
            foreach (DataRow row in result.Item2.Rows)
            {
                var approver = new ComboBoxItem {Content = $"{row["full_name"]} | {row["subdivision"]}"};
                approver.Tag = row["id"];
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
