CREATE TABLE [dbo].[JobDependencies]
(
	[JobId] UNIQUEIDENTIFIER NOT NULL , 	
	[DependsOnJobId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_JobDependencies_JobId] FOREIGN KEY ([JobId]) REFERENCES [Job]([JobId]), 
	CONSTRAINT [FK_JobDependencies_DependsOnJobId] FOREIGN KEY ([DependsOnJobId]) REFERENCES [Job]([JobId]), 
)
