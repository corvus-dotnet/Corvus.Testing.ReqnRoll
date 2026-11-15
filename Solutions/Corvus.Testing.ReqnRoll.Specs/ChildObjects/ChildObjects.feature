# This tag is the thing we're testing. It registers a value retriever that resolves {name} table values from the
# scenario context.
@useChildObjects

Feature: ChildObjects
    When I build an object from a table in a scenario
    As a developer
    I want to refer to objects that an earlier step stored in the scenario context
    So that I can build object graphs from tables

Scenario: A table value in braces is replaced with the object stored under that name
    Given I have stored a person called 'Alice' in the scenario context as 'alice'
    When I create a team from this table
        | Name | Leader  |
        | Red  | {alice} |
    Then the team leader is the person stored as 'alice'
    And the team name is 'Red'

Scenario: A table value without braces is converted as normal
    When I create a team from this table
        | Name  |
        | Green |
    Then the team name is 'Green'
    And the team has no leader
