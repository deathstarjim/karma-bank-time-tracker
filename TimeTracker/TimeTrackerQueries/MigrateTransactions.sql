select 
	--vols.VolunteerId,
	voltemp.FirstName
	,voltemp.LastName
	,trans.dateOpen as PunchInDateTime
	,trans.dateClosed as PunchOutDateTime
	,trans.totalTime as TotalTimeInSeconds 
	,trans.credits as CreditsEarned
	,trans.comment as TransactionNotes
	,transtypes.Name as TransactionType
from jos_cbodb_transactions trans
	left outer join volunteerstemp voltemp
		on trans.memberID = voltemp.Id
	left outer join jos_cbodb_transaction_types transtypes
		on trans.type = transtypes.ID
	--inner join volunteers vols
	--	on vols.OldSystemId = voltemp.id
where comment <> 'SYSTEM MESSAGE: Task left open on logout'
	and trans.memberID is not null
	and trans.memberID <> 'NULL'
	and trans.credits = 'null'
order by trans.dateClosed desc
