Feature: Login

A short summary of the feature

Scenario: With_Success
	Given I wanted to login with username "alan.martins@gmail.com" and password "Str0ngPsword12!"
	When I send the request that user exists "true" and password is correct "true"
	Then I wish return with success "true" and status code "200"

Scenario: With_Error_UserNotFound
	Given I wanted to login with username "alan.martins@gmail.com" and password "Str0ngPsword12!"
	When I send the request that user exists "false" and password is correct "true"
	Then I wish return with success "false" and status code "400"

Scenario: With_Error_PasswordIncorrect
	Given I wanted to login with username "alan.martins@gmail.com" and password "Str0ngPsword12!"
	When I send the request that user exists "true" and password is correct "false"
	Then I wish return with success "false" and status code "400"

Scenario: With_Error_InvalidCredentials
	Given I wanted to login with username "alan.martins" and password ""
	When I validate that user
	Then I wish the validator response with success "false" and message error is "Invalid password"