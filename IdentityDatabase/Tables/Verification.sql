CREATE TABLE [dbo].[Verification]
(
	[UserId] NVARCHAR(450) NOT NULL,
    [VerifyKey] NVARCHAR(1024) NOT NULL,
	PRIMARY KEY (UserId, VerifyKey)
)
