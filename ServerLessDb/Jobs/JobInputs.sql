CREATE TABLE [dbo].[JobInputs] -- inputs to a job - either hard coded or the output of another job
(
	[JobId] UNIQUEIDENTIFIER NOT NULL , 	
    [JobInputSeq] INT NOT NULL DEFAULT 0,
	[InputJobId] UNIQUEIDENTIFIER,
	[Input] NVARCHAR(MAX),
    CONSTRAINT [PK_JobInputs] PRIMARY KEY ([JobId], [JobInputSeq]),
	CONSTRAINT [FK_JobInputs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Job]([JobId]), 
	CONSTRAINT [FK_JobInputs_InputJobId] FOREIGN KEY ([InputJobId]) REFERENCES [Job]([JobId]), 
)
