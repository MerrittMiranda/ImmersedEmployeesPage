using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.InviteMembers
{
    public class InviteMembersAddRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int OrganizationId { get; set; }
    }
}
