CREATE TABLE [dbo].[JobType]
(
	[JobTypeId] INT NOT NULL, 
    [JobTypeName] NVARCHAR(20) NOT NULL, 
	[MaxAttempts] int not null default 3,
    [JobTypeAdded] DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(), 
    CONSTRAINT [PK_JobType] PRIMARY KEY ([JobTypeId]) 
)
