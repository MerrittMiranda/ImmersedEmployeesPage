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
        [Required]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "First name should have at least 1 character.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Last name should have at least 1 character.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int OrganizationId { get; set; }
    }
}
