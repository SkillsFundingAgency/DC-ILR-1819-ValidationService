Feature: TestConnectionToLogDb
	In order to avoid silly mistakes
	As an automated test
	I want to test the connection to the Log DB by Getting Date from Server.

@SmokeTest
Scenario: Connect to DB and GetDate
	Given app config "LoggerConnectionString" from the file
	When I try and Get the Datetime Now
	