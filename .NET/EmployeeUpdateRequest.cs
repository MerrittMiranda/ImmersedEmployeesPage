using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Employees
{
    public class EmployeeUpdateRequest : EmployeeBaseAddRequest
    {

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Id { get; set; }
    }
}
