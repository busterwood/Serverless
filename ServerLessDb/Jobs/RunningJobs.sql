CREATE VIEW [dbo].[RunningJobs] AS
SELECT s.JobId, s.Attempt, s.HostName, s.StartAt
FROM JobStart s
WHERE NOT EXISTS (SELECT * FROM JobEnd e where e.JobId = s.JobId and e.Attempt = s.Attempt)
