using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Employees
{
    public class EmployeeAddRequest : EmployeeBaseAddRequest
    {

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int OrganizationId { get; set; }
    }
}
