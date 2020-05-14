CREATE TABLE [dbo].[BlockedPeople]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserID] INT NOT NULL, 
    [BlockedUserID] NCHAR(10) NULL
)
