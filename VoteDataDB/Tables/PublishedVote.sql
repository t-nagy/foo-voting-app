CREATE TABLE [dbo].[PublishedVote]
(
	[PollId] INT NOT NULL, 
    [EncryptedBallot] NVARCHAR(2048) NOT NULL, 
    [EncryptionKey] NVARCHAR(2048) NULL,
    [DecryptedBallot] INT,
    CONSTRAINT [FK_PublishedVote_Poll] FOREIGN KEY (PollId) REFERENCES [Poll]([Id])
)
