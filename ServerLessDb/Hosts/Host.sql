CREATE TABLE [dbo].[Host] -- a server that can run jobs
(
	[HostName] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [HostAdded] DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
)
