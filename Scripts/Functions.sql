CREATE FUNCTION [dbo].[Winner_Function] (@Local_Team tinyint, @Visitor_Team tinyint, @Local_Goals tinyint, @Visitor_Goals tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	IF @Local_Goals > @Visitor_Goals
	BEGIN
		SET @res = @Local_Team
	END
	ELSE
	BEGIN
		SET @res = @Visitor_Team
	END
	RETURN @res
END

CREATE FUNCTION [dbo].[Season_Name_Function] (@Start_Date datetime, @Finish_Date datetime)
RETURNS nvarchar(9)
AS
BEGIN
	DECLARE @var nvarchar(9)
	SET @var = CAST( YEAR(@Start_Date) AS nvarchar(4) ) + '-' + CAST( YEAR(@Finish_Date) AS nvarchar(4) )
	RETURN @var
END

CREATE FUNCTION [dbo].[Points_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = [dbo].[Regulation_Wins_Function](@ID_Team, @ID_Season)*2 + [dbo].[Overtime_Wins_Function](@ID_Team, @ID_Season)*2 +[dbo].[Shootout_Wins_Function](@ID_Team, @ID_Season)*2 + [dbo].[Overtime_Losses_Function](@ID_Team, @ID_Season) +[dbo].[Shootout_Losses_Function](@ID_Team, @ID_Season)
	RETURN @res
END

CREATE FUNCTION [dbo].[Matches_Played_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = COUNT(*) FROM [dbo].[Matches] WHERE ([Local_Team] = @ID_Team OR [Visitor_Team] = @ID_Team) AND [Season] = @ID_Season AND [Regular_Season?] = 1
	RETURN @res
END

CREATE FUNCTION [dbo].[Matches_Left_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = [N_Matches_Team] FROM [dbo].[Season] WHERE [ID_Season] = @ID_Season
	SELECT @res = @res - [dbo].[Matches_Played_Function] (@ID_Team, @ID_Season)
	RETURN @res
END

CREATE FUNCTION [dbo].[Regulation_Wins_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = COUNT(*) FROM [dbo].[Matches] WHERE [Season] = @ID_Season AND [Regular_Season?] = 1 AND [Win_Type] = 1 AND [Winner] = @ID_Team
	RETURN @res
END

CREATE FUNCTION [dbo].[Regulation_Losses_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = COUNT(*) FROM [dbo].[Matches] WHERE [Season] = @ID_Season AND [Regular_Season?] = 1 AND [Win_Type] = 1 AND [Winner] <> @ID_Team
	RETURN @res
END

CREATE FUNCTION [dbo].[Overtime_Wins_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = COUNT(*) FROM [dbo].[Matches] WHERE [Season] = @ID_Season AND [Regular_Season?] = 2 AND [Win_Type] = 1 AND [Winner] = @ID_Team
	RETURN @res
END

CREATE FUNCTION [dbo].[Overtime_Losses_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = COUNT(*) FROM [dbo].[Matches] WHERE [Season] = @ID_Season AND [Regular_Season?] = 2 AND [Win_Type] = 1 AND [Winner] <> @ID_Team
	RETURN @res
END

CREATE FUNCTION [dbo].[Shootout_Wins_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = COUNT(*) FROM [dbo].[Matches] WHERE [Season] = @ID_Season AND [Regular_Season?] = 3 AND [Win_Type] = 1 AND [Winner] = @ID_Team
	RETURN @res
END

CREATE FUNCTION [dbo].[Shootout_Losses_Function] (@ID_Team tinyint, @ID_Season tinyint)
RETURNS tinyint
AS
BEGIN
	DECLARE @res tinyint
	SELECT @res = COUNT(*) FROM [dbo].[Matches] WHERE [Season] = @ID_Season AND [Regular_Season?] = 3 AND [Win_Type] = 1 AND [Winner] <> @ID_Team
	RETURN @res
END