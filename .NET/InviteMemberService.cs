using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain.InviteMembers;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class InviteMemberService : IInviteMemberService
    {

        IDataProvider _data = null;

        public InviteMemberService(IDataProvider data)
        {
            _data = data;
        }

        public InviteMember GetByToken(string token)
        {
            string procName = "[dbo].[InviteMembers_Select_ByToken]";

            InviteMember member = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Token", token);
            },
            delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                member = MapSingleMember(reader, ref startingIndex);
            });

            return member;
        }

        private static InviteMember MapSingleMember(IDataReader reader, ref int startingIndex)
        {
            InviteMember member = new InviteMember();

            member.Id = reader.GetSafeInt32(startingIndex++);
            member.FirstName = reader.GetSafeString(startingIndex++);
            member.LastName = reader.GetSafeString(startingIndex++);
            member.Email = reader.GetSafeString(startingIndex++);
            member.UserRoleTypeId = reader.GetSafeInt32(startingIndex++);
            member.OrganizationId = reader.GetSafeInt32(startingIndex++);

            return member;
        }
    }
}
