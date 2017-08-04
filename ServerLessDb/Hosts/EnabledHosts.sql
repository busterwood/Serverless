CREATE VIEW [dbo].[EnabledHosts] AS
WITH LatestState (HostName, HostStateSeq) as (select HostName, max(HostStateSeq) from HostState group by HostName)
SELECT hs.HostName
FROM HostState hs
JOIN LatestState l on l.HostName = hs.HostName AND l.HostStateSeq = hs.HostStateSeq
WHERE hs.HostEnabled = 1
