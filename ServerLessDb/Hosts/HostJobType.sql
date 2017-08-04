CREATE TABLE [dbo].[HostJobType]
(
	[HostName] NVARCHAR(50) NOT NULL , 
	[JobTypeId] INT NOT NULL, -- the host can run this type of job
	[HostCapacity] INT NOT NULL, -- number of concurrent jobs of this type that this host can run 
    PRIMARY KEY ([HostName], [JobTypeId]), 
    CONSTRAINT [FK_HostJobType_Host] FOREIGN KEY ([HostName]) REFERENCES [Host]([HostName]),
	CONSTRAINT [FK_HostJobType_JobType] FOREIGN KEY ([JobTypeId]) REFERENCES [JobType]([JobTypeId])
)
