using Sabio.Models.Domain.Organizations;
using Sabio.Models.Requests;
using Sabio.Models.Requests.InviteMembers;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface IEmailsService
    {
        public void WelcomeEmail();

        void PhishingEmail(string token, PhishingAddRequest model);

        public void SendConfirmEmail(string token, string email);
        public void SendInviteEmail(InviteMembersAddRequest model, string token, Organization organization);
        public void ContactUsEmail(ContactUsAddRequest model);

        public void SendConfirmContactUsEmail(ContactUsAddRequest model);
    }
}