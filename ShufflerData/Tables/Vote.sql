CREATE TABLE [dbo].[Vote]
(
	[PollId] INT NOT NULL, 
    [EncryptedBallot] NVARCHAR(1024) NOT NULL, 
    [AdminSignature] NVARCHAR(1024) NOT NULL, 
    [EncyptionKey] NVARCHAR(1024) NULL
)
