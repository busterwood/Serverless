CREATE TABLE [dbo].[JobInputs]
(
	[JobId] UNIQUEIDENTIFIER NOT NULL , 	
    [JobInputSeq] INT NOT NULL DEFAULT 0,
	[InputJobId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_JobInputs] PRIMARY KEY ([JobId], [JobInputSeq]),
	CONSTRAINT [FK_JobInputs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Job]([JobId]), 
	CONSTRAINT [FK_JobInputs_InputJobId] FOREIGN KEY ([InputJobId]) REFERENCES [Job]([JobId]), 
)
