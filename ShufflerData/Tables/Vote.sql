CREATE TABLE [dbo].[Vote]
(
	[PollId] INT NOT NULL, 
    [EncryptedBallot] NVARCHAR(2048) NOT NULL, 
    [AdminSignature] NVARCHAR(2048) NOT NULL, 
    [EncryptionKey] NVARCHAR(2048) NULL,
    [IsSubmitted] BIT NOT NULL DEFAULT 0
)
