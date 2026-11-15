// <copyright file="TeardownExceptionsBindingsForTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Testing.ReqnRoll.Specs.TeardownExceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Reqnroll;

    /// <summary>
    /// After bindings that make teardown fail on purpose, then check that <see cref="ReqnRoll.TeardownExceptions"/>
    /// and <see cref="TeardownExceptionsBindings"/> handled the failures.
    /// </summary>
    /// <remarks>
    /// Each verify binding runs just before the library's rethrow binding (Order int.MaxValue). It calls the rethrow
    /// binding itself to check the <see cref="AggregateException"/>, then removes the stored exceptions so that the
    /// real rethrow binding has nothing to throw.
    /// </remarks>
    [Binding]
    public class TeardownExceptionsBindingsForTests
    {
        internal const string FailSynchronouslyKey = "TeardownExceptionsTests.FailSynchronously";
        internal const string FailAsynchronouslyKey = "TeardownExceptionsTests.FailAsynchronously";

        private const string LaterTeardownRanKey = "TeardownExceptionsTests.LaterTeardownRan";
        private const string Tag = "runTeardownExceptionsTests";
        private const int VerifyOrder = int.MaxValue - 1;

        private readonly ScenarioContext scenarioContext;

        public TeardownExceptionsBindingsForTests(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [AfterFeature(Tag, Order = 10)]
        public static void FailFeatureTeardownSynchronously(FeatureContext featureContext)
        {
            featureContext.RunAndStoreExceptions(() => throw new InvalidOperationException("feature sync"));
        }

        [AfterFeature(Tag, Order = 20)]
        public static Task FailFeatureTeardownAsynchronously(FeatureContext featureContext)
        {
            return featureContext.RunAndStoreExceptionsAsync(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("feature async");
            });
        }

        [AfterFeature(Tag, Order = 30)]
        public static void RecordLaterFeatureTeardownRan(FeatureContext featureContext)
        {
            featureContext.Set(true, LaterTeardownRanKey);
        }

        [AfterFeature(Tag, Order = VerifyOrder)]
        public static void VerifyFeatureTeardownExceptions(FeatureContext featureContext)
        {
            Verify(
                featureContext,
                ["feature sync", "feature async"],
                () => TeardownExceptionsBindings.RethrowFeatureTeardownExceptions(featureContext),
                () => featureContext.RunAndStoreExceptions(() => { }));
        }

        [AfterScenario(Tag, Order = 10)]
        public void FailScenarioTeardownSynchronously()
        {
            if (this.scenarioContext.ContainsKey(FailSynchronouslyKey))
            {
                this.scenarioContext.RunAndStoreExceptions(() => throw new InvalidOperationException("scenario sync"));
            }
        }

        [AfterScenario(Tag, Order = 20)]
        public async Task FailScenarioTeardownAsynchronously()
        {
            if (this.scenarioContext.ContainsKey(FailAsynchronouslyKey))
            {
                await this.scenarioContext.RunAndStoreExceptionsAsync(async () =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException("scenario async");
                }).ConfigureAwait(false);
            }
        }

        [AfterScenario(Tag, Order = 30)]
        public void RecordLaterScenarioTeardownRan()
        {
            this.scenarioContext.Set(true, LaterTeardownRanKey);
        }

        [AfterScenario(Tag, Order = VerifyOrder)]
        public void VerifyScenarioTeardownExceptions()
        {
            if (this.scenarioContext.ContainsKey(FailSynchronouslyKey))
            {
                Verify(
                    this.scenarioContext,
                    ["scenario sync", "scenario async"],
                    () => TeardownExceptionsBindings.RethrowScenarioTeardownExceptions(this.scenarioContext),
                    () => this.scenarioContext.RunAndStoreExceptions(() => { }));
            }
        }

        private static void Verify(ReqnrollContext context, string[] expectedMessages, Action rethrow, Action storeAfterRethrow)
        {
            Assert.That(context.ContainsKey(LaterTeardownRanKey), Is.True, "A later After binding did not run after the failures");

            AggregateException? aggregate = Assert.Throws<AggregateException>(() => rethrow());
            Assert.That(aggregate!.InnerExceptions.Select(x => x.Message), Is.EqualTo(expectedMessages));

            // Remove the stored exceptions, so that the library's own rethrow binding doesn't fail the test run.
            string key = context.Single(kv => kv.Value is List<Exception>).Key;
            context.Remove(key);

            InvalidOperationException? tooLate = Assert.Throws<InvalidOperationException>(() => storeAfterRethrow());
            Assert.That(tooLate!.Message, Does.Contain("Order < int.MaxValue"));
        }
    }
}
