﻿CREATE TABLE [dbo].[JobType]
(
	[JobTypeId] INT NOT NULL, 
    [JobTypeName] NVARCHAR(20) NOT NULL, 
    [JobTypeAdded] DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(), 
    CONSTRAINT [PK_JobType] PRIMARY KEY ([JobTypeId]) 
)
