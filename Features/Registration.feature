Feature: User Registration
  As a prospective customer
  I want to register for an account
  So that I can access Parabank services

  Background:
    Given I open the application
    And I navigate to the registration page

  Scenario: Successful registration
    When I register with valid registration data
    Then the registration is successful

  Scenario: Registration fails when passwords do not match
    When I register with mismatched passwords
    Then the registration fails with a validation error

  Scenario: Registration fails when username is missing
    When I register without a username
    Then the registration fails with a validation error

  Scenario: Login with newly registered account
    When I register with valid registration data
    Then the registration is successful
    When I login with registered credentials
    Then the user is logged in
