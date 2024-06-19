using Junior_CRM_Developer_Test.HRM;
using System;
using System.Collections.Generic;
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

namespace Junior_CRM_Developer_Test.EmployeeView
{
    public partial class CommentEntry : Window
    {
        public static string? CommentText;
        public CommentEntry()
        {
            InitializeComponent();
        }
        private void Accept(object sender, RoutedEventArgs e)
        {
            if(Comment.Text == null) {
                Comment.Text = string.Empty;
            }
            CommentText = Comment.Text;
            this.Close();
        }

        private void Reject(object sender, RoutedEventArgs e)
        {
            CommentText = null;
            this.Close();
        }
    }
}
