Feature: CreateUser

the feature to create a new user

Scenario: With_Success
	Given I wanted to create a new user
	And with email "alan.martins@gmail.com" and password "Str0ngPsword12!" and name "Alan Martins" and blogUserType "READER"
	When I send a request to create a new user that exists is "false" and success to create is "true"
	Then I should get a response to create a new user with status code 201
	And result success is "true"

Scenario: With_Error_Invalid_Password
	Given I wanted to create a new user
	And with email "alan.martins@gmail.com" and password "123" and name "Alan Martins" and blogUserType "READER"
	When I validate request to create a new user
	Then I should get a validate error to create a new user with this message "Invalid password"
