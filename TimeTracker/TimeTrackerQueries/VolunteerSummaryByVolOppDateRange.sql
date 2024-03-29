-- Volunteer Summary by Volunteer Opportunity by Date Range
DECLARE @StartDate		datetime
DECLARE @EndDate		datetime

SET @StartDate = '11/1/2018'
SET @EndDate = '11/15/2018'

SELECT
	vols.FullName + ' - ' + vols.[SecurityWordPhrase]  as VolunteerName
	,volopps.OpportunityName
	,SUM(CONVERT(decimal(18, 2), punches.TimeInSeconds)) / 60 / 60 as DurationInHours
	,SUM(punches.CreditsEarned) as TotalCreditsEarned
FROM Volunteers vols
LEFT OUTER JOIN TimePunches punches
	ON vols.VolunteerId = punches.VolunteerId
INNER JOIN VolunteerOpportunities volopps
		ON volopps.VolunteerOpportunityId = punches.VolunteerOpportunityId
WHERE punches.PunchOutDateTime IS NOT NULL
	AND punches.PunchOutDateTime BETWEEN @StartDate AND @EndDate
GROUP BY vols.VolunteerId, vols.FullName, volopps.OpportunityName, vols.[SecurityWordPhrase]
ORDER BY vols.FullName

-- Volunteer Summary by Volunteer Opportunity by Date Range
DECLARE @StartDate		datetime
DECLARE @EndDate		datetime

SET @StartDate = '10/1/2018'
SET @EndDate = '11/30/2018'

SELECT
	vols.FullName + ' - ' + vols.[SecurityWordPhrase]  as VolunteerName
	,SUM(CONVERT(decimal(18, 2), punches.TimeInSeconds)) / 60 / 60 as DurationInHours
	,SUM(punches.CreditsEarned) as TotalCreditsEarned
FROM Volunteers vols
LEFT OUTER JOIN TimePunches punches
	ON vols.VolunteerId = punches.VolunteerId
INNER JOIN VolunteerOpportunities volopps
		ON volopps.VolunteerOpportunityId = punches.VolunteerOpportunityId
WHERE punches.PunchOutDateTime IS NOT NULL
	AND punches.PunchOutDateTime BETWEEN @StartDate AND @EndDate
GROUP BY vols.VolunteerId, vols.FullName, vols.[SecurityWordPhrase]
ORDER BY vols.FullName