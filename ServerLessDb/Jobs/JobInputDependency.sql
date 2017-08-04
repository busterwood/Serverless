CREATE VIEW [dbo].[JobInputDependency] WITH SCHEMABINDING AS
select i.JobId, i.JobInputSeq, i.InputJobId
from dbo.JobInputs i
where i.InputJobId is not null

