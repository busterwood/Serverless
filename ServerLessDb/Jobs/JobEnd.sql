CREATE TABLE [dbo].[JobEnd]
(
	[JobId] UNIQUEIDENTIFIER NOT NULL, 	
	[Attempt] INT NOT NULL, 
    [EndAt] DATETIMEOFFSET NOT NULL, 
    [ExitCode] int NOT NULL DEFAULT 0, 
    [Output] NVARCHAR(MAX) NOT NULL DEFAULT '', 
	[Logging] NVARCHAR(MAX) NOT NULL DEFAULT '', 
	[Exception] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_JobEnd] PRIMARY KEY ([JobId], [Attempt]), 
	CONSTRAINT [FK_JobEnd_JobStart] FOREIGN KEY ([JobId], [Attempt]) REFERENCES [JobStart]([JobId], [Attempt]), 
)