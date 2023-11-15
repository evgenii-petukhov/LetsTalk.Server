INSERT INTO `accounts`
(
	`AccountTypeId`,
	`FirstName`,
	`LastName`,
	`PhotoUrl`
)
VALUES
(
	1,
	'Neil',
	'Johnston',
	'https://cdn-icons-png.flaticon.com/512/2202/2202112.png'
),
(
	2,
	'Bob',
	'Pettit',
	'https://cdn-icons-png.flaticon.com/512/4333/4333609.png '
),
(
	1,
	'Rick',
	'Barry',
	'https://cdn-icons-png.flaticon.com/512/706/706830.png'
),
(
	2,
	'George',
	'Gervin',
	'https://cdn-icons-png.flaticon.com/512/3006/3006876.png'
)




db.getCollection("Account").insertMany([{
    "AccountTypeId" : 1,
    "PhotoUrl" : 'https://cdn-icons-png.flaticon.com/512/2202/2202112.png',
    "FirstName" : "Neil",
    "LastName" : "Johnston"
},
{
    "AccountTypeId" : 1,
    "PhotoUrl" : 'https://cdn-icons-png.flaticon.com/512/4333/4333609.png',
    "FirstName" : "Bob",
    "LastName" : "Pettit"
},
{
    "AccountTypeId" : 1,
    "PhotoUrl" : 'https://cdn-icons-png.flaticon.com/512/706/706830.png',
    "FirstName" : "Rick",
    "LastName" : "Barry"
},
{
    "AccountTypeId" : 1,
    "PhotoUrl" : 'https://cdn-icons-png.flaticon.com/512/3006/3006876.png',
    "FirstName" : "George",
    "LastName" : "Gervin"
}])




db.getCollection("Account").updateMany({"ExternalId": { $exists: false }}, {"$set":{
    "ExternalId" : null,
    "Email" : null,
    "ImageId" : null
}})