// <copyright file="TeardownExceptionsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Testing.Reqnroll.Specs.TeardownExceptions
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using global::Reqnroll;

    [Binding]
    public class TeardownExceptionsSteps
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;
        private Exception? exception;

        public TeardownExceptionsSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        [Given("a scenario teardown binding will fail synchronously")]
        public void GivenAScenarioTeardownBindingWillFailSynchronously()
        {
            this.scenarioContext.Set(true, TeardownExceptionsBindingsForTests.FailSynchronouslyKey);
        }

        [Given("a scenario teardown binding will fail asynchronously")]
        public void GivenAScenarioTeardownBindingWillFailAsynchronously()
        {
            this.scenarioContext.Set(true, TeardownExceptionsBindingsForTests.FailAsynchronouslyKey);
        }

        [Then("the After bindings for this scenario verify that both failures were stored and rethrown together")]
        public void ThenTheAfterBindingsVerifyTheFailures()
        {
            // The verification happens in TeardownExceptionsBindingsForTests.VerifyScenarioTeardownExceptions,
            // after this step has run. This step only makes the scenario read correctly.
        }

        [When("I call RunAndStoreExceptions on the scenario context during a step")]
        public void WhenICallRunAndStoreExceptionsOnTheScenarioContextDuringAStep()
        {
            this.exception = Record(() => this.scenarioContext.RunAndStoreExceptions(() => { }));
        }

        [When("I call RunAndStoreExceptionsAsync on the feature context during a step")]
        public async Task WhenICallRunAndStoreExceptionsAsyncOnTheFeatureContextDuringAStep()
        {
            try
            {
                await this.featureContext.RunAndStoreExceptionsAsync(() => Task.CompletedTask).ConfigureAwait(false);
                this.exception = null;
            }
            catch (Exception x)
            {
                this.exception = x;
            }
        }

        [Then("an InvalidOperationException is thrown saying that the TeardownExceptionsBindings must be registered")]
        public void ThenAnInvalidOperationExceptionIsThrownSayingTheBindingsMustBeRegistered()
        {
            Assert.That(this.exception, Is.InstanceOf<InvalidOperationException>());
            Assert.That(this.exception!.Message, Does.Contain(typeof(TeardownExceptionsBindings).FullName));
        }

        private static Exception? Record(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception x)
            {
                return x;
            }
        }
    }
}
