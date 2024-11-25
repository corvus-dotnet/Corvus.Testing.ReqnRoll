# Corvus.Testing.ReqnRoll

![Build Status](https://github.com/endjin/Corvus.Testing.ReqnRoll/actions/workflows/build.yml/badge.svg)
[![GitHub license](https://img.shields.io/badge/License-Apache%202-blue.svg)](https://raw.githubusercontent.com/corvus-dotnet/Corvus.Testing.ReqnRoll/main/LICENSE)
[![IMM](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/total?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/total?cache=false)

This provides a library of useful testing extensions, primarily focussed on ReqnRoll operations in v4.

It is built for .NET 8.0

The ReqnRoll specific libraries contain additional bindings; to use these, you will need to add a `reqnroll.json` file to any project wishing to use them. Add entries to the `stepAssemblies` array for each of the Corvus libraries you need to use:

```json
{
  "$schema": "https://schemas.reqnroll.net/reqnroll-config-latest.json",
  "stepAssemblies": [
    { "assembly": "Corvus.Testing.ReqnRoll" },
    { "assembly": "Corvus.Testing.AzureFunctions.ReqnRoll" },
  ]
}
```

## Features

This library offers the following features.

### Container Bindings (Corvus.Testing.ReqnRoll)

`Corvus.Testing.ReqnRoll` can simplify the use of dependency injection. It can automatically create a dependency injection service collection (an instance of the `IServiceCollection` type defined by the `Microsoft.Extensions.DependencyInjection.Abstractions` component). It offers your tests an opportunity to populate this collection during the early phases of test execution. It then automatically builds an `IServiceProvider` from the service collection, and makes this accessible to your test. It disposes the provider at the end, ensuring any dependencies that need to clean up will have their `Dispose` methods called.

This feature offers two modes: you can either arrange for the service collection to be created, built, and torn down for individual scenarios, or you can create one that is created at the start of a feature, in which the service provider is shared by all scenarios within the feature, and the services are disposed only after all scenarios have run.

In most cases per-scenario mode is better, because it improves isolation between tests. However, if service initialization is particularly slow, feature-level containers might offer a useful escape hatch.

To use per-scenario mode, use the `@perScenarioContainer` tag. You can either put this on each of the individual scenarios you need, or put it at the top of the feature file, in which case every scenario in that feature will get its own container. To use per-feature mode, put a `@perFeatureContainer` tag at the top of the feature file. (You cannot use both modes at once.)

To populate the service collection you must write a binding that runs at the appropriate time. For per-scenario collections, it will look like this:

```csharp
[BeforeScenario("@perScenarioContainer", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
public static void PopulateScenarioServiceCollection(ScenarioContext scenarioContext)
{
    ContainerBindings.ConfigureServices(scenarioContext, services =>
        services.AddLogging()
            .AddSingleton<IStore, FakeStore>());
}
```

and per-feature collections work in almost exactly the same way, largely just changing "Scenario" to "Feature":

```csharp
[BeforeFeature("@perFeatureContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
public static void PopulateFeatureServiceCollection(FeatureContext featureContext)
{
    ContainerBindings.ConfigureServices(featureContext, services =>
        services.AddLogging()
            .AddSingleton<IStore, FakeStore>());
}
```

The method name doesn't matter. It's the attribute that's significant. Note that by specifying the same `@perScenarioContainer` (or `@perFeatureContainer`) tag that `Corvus.Testing.ReqnRoll` is looking for, this binding will run for any tests that specify that tag. If you want to use this container feature in multiple tests but you want to use different service configuration in different tests, you can define your own tag, e.g. your feature file might start:

```
@perScenarioContainer
@adminTestContainerInitialization
```

And then you would specify that more specialized `@adminTestContainerInitialization` tag in your bindings instead of `@perScenarioContainer`.

To obtain services from the DI container, you can write code like this:

```csharp
IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.scenarioContext);
MyService svc = this.ServiceProvider.GetRequiredService<MyService>()
```

The `ContainerBindings.GetServiceProvider` method is overloaded, accepting either a `ScenarioContext` or a `FeatureContext`. If you used the `@perScenarioContainer` tag you should pass the scenario context as the example above does, but if you're using `@perFeatureContainer` pass the feature context instead.

`GetServiceProvider` will work from inside any step, whether it's `Given`, `When`, or `Then`. But if you need to write a custom `Before...` binding that has access to the service provider, you'll need to make sure it runs at the appropriate moment. Since `Corvus.Testing.ReqnRoll` relies on this binding mechanism to create and initialize the services, you need to make sure that your code runs at the right moment, which means using the `Order` property on these ReqnRoll binding attributes. The bindings shown above that populate the `IServiceCollection` set their `Order` property with constants supplied by `Corvus.Testing.ReqnRoll`. This ensures that these bindings run after `Corvus.Testing.ReqnRoll` has created the service collection but before it has built the provider. If you want to access services from DI you will need to run after the provider has been built, which you can do by specifying a different constant for the `Order`:

```csharp
[BeforeScenario("@adminTestContainerInitialization", Order = ContainerBeforeScenarioOrder.ServiceProviderAvailable)]
public static void ServiceProviderAvailableBeforeScenario(ScenarioContext scenarioContext)
{
    IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(scenarioContext);
    MyService svc = this.ServiceProvider.GetRequiredService<MyService>()
    // etc.
}
```

(This presumes that we've got some test-specific work going on. If you wanted this binding to run any time you used a per-scenario container you'd put `@perFeatureContainer` instead of `@adminTestContainerInitialization`.)

Note that the `Order` constants that `Corvus.Testing.ReqnRoll` provides space to allow you to control the order of multiple bindings of your own if necessary. The `BuildServiceProvider` is 9999 higher than `PopulateServiceCollection`, so if you need to perform multiple bindings in between the the service collection being created, and the final service provider being built from that collection, you can do so, and you can control their ordering. (E.g., you can have one binding with `Order = ContainerBeforeScenarioOrder.PopulateServiceCollection`, and then another with `ContainerBeforeScenarioOrder.PopulateServiceCollection + 1`.)

Exceptions during container disposal are handled using the Teardown Exception Handling mechanism described below, meaning that if errors occur, they will not prevent other teardown from happening, but will still eventually be reported. (Although be aware that depending on how you run your tests, feature-level errors will not necessarily be reported as errors, due to limitations inherent in how .NET test runners integrate with build and development tools.)

### Child Object Value Retriever (Corvus.Testing.ReqnRoll)

If a scenario or feature specifies the `@useChildObjects` tag, this registers a Value Retrieved with ReqnRoll enabling named objects in the scenario context to be referred to with a `{name}` syntax. So if an earlier stage of a test puts something into the scenario context with the key `transactionId` you could write `{transactionId}` in a ReqnRoll feature fle to retrieve that value from the context and pass it in as the argument to a step.

### Teardown Exception Handling (Corvus.Testing.ReqnRoll)

If you do any non-trivial work during `After...` bindings (e.g., tearing down a function) it is often important to ensure that all the bindings run. This is problematic in cases where failures might occur at this stage, because the only way a binding can report failure is by throwing an exception, and if it does so, it will normally prevent all other bindings from running, because it causes the test to come to an abrupt halt. (ReqnRoll doesn't have a concept of "fail, but continue to clean up".) This is especially problematic for integration tests that create external resources. (E.g., if your test creates resources in Azure, it can be costly if you fail to clean these up when you're done.)

`Corvus.Testing.ReqnRoll` provides a mechanism by which you can run code in some sort of `After...` binding, and be free to throw exceptions without that halting all further cleanup, while still eventually seeing the error. You do this through a helper like this:

```csharp
[AfterScenario]
public void TeardownFunctionsAfterScenario()
{
    this.scenarioContext.RunAndStoreExceptions(this.functionsController.TeardownFunctions);
}
```

The `RunAndStoreExceptions` method here is an extension method on `ScenarioContext`. There's also one for `FeatureContext`.

This is the code in `Corvus.Testing.AzureFunctions.ReqnRoll` that tears down any functions created with the Azure Functions Launch feature described earlier. The code that performs this work is called `TeardownFunctions` and it's an instance member of the `FunctionsController` class, and it has been passed here as a delegate argument to `RunAndStoreExceptions`.

`RunAndStoreExceptions` lets you throw exceptions without risk. It catches any exceptions that emerge from your code, stores them, enables all other teardown to proceed, and then at the very last moment, it checks to see if any exceptions were detected this way, and if they were, it reports them all in one go by throwing an `AggregateException` containing every failure.

To work, `Corvus.Testing.ReqnRoll` defines unconditional `AfterScenario` and `AfterFeature` bindings each with an `Order` of `int.MaxValue`. This means they will only truly be the last thing to run for the scenario or feature if nothing else tries the same thing. If you want to use this feature, you will need to ensure that you're not using anything else that also depends on being able to be the very last thing that happens.

## Licenses

[![GitHub license](https://img.shields.io/badge/License-Apache%202-blue.svg)](https://raw.githubusercontent.com/corvus-dotnet/Corvus.Testing/main/LICENSE)

Corvus.Testing is available under the Apache 2.0 open source license.

For any licensing questions, please email [&#108;&#105;&#99;&#101;&#110;&#115;&#105;&#110;&#103;&#64;&#101;&#110;&#100;&#106;&#105;&#110;&#46;&#99;&#111;&#109;](&#109;&#97;&#105;&#108;&#116;&#111;&#58;&#108;&#105;&#99;&#101;&#110;&#115;&#105;&#110;&#103;&#64;&#101;&#110;&#100;&#106;&#105;&#110;&#46;&#99;&#111;&#109;)

## Project Sponsor

This project is sponsored by [endjin](https://endjin.com), a UK based Microsoft Partner.

For more information about our products and services, or for commercial support of this project, please [contact us](https://endjin.com/contact-us). 

We produce two free weekly newsletters; [Azure Weekly](https://azureweekly.info) for all things about the Microsoft Azure Platform, and [Power BI Weekly](https://powerbiweekly.info).

Keep up with everything that's going on at endjin via our [blog](https://endjin.com/blog), follow us on [X](https://x.com/endjin), or [LinkedIn](https://www.linkedin.com/company/endjin/).

Our other Open Source projects can be found on [GitHub](https://endjin.com/open-source)

## Code of conduct

This project has adopted a code of conduct adapted from the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. This code of conduct has been [adopted by many other projects](http://contributor-covenant.org/adopters/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [&#104;&#101;&#108;&#108;&#111;&#064;&#101;&#110;&#100;&#106;&#105;&#110;&#046;&#099;&#111;&#109;](&#109;&#097;&#105;&#108;&#116;&#111;:&#104;&#101;&#108;&#108;&#111;&#064;&#101;&#110;&#100;&#106;&#105;&#110;&#046;&#099;&#111;&#109;) with any additional questions or comments.

## IP Maturity Scorecard

The IM is endjin's IP quality framework.

[![Shared Engineering Standards](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/74e29f9b-6dca-4161-8fdd-b468a1eb185d?nocache=true)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/74e29f9b-6dca-4161-8fdd-b468a1eb185d?cache=false)

[![Coding Standards](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/f6f6490f-9493-4dc3-a674-15584fa951d8?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/f6f6490f-9493-4dc3-a674-15584fa951d8?cache=false)

[![Executable Specifications](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/bb49fb94-6ab5-40c3-a6da-dfd2e9bc4b00?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/bb49fb94-6ab5-40c3-a6da-dfd2e9bc4b00?cache=false)

[![Code Coverage](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/0449cadc-0078-4094-b019-520d75cc6cbb?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/0449cadc-0078-4094-b019-520d75cc6cbb?cache=false)

[![Benchmarks](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/64ed80dc-d354-45a9-9a56-c32437306afa?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/64ed80dc-d354-45a9-9a56-c32437306afa?cache=false)

[![Reference Documentation](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/2a7fc206-d578-41b0-85f6-a28b6b0fec5f?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/2a7fc206-d578-41b0-85f6-a28b6b0fec5f?cache=false)

[![Design & Implementation Documentation](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/f026d5a2-ce1a-4e04-af15-5a35792b164b?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/f026d5a2-ce1a-4e04-af15-5a35792b164b?cache=false)

[![How-to Documentation](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/145f2e3d-bb05-4ced-989b-7fb218fc6705?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/145f2e3d-bb05-4ced-989b-7fb218fc6705?cache=false)

[![Date of Last IP Review](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/da4ed776-0365-4d8a-a297-c4e91a14d646?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/da4ed776-0365-4d8a-a297-c4e91a14d646?cache=false)

[![Framework Version](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/6c0402b3-f0e3-4bd7-83fe-04bb6dca7924?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/6c0402b3-f0e3-4bd7-83fe-04bb6dca7924?cache=false)

[![Associated Work Items](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/79b8ff50-7378-4f29-b07c-bcd80746bfd4?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/79b8ff50-7378-4f29-b07c-bcd80746bfd4?cache=false)

[![Source Code Availability](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/30e1b40b-b27d-4631-b38d-3172426593ca?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/30e1b40b-b27d-4631-b38d-3172426593ca?cache=false)

[![License](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/d96b5bdc-62c7-47b6-bcc4-de31127c08b7?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/d96b5bdc-62c7-47b6-bcc4-de31127c08b7?cache=false)

[![Production Use](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/87ee2c3e-b17a-4939-b969-2c9c034d05d7?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/87ee2c3e-b17a-4939-b969-2c9c034d05d7?cache=false)

[![Packaging](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/547fd9f5-9caf-449f-82d9-4fba9e7ce13a?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/547fd9f5-9caf-449f-82d9-4fba9e7ce13a?cache=false)

[![OpenChain](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/66efac1a-662c-40cf-b4ec-8b34c29e9fd7?cache=false)](https://imm.endjin.com/api/imm/github/corvus-dotnet/Corvus.Testing.ReqnRoll/rule/66efac1a-662c-40cf-b4ec-8b34c29e9fd7?cache=false)