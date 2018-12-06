DECLARE @StartDate		datetime
DECLARE @EndDate		datetime

SET @StartDate = '10/1/2018'
SET @EndDate = '11/15/2018'

-- Base Credits Earned Report
SELECT
	vols.FullName as VolunteerName
	,volopps.OpportunityName
	,punches.PunchInDateTime
	,punches.PunchOutDateTime
	,ROUND(CONVERT(decimal(18, 2), punchSummary.TotalSeconds) / 60 / 60, 2) as TotalTimeInHours
	,creditSummary.TotalCredits
FROM Volunteers vols
	LEFT OUTER JOIN TimePunches punches
		ON vols.VolunteerId = punches.VolunteerId
	INNER JOIN VolunteerOpportunities volopps
		ON volopps.VolunteerOpportunityId = punches.VolunteerOpportunityId
	LEFT OUTER JOIN (
		SELECT
			punches.VolunteerId
			,punches.VolunteerOpportunityId
			,SUM(punches.TimeInSeconds) as TotalSeconds
		FROM TimePunches punches
		WHERE punches.PunchOutDateTime IS NOT NULL
			AND punches.PunchOutDateTime BETWEEN @StartDate AND @EndDate
		GROUP BY punches.VolunteerOpportunityId, punches.VolunteerId
	) as punchSummary
		ON vols.VolunteerId = punchSummary.VolunteerId
	LEFT OUTER JOIN (
		SELECT
			punches.VolunteerId
			,SUM(punches.CreditsEarned) as TotalCredits
		FROM TimePunches punches
		WHERE CreateDateTime BETWEEN @StartDate AND @EndDate
		GROUP BY punches.VolunteerId
	) as creditSummary
		ON vols.VolunteerId = creditSummary.VolunteerId
WHERE punches.PunchOutDateTime IS NOT NULL
	AND punches.PunchOutDateTime BETWEEN @StartDate AND @EndDate
ORDER BY vols.FullName ASC, punches.PunchOutDateTime DESC
