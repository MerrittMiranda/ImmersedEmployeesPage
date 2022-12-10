using Sabio.Models.Requests;
using Sabio.Models.Requests.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.InviteMembers
{
    public class UserEmployeeAddRequest
    {
        public UserAddRequest User { get; set; }

        public EmployeeAddRequest Employee { get; set; }
    }
}
