CREATE TABLE [dbo].[JobTypeAssembly]
(
	[JobTypeId] INT NOT NULL, 
	[AssemblySeq] INT NOT NULL,
    [AssemblyName] NVARCHAR(1000) NOT NULL, 
    [AssemblyAdded] DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(), 
    CONSTRAINT [PK_JobTypeAssembly] PRIMARY KEY ([JobTypeId], [AssemblySeq]), 
    CONSTRAINT [FK_JobTypeAssembly_JobType] FOREIGN KEY ([JobTypeId]) REFERENCES [JobType]([JobTypeId]) 

)
