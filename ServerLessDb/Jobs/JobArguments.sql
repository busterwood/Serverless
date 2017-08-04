CREATE TABLE [dbo].[JobArguments] -- zero or more arguments to a job
(
	[JobId] UNIQUEIDENTIFIER NOT NULL , 
    [ArgSeq] INT NOT NULL, 
    [Arg] NVARCHAR(1000) NOT NULL, 
    PRIMARY KEY ([JobId], [ArgSeq]), 
    CONSTRAINT [FK_JobArguments_Job] FOREIGN KEY ([JobId]) REFERENCES [Job]([JobId]), 
)
