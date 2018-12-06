-- delete from credittransactions
-- delete from timepunches
-- delete from volunteers
-- delete from volunteerstemp
-- drop table volunteerstemp

SELECT
	CONVERT(int, id) as Id
	,CONVERT(int, oldid) as OldId
	,nameFirst as FirstName
	,nameLast as LastName
	,phoneMain as MainPhone
	,phoneAlt as AlternatePhone
	,phoneEmerg as EmergencyPhone
	,emailAddress as EmailAddress
	,CONVERT(datetime, timeCreated) as CreateDateTime
	,address1 as StreetAddress1
	,address2 as StreetAddress2
	,city as City
	,state as State
	,zip as ZipCode
INTO VolunteersTemp
FROM jos_cbodb_members members
WHERE members.id is not null
	and members.oldid is not null
	and members.timeCreated not in ('NULL', '0000-00-00 00:00:00')
	and members.id not in(5598, 5635, 5723)


INSERT INTO Volunteers
	(
		[SystemRoleId]
		,OldSystemId
		,[FirstName]
		,[LastName]
		,[FullName]
		,[EmailAddress]
		,[PhoneNumber]
		,[EmergencyContactNumber]
		,[CreateDateTime]
		,[SecurityWordPhrase]
		,[Active]
	)
SELECT
	'7F307BC1-6015-4F77-AB9F-162AF6451653' as SystemRoleId
	,voltemp.ID as OldSystemId
	,voltemp.FirstName
	,voltemp.LastName
	,voltemp.FirstName + ' ' + voltemp.LastName as FullName
	,voltemp.EmailAddress
	,ISNULL(voltemp.MainPhone, voltemp.AlternatePhone) as PhoneNumber
	,voltemp.EmergencyPhone as EmergencyContactNumber
	,CreateDateTime
	,'' as SecurityWordPhrase
	,'1' as Active
FROM VolunteersTemp voltemp
WHERE voltemp.CreateDateTime >= '1/1/2018'


select * from Volunteers

select * from jos_cbodb_transactions 
where memberid <> null 
	and memberid = 5722
 
