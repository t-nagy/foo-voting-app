CREATE TABLE [dbo].[Participant]
(
	[UserId] NVARCHAR(450) NOT NULL,
	[PollId] INT NOT NULL,
	[ParticipantRole] INT NOT NULL,
	[HasVoted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Participant_Election] FOREIGN KEY (PollId) REFERENCES [Poll]([Id]),
)
