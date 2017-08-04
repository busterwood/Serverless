CREATE VIEW [dbo].[EnabledJobTypes] WITH SCHEMABINDING AS
WITH LatestState (JobTypeId, JobTypeStateSeq) as (select JobTypeId, max(JobTypeStateSeq) from dbo.JobTypeState group by JobTypeId)
SELECT jts.JobTypeId
FROM dbo.JobTypeState jts
JOIN LatestState l on l.JobTypeId = jts.JobTypeId and l.JobTypeStateSeq = jts.JobTypeStateSeq
WHERE jts.JobTypeEnabled = 1