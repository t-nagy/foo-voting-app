CREATE PROCEDURE [dbo].[uspVote_GetPollIdsAndSubmittedState]
AS
BEGIN
	SELECT PollId, IsSubmitted FROM dbo.Vote
	GROUP BY PollId, IsSubmitted;
END;
