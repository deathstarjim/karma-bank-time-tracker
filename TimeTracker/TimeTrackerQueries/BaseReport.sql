DECLARE @StartDate		datetime
DECLARE @EndDate		datetime

SET @StartDate = '11/1/2018'
SET @EndDate = '11/15/2018'

-- Base Credits Earned Report
SELECT
	vols.FullName as VolunteerName
	,volopps.OpportunityName
	,punches.PunchInDateTime
	,punches.PunchOutDateTime
	,CONVERT(decimal(18, 2), punches.TimeInSeconds) / 60 as DurationInMinutes
	,CONVERT(decimal(18, 2), punches.TimeInSeconds) / 60 / 60 as DurationInHours
	,punches.CreditsEarned
FROM Volunteers vols
	LEFT OUTER JOIN TimePunches punches
		ON vols.VolunteerId = punches.VolunteerId
	INNER JOIN VolunteerOpportunities volopps
		ON volopps.VolunteerOpportunityId = punches.VolunteerOpportunityId
WHERE punches.PunchOutDateTime IS NOT NULL
	AND punches.PunchOutDateTime BETWEEN @StartDate AND @EndDate

-- Base Transaction Report
SELECT
	vols.FullName as VolunteerName
	,transactions.CreateDateTime as TransactionDateTime
	,transactions.CreditAmount
	,transtypes.TransactionType
FROM Volunteers vols
	INNER JOIN CreditTransactions transactions
		ON vols.VolunteerId = transactions.VolunteerId
	INNER JOIN TransactionTypes transtypes
		ON transactions.TransactionTypeId = transtypes.TransactionTypeId
WHERE transactions.CreateDateTime BETWEEN @StartDate AND @EndDate