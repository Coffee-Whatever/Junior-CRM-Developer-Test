using Junior_CRM_Developer_Test.ManagerShared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Junior_CRM_Developer_Test.PM
{
    /// <summary>
    /// Interaction logic for PMMenu.xaml
    /// </summary>
    public partial class PMMenu : Window
    {
        private int UserId { get; set; }
        public PMMenu()
        {
            InitializeComponent();
        }
        public PMMenu(int id)
        {
            InitializeComponent();
            UserId = id;
        }
        public void OpenEmployeeWindow(object sender, EventArgs e)
        {
            PMEmployees window = new PMEmployees();
            window.Owner = this;
            window.Closing += (object sender, CancelEventArgs e) =>
            {
                this.Show();
                this.Activate();
            };
            this.Hide();
            window.Show();
        }
        public void OpenProjectsWindow(object sender, EventArgs e)
        {
            PMProjects window = new PMProjects();
            window.Owner = this;
            window.Closing += (object sender, CancelEventArgs e) =>
            {
                this.Show();
                this.Activate();
            };
            this.Hide();
            window.Show();
        }
        public void OpenLeaveRWindow(object sender, EventArgs e)
        {
            ManagerShared.LeaveRequestsDisplay window = new ManagerShared.LeaveRequestsDisplay();
            window.Owner = this;
            window.Closing += (object sender, CancelEventArgs e) =>
            {
                this.Show();
                this.Activate();
            };
            this.Hide();
            window.Show();
        }
        public void OpenApprovalRWindow(object sender, EventArgs e)
        {
            ManagerShared.ApprovalRequests window = new ManagerShared.ApprovalRequests(UserId);
            window.Owner = this;
            window.Closing += (object sender, CancelEventArgs e) =>
            {
                this.Show();
                this.Activate();
            };
            this.Hide();
            window.Show();
        }
    }
}
