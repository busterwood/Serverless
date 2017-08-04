CREATE VIEW [dbo].[EnabledHostJobTypes] WITH SCHEMABINDING AS
SELECT hc.HostName, hc.JobTypeId, hc.HostCapacity
FROM dbo.[HostJobType] hc
WHERE hc.HostName in (select e.HostName from dbo.EnabledHosts e)
AND hc.JobTypeId in (select e.JobTypeId from dbo.EnabledJobTypes e)
