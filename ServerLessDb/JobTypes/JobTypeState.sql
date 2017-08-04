CREATE TABLE [dbo].[JobTypeState]
(
	[JobTypeId] INT NOT NULL,
	[JobTypeStateSeq] int not null identity(1, 1),
	[JobTypeEnabled] bit not null, -- turn off to stop all jobs of this type
	[JobTypeStateChanged] datetimeoffset not null default SYSDATETIMEOFFSET(), 
    CONSTRAINT [PK_JobTypeState] PRIMARY KEY ([JobTypeId], [JobTypeStateSeq]), 
	CONSTRAINT [FK_JobTypeState_JobType] FOREIGN KEY ([JobTypeId]) REFERENCES [JobType]([JobTypeId]) 
)
