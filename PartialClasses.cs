using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junior_CRM_Developer_Test
{
    public partial class Project : ObservableObject
    {
        public string? _Type { get; set; }
        public DateTime? _StartDate { get; set; }
        public DateTime? _EndDate { get; set; }
        public string? _Manager { get; set; }
        public string? _Status { get; set; }
        public string? _Comment { get; set; }
    }
    public partial class LeaveRequest : ObservableObject
    {
        public int Id { get; set; }
        public string? _Reason { get; set; }
        public DateTime? _StartDate { get; set; }
        public DateTime? _EndDate { get; set; }
        public string? _Comment { get; set; }
        public string? _Status { get; set; }
    }
}
