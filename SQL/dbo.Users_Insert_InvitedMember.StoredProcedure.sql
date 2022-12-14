USE [Immersed]
GO
/****** Object:  StoredProcedure [dbo].[Users_Insert_InvitedMember]    Script Date: 12/10/2022 1:11:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: <Miranda Merritt>
-- Create date: <12/2/2022>
-- Description: <Creates new User by Invitation from Employees/ Organization>
-- Code Reviewer: Joe Medina

-- MODIFIED BY: author
-- MODIFIED DATE: mm/dd/yyyy
-- Code Reviewer:
-- Note:
-- =============================================
CREATE PROC [dbo].[Users_Insert_InvitedMember]
					@Id int OUTPUT
					,@Email nvarchar(255)
					,@FirstName nvarchar(100)
					,@LastName nvarchar(100)
					,@Mi nvarchar(2)
					,@AvatarUrl varchar(255)
					,@Password varchar(100)
					,@StatusTypeId int

AS

/*
	Declare @Id int = 0
			,@Email nvarchar(255) = 'testEmail@email.com'
			,@FirstName nvarchar(100) = 'testFirstName'
			,@LastName nvarchar(100) = 'testLastName'
			,@Mi nvarchar(2) = 'MI'
			,@AvatarUrl varchar(255) = 'testAvatarUrl'
			,@Password varchar(100) = 'testPassword'
			,@StatusTypeId int = 1

	EXECUTE [dbo].[Users_Insert_InvitedMember]
										@Id OUTPUT
										,@Email
										,@FirstName
										,@LastName
										,@Mi
										,@AvatarUrl
										,@Password
										,@StatusTypeId

	SELECT *
	FROM dbo.users
	WHERE Id = @Id
*/

BEGIN
				
	INSERT INTO [dbo].[Users]
				([Email]
				,[FirstName]
				,[LastName]
				,[Mi]
				,[AvatarUrl]
				,[Password]
				,[StatusTypeId]
				,[IsConfirmed])

     VALUES (@Email
			,@FirstName
			,@LastName
			,@Mi
			,@AvatarUrl
			,@Password
			,@StatusTypeId
			,1)

	SET @Id = SCOPE_IDENTITY()
	

END
GO
