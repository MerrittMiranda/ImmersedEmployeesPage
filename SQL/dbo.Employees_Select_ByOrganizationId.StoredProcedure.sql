USE [Immersed]
GO
/****** Object:  StoredProcedure [dbo].[Employees_Select_ByOrganizationId]    Script Date: 12/10/2022 1:11:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author: <Miranda Merritt>
-- Create date: <10/20/2022>
-- Description: <Selects and Orders the Employees, Users, and Organizations information by Organization Id>
-- Code Reviewer: Pablo Demalde, Andrew Hoang, Joe Medina (PR)

-- MODIFIED BY: author
-- MODIFIED DATE:12/1/2020
-- Code Reviewer:
-- Note:
-- =============================================


CREATE proc [dbo].[Employees_Select_ByOrganizationId]
				@OrganizationId int
				, @PageIndex int
				, @PageSize int
as

/*--------- TEST CODE --------------

	Declare @OrganizationId int = 35;
	Declare @PageIndex int = 0;
	Declare @PageSize int = 3;

	Execute [dbo].[Employees_Select_ByOrganizationId]	
								@OrganizationId
								, @PageIndex
								, @PageSize



*/

BEGIN
	
	Declare @offset int = @PageIndex * @PageSize

	SELECT emp.[Id]
			  ,us.[Id]
			  ,us.[FirstName]
			  ,us.[LastName]
			  ,us.[Mi]
			  ,us.[Email]
			  ,emp.[Phone]
			  ,emp.[IsActive]
			  ,org.[Id]
			  ,org.[Name]
			  ,org.[LogoUrl] 
			  ,org.[BusinessPhone] 
			  ,org.[SiteUrl]
			  ,emp.[StartDate]
			  ,emp.[EndDate]
			  ,emp.[DateCreated]
			  ,emp.[DateModified]
			  ,TotalCount = COUNT(1) OVER()

	FROM [dbo].[Employees] emp INNER JOIN [dbo].[Users] us
					on emp.UserId = us.Id
					INNER JOIN [dbo].[Organizations] org
					on emp.OrganizationId = org.Id
	WHERE emp.OrganizationId = @OrganizationId
	ORDER BY emp.OrganizationId

	OFFSET @offset Rows
	FETCH NEXT @PageSize Rows ONLY

END
GO
