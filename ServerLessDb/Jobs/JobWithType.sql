CREATE VIEW [dbo].[JobDetails] WITH SCHEMABINDING AS
select j.JobId, j.JobAdded, t.JobTypeId, t.JobTypeName, jta.AssemblySeq, jta.AssemblyName
from dbo.Job j
join dbo.JobType t on t.JobTypeId = j.JobTypeId
join dbo.JobTypeAssembly jta on jta.JobTypeId = j.JobTypeId AND jta.AssemblySeq = j.AssemblySeq