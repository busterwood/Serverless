CREATE VIEW [dbo].[EnabledHostJobTypes] WITH SCHEMABINDING AS
-- which jobs are enabled for the host
SELECT hc.HostName, hc.JobTypeId, hc.HostCapacity
FROM dbo.[HostJobType] hc
WHERE hc.HostName in (select e.HostName from dbo.EnabledHosts e) -- the host is enabled
AND hc.JobTypeId in (select e.JobTypeId from dbo.EnabledJobTypes e) -- and the job is enabled on the host
