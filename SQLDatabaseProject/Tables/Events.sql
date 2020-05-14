﻿CREATE TABLE [dbo].[Events]
(
	[ID] INT NOT NULL IDENTITY,
	[EVENTNAME] NVARCHAR(50) NOT NULL,
	[DESCRIPTION] NVARCHAR(MAX) NULL,
	[FROM] DATETIMEOFFSET NULL,
	[TO] DATETIMEOFFSET NULL,
	[ONLINE] INT NOT NULL,
	[TOWN] NVARCHAR(50) NULL,
	[PLACE] NVARCHAR(50) NULL,
	[MDESCRIPTION] NVARCHAR(MAX) NULL,
	[HOWMANY] INT NOT NULL,
    [MEETINGCORD] NVARCHAR(50) NULL, 
    [PLACECORD] NVARCHAR(50) NULL,
	[REPORTED] INT NOT NULL,
    [CREATEUID] INT NOT NULL, 
    CONSTRAINT [PK_Events] PRIMARY KEY ([ID]) 
)
