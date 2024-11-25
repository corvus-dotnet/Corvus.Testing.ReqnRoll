﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Containers
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("PerScenarioContainer")]
    [NUnit.Framework.CategoryAttribute("perScenarioContainer")]
    [NUnit.Framework.CategoryAttribute("runPerScenarioContainerTests")]
    public partial class PerScenarioContainerFeature
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "perScenarioContainer",
                "runPerScenarioContainerTests"};
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Containers", "PerScenarioContainer", "    When I write a test scenario that needs its own DI container\r\n    As a develo" +
                "per\r\n    I want to be able to define container services in a BeforeScenario bind" +
                "ing and then use those services later in the test", global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
#line 1 "PerScenarioContainer.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public static async System.Threading.Tasks.Task FeatureSetupAsync()
        {
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public static async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(featureHint: featureInfo);
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Equals(featureInfo) == false)))
            {
                await testRunner.OnFeatureEndAsync();
            }
            if ((testRunner.FeatureContext == null))
            {
                await testRunner.OnFeatureStartAsync(featureInfo);
            }
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
            global::Reqnroll.TestRunnerManager.ReleaseTestRunner(testRunner);
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        public virtual async System.Threading.Tasks.Task FeatureBackgroundAsync()
        {
#line 18
#line hidden
#line 19
    await testRunner.GivenAsync("I have specified the perScenarioContainer tag", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Services added during PopulateServiceCollection BeforeScenario phase are in Servi" +
            "ce Provider available to tests")]
        public async System.Threading.Tasks.Task ServicesAddedDuringPopulateServiceCollectionBeforeScenarioPhaseAreInServiceProviderAvailableToTests()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Services added during PopulateServiceCollection BeforeScenario phase are in Servi" +
                    "ce Provider available to tests", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 21
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 18
await this.FeatureBackgroundAsync();
#line hidden
#line 22
    await testRunner.GivenAsync("I use scenario ContainerBindings.GetServiceProvider during a Given step", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 23
    await testRunner.WhenAsync("I use scenario ContainerBindings.GetServiceProvider during a When step", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 24
    await testRunner.ThenAsync("if I also use scenario ContainerBindings.GetServiceProvider during a Then step", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
#line 25
    await testRunner.ThenAsync("services added during the PopulateServiceCollection BeforeScenario phase should b" +
                        "e available during \'Given\' steps", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
#line 26
    await testRunner.AndAsync("services added during the PopulateServiceCollection BeforeScenario phase should b" +
                        "e available during \'When\' steps", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 27
    await testRunner.AndAsync("services added during the PopulateServiceCollection BeforeScenario phase should b" +
                        "e available during \'Then\' steps", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Service Provider is available during ServiceProviderAvailable BeforeScenario phas" +
            "e")]
        public async System.Threading.Tasks.Task ServiceProviderIsAvailableDuringServiceProviderAvailableBeforeScenarioPhase()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Service Provider is available during ServiceProviderAvailable BeforeScenario phas" +
                    "e", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 29
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 18
await this.FeatureBackgroundAsync();
#line hidden
#line 30
    await testRunner.ThenAsync("during the ServiceProviderAvailable BeforeScenario phase, services added during t" +
                        "he PopulateServiceCollection BeforeScenario phase should have been available", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
    }
}
#pragma warning restore
#endregion
