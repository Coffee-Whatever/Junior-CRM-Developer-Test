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

namespace Junior_CRM_Developer_Test.HRM
{
    public partial class HRMenu : Window
    {
        private int UserId { get; set; }
        public HRMenu()
        {
            InitializeComponent();
        }
        public HRMenu(int id)
        {
            InitializeComponent();
            UserId = id;
        }
        public void OpenEmployeeWindow(object sender, EventArgs e)
        {
            HREmployees window = new HREmployees();
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
            HRProjects window = new HRProjects();
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
            HRLeaveRequests window = new HRLeaveRequests(UserId);
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
            HRApprovalRequests window = new HRApprovalRequests(UserId);
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
