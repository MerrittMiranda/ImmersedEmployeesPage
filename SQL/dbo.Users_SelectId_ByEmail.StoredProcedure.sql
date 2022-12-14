USE [Immersed]
GO
/****** Object:  StoredProcedure [dbo].[Users_SelectId_ByEmail]    Script Date: 12/10/2022 1:11:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: <Miranda Merritt>
-- Create date: <01/07/2022>
-- Description: <Selects User id by Email>
-- Code Reviewer: Joe Medina (PR)

-- MODIFIED BY: author
-- MODIFIED DATE: mm/dd/yyyy
-- Code Reviewer:
-- Note:
-- =============================================
CREATE PROC [dbo].[Users_SelectId_ByEmail]
	@Email nvarchar(255)
AS

/*
	DECLARE @Email nvarchar(255) = 'mirandatest@email.com'
	EXECUTE dbo.Users_SelectId_ByEmail @Email
*/

BEGIN

	SELECT [Id]
		
	  FROM [dbo].[Users]
	  WHERE Email = @Email

END


GO
