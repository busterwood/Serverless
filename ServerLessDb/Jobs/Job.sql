CREATE TABLE [dbo].[Job]
(
	[JobId] UNIQUEIDENTIFIER NOT NULL, 
    [JobTypeId] INT NOT NULL, 
    [AssemblySeq] INT NOT NULL, 
    [JobAdded] DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(), 
	CONSTRAINT [PK_Job]  PRIMARY KEY ([JobId]),
    CONSTRAINT [FK_Job_JobTypeAssembly] FOREIGN KEY ([JobTypeId], [AssemblySeq]) REFERENCES [JobTypeAssembly]([JobTypeId], [AssemblySeq]) 
)