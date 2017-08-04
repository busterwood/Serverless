CREATE TABLE [dbo].[HostState] -- is a host enabled or disabled?
(
	[HostName] NVARCHAR(50) NOT NULL , 
	[HostStateSeq] int not null identity(1, 1),
	[HostEnabled] bit not null, -- turn off to stop all jobs on this host
	[HostStateChanged] datetimeoffset not null default SYSDATETIMEOFFSET(), 
    PRIMARY KEY ([HostName], [HostStateSeq]), 
    CONSTRAINT [FK_HostState_Host] FOREIGN KEY ([HostName]) REFERENCES [Host]([HostName])
)
