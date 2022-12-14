USE [Immersed]
GO
/****** Object:  StoredProcedure [dbo].[Employees_Search]    Script Date: 12/10/2022 1:11:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Employees_Search]
		@PageIndex int
		, @PageSize int
		, @Query nvarchar(100)
as

/*------------ TEST CODE -------------------
	
	Declare @PageIndex int = 0
			, @PageSize int = 10
			, @Query nvarchar(100) = 'miranda'

	Execute [dbo].[Employees_Search]
							@PageIndex
							, @PageSize
							, @Query

							select * from dbo.Employees
							select * from dbo.Users

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

	WHERE (us.FirstName LIKE '%' + @Query + '%' OR 
			us.LastName LIKE '%' + @Query + '%' OR 
			us.[Email] LIKE '%' + @Query + '%')
	ORDER BY emp.[Id]

	OFFSET @offset Rows
	FETCH NEXT @PageSize Rows ONLY

END
GO
