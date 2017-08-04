CREATE VIEW [dbo].[EnabledJobTypes] AS
WITH LatestState (JobTypeId, JobTypeStateSeq) as (select JobTypeId, max(JobTypeStateSeq) from JobTypeState group by JobTypeId)
SELECT jts.JobTypeId
FROM JobTypeState jts
JOIN LatestState l on l.JobTypeId = jts.JobTypeId and l.JobTypeStateSeq = jts.JobTypeStateSeq
WHERE jts.JobTypeEnabled = 1