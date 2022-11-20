using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Employees
{
    public class Employee : EmpUserBase
    {
        public int EmployeeId { get; set; }

        public BaseOrganization Organization { get; set; }  

        public string Phone { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }


    }
}
