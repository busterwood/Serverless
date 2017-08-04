CREATE TABLE [dbo].[JobStart]
(
	[JobId] UNIQUEIDENTIFIER NOT NULL, 	
	[Attempt] SMALLINT NOT NULL, 
    [StartAt] DATETIMEOFFSET NOT NULL, 
    [HostName] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_JobAttempt] PRIMARY KEY ([JobId], [Attempt]),
	CONSTRAINT [FK_JobStart_JobId] FOREIGN KEY ([JobId]) REFERENCES [Job]([JobId]), 
    CONSTRAINT [FK_JobStart_Host] FOREIGN KEY ([HostName]) REFERENCES [Host]([HostName]),

)
