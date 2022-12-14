USE [Immersed]
GO
/****** Object:  Table [dbo].[InviteMembers]    Script Date: 12/10/2022 1:11:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InviteMembers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](200) NOT NULL,
	[UserRoleTypeId] [int] NOT NULL,
	[OrganizationId] [int] NOT NULL,
	[TokenTypeId] [int] NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[ExpirationDate] [datetime2](7) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NOT NULL,
 CONSTRAINT [PK_InviteMembers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[InviteMembers] ADD  CONSTRAINT [DF_InviteMembers_UserRoleTypeId]  DEFAULT ((10)) FOR [UserRoleTypeId]
GO
ALTER TABLE [dbo].[InviteMembers] ADD  CONSTRAINT [DF_InviteMembers_ExpirationDate]  DEFAULT (getutcdate()+(30)) FOR [ExpirationDate]
GO
ALTER TABLE [dbo].[InviteMembers] ADD  CONSTRAINT [DF_InviteMembers_CreatedDate]  DEFAULT (getutcdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[InviteMembers]  WITH CHECK ADD  CONSTRAINT [FK_InviteMembers_OrganizationId_on_Organizations_Id] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[InviteMembers] CHECK CONSTRAINT [FK_InviteMembers_OrganizationId_on_Organizations_Id]
GO
ALTER TABLE [dbo].[InviteMembers]  WITH CHECK ADD  CONSTRAINT [FK_InviteMembers_TokenTypes_on_TokenTypes_Id] FOREIGN KEY([TokenTypeId])
REFERENCES [dbo].[TokenTypes] ([Id])
GO
ALTER TABLE [dbo].[InviteMembers] CHECK CONSTRAINT [FK_InviteMembers_TokenTypes_on_TokenTypes_Id]
GO
ALTER TABLE [dbo].[InviteMembers]  WITH CHECK ADD  CONSTRAINT [FK_InviteMembers_UserRoleTypeId_On_Roles_Id] FOREIGN KEY([UserRoleTypeId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[InviteMembers] CHECK CONSTRAINT [FK_InviteMembers_UserRoleTypeId_On_Roles_Id]
GO
