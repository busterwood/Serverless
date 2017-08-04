CREATE VIEW [dbo].[EnabledHosts] WITH SCHEMABINDING AS
WITH LatestState (HostName, HostStateSeq) as (select HostName, max(HostStateSeq) from dbo.HostState group by HostName)
SELECT hs.HostName
FROM dbo.HostState hs
JOIN LatestState l on l.HostName = hs.HostName AND l.HostStateSeq = hs.HostStateSeq
WHERE hs.HostEnabled = 1
