using System.ComponentModel.DataAnnotations;
using System;

namespace Sabio.Models.Requests.Employees
{
    public class EmployeeBaseAddRequest
    {
        [StringLength(20)]
        [Phone]
        public string Phone { get; set; }

        public DateTime StartDate { get; set; }
    }
}