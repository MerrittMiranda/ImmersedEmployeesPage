using Sabio.Models.Domain.InviteMembers;

namespace Sabio.Services.Interfaces
{
    public interface IInviteMemberService
    {
        InviteMember GetByToken(string token);
    }
}