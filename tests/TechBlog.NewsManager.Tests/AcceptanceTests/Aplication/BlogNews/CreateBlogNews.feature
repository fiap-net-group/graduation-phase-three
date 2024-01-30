Feature: CreateBlogNews

A short summary of the feature

@tag1
Scenario: With_Success
	Given I wanted to create a blog news as "JOURNALIST"	
	And with title "title" and description "description" and body "body" and enabled "true"
	And tags:
		| name |
		| test |
		| tag |
	When I send a request to create a blog news
	Then I should get a response to create create a blog news with status code 201
	And result success to create a blog news is "true"


Scenario: With_Error_Is_Not_Journalist
	Given I wanted to create a blog news as "READER"
	And with title "title" and description "description" and body "body" and enabled "true"
	And tags:
		| name |
		| test |
		| tag |
	When I send a request to create a blog news
	Then I should get a response to create create a blog news with status code 400
	And result success to create a blog news is "false"
	And message to create a blog news is "User must be a journalist"

