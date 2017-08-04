CREATE VIEW [dbo].[JobDetails] AS
select j.JobId, j.JobAdded, t.JobTypeId, t.JobTypeName, jta.AssemblySeq, jta.AssemblyName
from Job j
join JobType t on t.JobTypeId = j.JobTypeId
join JobTypeAssembly jta on jta.JobTypeId = j.JobTypeId AND jta.AssemblySeq = j.AssemblySeq