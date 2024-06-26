CREATE DATABASE crmtest;
USE crmtest;
SET GLOBAL max_allowed_packet=33554432;
CREATE TABLE Employees (
	id int AUTO_INCREMENT PRIMARY KEY NOT NULL,
	full_name VARCHAR(150) NOT NULL,
	subdivision enum('Admin', 'HR', 'Dev', 'Sales') NOT NULL,
	position enum('Administrator', 'HR Manager', 'Developer', 'Marketer') NOT NULL,
	status enum('Active','Inactive') NOT NULL DEFAULT 'Active',
	people_partner int NOT NULL,
	3ob int NOT NULL,
	picture MEDIUMBLOB,
    FOREIGN KEY (people_partner) REFERENCES Employees(id)
);
CREATE TABLE Projects (
	id int AUTO_INCREMENT PRIMARY KEY NOT NULL,
	type enum('SalesPitch', 'CompanyPR', 'AppDevelopmnet') NOT NULL,
	start_date date NOT NULL,
	end_date date,
	manager int NOT NULL,
    comment TEXT,
	status enum('Active','Inactive') NOT NULL DEFAULT 'Active',
    FOREIGN KEY (manager) REFERENCES Employees(id)
);
CREATE TABLE EmployeeProjectRelations(
	employeeId int,
	projectId int,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(id),
    FOREIGN KEY (projectId) REFERENCES Projects(id)
);

CREATE TABLE LeaveRequest (
	id int AUTO_INCREMENT PRIMARY KEY NOT NULL,
	employee int NOT NULL,
	absence_reason enum('Illness', 'Holyday', 'Other') NOT NULL,
	start_date date NOT NULL,
	end_date date NOT NULL,
    comment TEXT,
	status enum('New', 'Submitted', 'Cancelled', 'Rejected', 'Accepted') NOT NULL DEFAULT 'New',
    FOREIGN KEY (employee) REFERENCES Employees(id)
);
CREATE TABLE ApprovalRequest (
	id int AUTO_INCREMENT PRIMARY KEY NOT NULL,
	approver int NOT NULL,
	leave_request int NOT NULL,
	status enum('New', 'Rejected', 'Accepted') NOT NULL DEFAULT 'New',
    comment TEXT,
    FOREIGN KEY (approver) REFERENCES Employees(id),
    FOREIGN KEY (leave_request) REFERENCES LeaveRequest(id)
);

CREATE TABLE Credentials (
	id int AUTO_INCREMENT PRIMARY KEY NOT NULL,
	employee_id int UNIQUE,
    login varchar(150),
    password varchar(255),
    current_salt char(20),
    access_level enum('base', 'HRmanagement', 'Pmanagement', 'full') DEFAULT 'base',
	FOREIGN KEY (employee_id) REFERENCES Employees(id)
);

INSERT INTO Employees VALUES
(1, 'Patryk Klimkiewicz', 'Admin', 'Administrator', 'Active', 1, 26, ''),
(2, 'TestUser1', 'HR', 'HR Manager', 'Active', 1, 26, ''),
(3, 'TestUser2', 'Dev', 'Developer', 'Active', 2, 26, ''),
(4, 'TestUser3', 'Sales', 'Marketer', 'Active', 4, 26, ''),
(5, 'UserWithOutCredentials', 'HR', 'HR Manager', 'Active', 1, 26, '');

INSERT INTO `credentials` VALUES
(1, 1, 'admin', '0F2FD52805592D291CE45E932F85176274B003F8D1B9F297274F4326E9F40580', 'k!xkzf>odaaab<%m),o1', 'full'),
(2, 4, 'baseuser', '33A1BF8BEBDCDBE1CD6989176CA34D6375EF3B31377D3CAE2D60C363B09C1E0A', 'g;!(m,m!n1fj2ad9jfa{', 'base'),
(3, 2, 'hr', '676377AE98C151E4D691399A56BDFA47599398C1BB125B32904260BB699CD530', '7tp]df:st)sta]i0rae7', 'HRmanagement'),
(4, 3, 'pm', '97C4B45D6ACD82FFC4403989FD0D9B1B69452EAEE12D7977A235E6EA0B7DB1E0', '&d&q08#ftk:2oyt#fixb', 'Pmanagement');

INSERT INTO `projects`(`id`, `type`, `start_date`, `end_date`, `manager`, `comment`, `status`) VALUES
(1,'AppDevelopmnet', "2024-06-13", "", 1, 'TestApp','Active');

INSERT INTO `employeeprojectrelations`(`employeeId`, `projectId`)
VALUES (1,1);

INSERT INTO `leaverequest`(`employee`, `absence_reason`, `start_date`, `end_date`, `comment`) VALUES 
(4, 'Holyday','2024-12-24','2025-01-02','Newyears and Christmas leave'),
(5, 'Other','2024-12-21','2024-12-28','Other test leave');

INSERT INTO `approvalrequest` (`approver`, `leave_request`) VALUES (2, 1), (3, 2);
