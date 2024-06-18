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
        public int _Id { get; set; }
        public string? _Type { get; set; }
        public DateTime _StartDate { get; set; }
        public DateTime _EndDate { get; set; }
        public string? _Manager { get; set; }
        public string? _ManagerId { get; set; }
        public string? _Status { get; set; }
        public string? _Comment { get; set; }
    }
    public partial class LeaveRequest : ObservableObject
    {
        public int _Id { get; set; }
        public string? _Reason { get; set; }
        public DateTime _StartDate { get; set; }
        public DateTime _EndDate { get; set; }
        public string? _Comment { get; set; }
        public string? _Status { get; set; }
    }
    public partial class Employee : ObservableObject
    {
        public int _Id { get; set; }
        public string? _Name { get; set; }
        public string? _Subdivision { get; set; }
        public string? _Position { get; set; }
        public string? _Status { get; set; }
        public string? _Partner { get; set; }
        public int _OOOB { get; set; }
    }
    public partial class ApprovalRequest : ObservableObject 
    {
        public int _ApproveId { get; set; }
        public int _LeaveId { get; set; }
        public string? _ApproveStatus { get; set; }
        public string? _Comment { get; set; }
        public string? _AbscenceReason { get; set; }
        public string? _EmployeeName { get; set; }
        public DateTime _StartDate { get; set; }
        public DateTime _EndDate { get; set; }
    }
}
