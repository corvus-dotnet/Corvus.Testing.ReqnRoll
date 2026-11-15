# These tests are a bit meta. The behaviour under test happens in After bindings, after the steps have finished, so
# most of the verification is done by the After bindings in TeardownExceptionsBindingsForTests. Those bindings check
# what was recorded, then clear it, so that the library's own rethrow binding doesn't fail the test run.

# This tag enables the feature-level After bindings that perform and verify failing feature teardown.
@runTeardownExceptionsTests

Feature: TeardownExceptions
    When an After binding fails
    As a developer
    I want the other After bindings to still run, and all the failures to be reported together at the end

Scenario: Failures in scenario teardown are stored, later teardown still runs, and the failures are rethrown together
    Given a scenario teardown binding will fail synchronously
    And a scenario teardown binding will fail asynchronously
    Then the After bindings for this scenario verify that both failures were stored and rethrown together

Scenario: Storing exceptions outside teardown is rejected
    When I call RunAndStoreExceptions on the scenario context during a step
    Then an InvalidOperationException is thrown saying that the TeardownExceptionsBindings must be registered
    When I call RunAndStoreExceptionsAsync on the feature context during a step
    Then an InvalidOperationException is thrown saying that the TeardownExceptionsBindings must be registered
