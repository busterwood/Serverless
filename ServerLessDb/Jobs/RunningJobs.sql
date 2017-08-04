CREATE VIEW [dbo].[RunningJobs] WITH SCHEMABINDING AS
SELECT s.JobId, s.Attempt, s.HostName, s.StartAt
FROM dbo.JobStart s
WHERE NOT EXISTS (SELECT 1 FROM dbo.JobEnd e where e.JobId = s.JobId and e.Attempt = s.Attempt)
