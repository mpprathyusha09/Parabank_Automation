Feature: User Login
  As a registered customer
  I want to login to my account
  So that I can access my banking services

  Background:
    Given I open the application

  Scenario Outline: Login with various credentials
    When I enter username as "<username>"
    And I enter password as "<password>"
    And I click the login button
    Then the login result should be "<result>"

    Examples:
      | username          | password      | result           | description                      |
      | validuser         | ValidPass1    | success          | Valid credentials               |
      | validuser         | InvalidPass   | failure          | Invalid password                |
      | invaliduser       | ValidPass1    | failure          | Invalid username                |
      | invaliduser       | InvalidPass   | failure          | Both invalid credentials        |
      |                   | ValidPass1    | failure          | Missing username                |
      | validuser         |               | failure          | Missing password                |
      |                   |               | failure          | Both username and password empty|
      | user@example.com  | P@ssw0rd123   | failure          | Non-existent user               |
      | admin             | admin123      | failure          | Common default credentials      |
      | testuser          | password      | failure          | Weak password attempt           |
      | ' or '1'='1       | ' or '1'='1   | failure          | SQL injection attempt           |
      | <script>alert</s  | test          | failure          | XSS injection attempt           |

  Scenario: Login with valid credentials and verify dashboard
    When I enter username as "validuser"
    And I enter password as "ValidPass1"
    And I click the login button
    Then the login result should be "success"
    And the dashboard should be displayed
    And the account services menu should be visible
    And the logout link should be present

  Scenario: Login failure displays error message
    When I enter username as "invaliduser"
    And I enter password as "InvalidPass"
    And I click the login button
    Then the login result should be "failure"
    And an error message should be displayed
    And the login form should remain visible

  Scenario: Login timeout after multiple failed attempts
    When I attempt login with invalid credentials 3 times
    Then the system should display a lockout message
    And the login form should be temporarily disabled

  Scenario: Login with special characters in password
    When I enter username as "validuser"
    And I enter password as "P@ss!#$%^&*()"
    And I click the login button
    Then the login result should be "failure"

  Scenario: Logout and re-login
    Given I am logged in as "validuser"
    When I click the logout link
    Then I should be logged out
    And the login page should be displayed
    When I enter username as "validuser"
    And I enter password as "ValidPass1"
    And I click the login button
    Then the login result should be "success"
