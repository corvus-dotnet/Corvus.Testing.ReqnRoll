// <copyright file="ChildObjectsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Testing.Reqnroll.Specs.ChildObjects
{
    using NUnit.Framework;
    using global::Reqnroll;

    [Binding]
    public class ChildObjectsSteps
    {
        private readonly ScenarioContext scenarioContext;
        private Team? team;

        public ChildObjectsSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("I have stored a person called '(.*)' in the scenario context as '(.*)'")]
        public void GivenIHaveStoredAPersonInTheScenarioContext(string name, string key)
        {
            this.scenarioContext.Set(new Person(name), key);
        }

        [When("I create a team from this table")]
        public void WhenICreateATeamFromThisTable(DataTable table)
        {
            this.team = table.CreateInstance<Team>();
        }

        [Then("the team leader is the person stored as '(.*)'")]
        public void ThenTheTeamLeaderIsThePersonStoredAs(string key)
        {
            Assert.That(this.team?.Leader, Is.SameAs(this.scenarioContext.Get<Person>(key)));
        }

        [Then("the team name is '(.*)'")]
        public void ThenTheTeamNameIs(string name)
        {
            Assert.That(this.team?.Name, Is.EqualTo(name));
        }

        [Then("the team has no leader")]
        public void ThenTheTeamHasNoLeader()
        {
            Assert.That(this.team?.Leader, Is.Null);
        }

        public class Person(string name)
        {
            public string Name { get; } = name;
        }

        public class Team
        {
            public string? Name { get; set; }

            public Person? Leader { get; set; }
        }
    }
}
