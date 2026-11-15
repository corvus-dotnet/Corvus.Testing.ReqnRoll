# Corvus.Testing.Reqnroll

Bindings that make [Reqnroll](https://reqnroll.net/) specs easier to write:

- **Container bindings:** a dependency injection container for each scenario or each feature, which is built, made available to your steps, and disposed for you.
- **Child objects:** refer to objects stored in the scenario context from Reqnroll tables, with `{name}`.
- **Teardown exception handling:** run every `After...` binding even when some fail, and report all the failures together at the end.

Requires .NET 10 and Reqnroll 3.

## Getting started

If you use NUnit or MSTest, install the metapackage for your test framework. It brings in this package along with Reqnroll, the test adapter and the test SDK:

```
dotnet add package Corvus.Testing.Reqnroll.NUnit
dotnet add package Corvus.Testing.Reqnroll.MSTest
```

Otherwise, install this package on its own:

```
dotnet add package Corvus.Testing.Reqnroll
```

Then tell Reqnroll to load the bindings, in your test project's `reqnroll.json`:

```json
{
  "$schema": "https://schemas.reqnroll.net/reqnroll-config-latest.json",
  "stepAssemblies": [
    { "assembly": "Corvus.Testing.Reqnroll" }
  ]
}
```

## Container bindings

Tag a scenario (or a whole feature) with `@perScenarioContainer`, or a feature with `@perFeatureContainer`. Then add services in a binding that runs at the right point:

```csharp
[BeforeScenario("@perScenarioContainer", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
public static void PopulateServices(ScenarioContext scenarioContext)
{
    ContainerBindings.ConfigureServices(scenarioContext, services =>
        services.AddLogging()
                .AddSingleton<IStore, FakeStore>());
}
```

Get services in any step:

```csharp
IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.scenarioContext);
IStore store = serviceProvider.GetRequiredService<IStore>();
```

For per-feature containers, use `[BeforeFeature]`, `ContainerBeforeFeatureOrder` and the `FeatureContext` instead. The service provider is disposed when the scenario or feature ends.

## Child objects

With the `@useChildObjects` tag, a table value in braces is replaced with the object stored in the scenario context under that name. This works with Reqnroll's table helpers, such as `CreateInstance` and `CreateSet`:

```gherkin
@useChildObjects
Scenario: Create a team
    Given I have stored a person as 'alice'
    When I create a team from this table
        | Name | Leader  |
        | Red  | {alice} |
```

```csharp
Team team = table.CreateInstance<Team>(); // team.Leader is the object stored as "alice"
```

## Teardown exception handling

Reqnroll stops running `After...` bindings as soon as one throws. Wrap teardown work in `RunAndStoreExceptions` (or `RunAndStoreExceptionsAsync`) so that the exception is stored and the remaining bindings still run:

```csharp
[AfterScenario]
public void DeleteTestResources()
{
    this.scenarioContext.RunAndStoreExceptions(() => this.resources.DeleteAll());
}
```

At the end of the scenario or feature, all the stored exceptions are thrown together as one `AggregateException`. There are overloads for both `ScenarioContext` and `FeatureContext`.

## More information

- [Full documentation](https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll#readme), including binding order and custom tags
- [Changelog](https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/blob/main/CHANGELOG.md)
- [Source code and issues](https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll)

Corvus.Testing.Reqnroll is maintained by [endjin](https://endjin.com) and licensed under Apache 2.0.
