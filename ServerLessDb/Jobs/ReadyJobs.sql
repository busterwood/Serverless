CREATE VIEW [dbo].[ReadyJobs] WITH SCHEMABINDING AS
SELECT j.JobId, j.JobTypeId, j.AssemblySeq, j.JobAdded, j.MaxAttempts
FROM dbo.[job] j
join dbo.JobType t on j.JobTypeId = t.JobTypeId
WHERE 
	-- the job is not currently not running
	NOT EXISTS (select 1 from dbo.RunningJobs r where r.JobId = j.JobId)
    -- all dependent jobs have sucessfully finished
	AND NOT EXISTS (
		select 1 
		from dbo.JobInputDependency i
		where i.JobId = j.JobId
		and not exists (select 1 from dbo.FinishedJobs f where f.JobId = i.InputJobId)
	)
	-- max retries limit not reached	
	and isnull((select max(attempt) from dbo.JobStart s where s.JobId = j.JobId), 0) < t.MaxAttempts
