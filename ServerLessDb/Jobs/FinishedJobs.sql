CREATE VIEW [dbo].[FinishedJobs] WITH SCHEMABINDING AS
SELECT e.JobId 
FROM dbo.JobEnd e
WHERE e.ExitCode = 0