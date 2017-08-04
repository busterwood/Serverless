CREATE VIEW [dbo].[EnabledHostJobTypes] AS
SELECT hc.HostName, hc.JobTypeId, hc.HostCapacity
FROM [HostJobType] hc
WHERE hc.HostName in (select e.HostName from EnabledHosts e)
AND hc.JobTypeId in (select e.JobTypeId from EnabledJobTypes e)
