# Corvus.Testing.Reqnroll.MSTest

A metapackage for writing [Reqnroll](https://reqnroll.net/) specs with MSTest and [Corvus.Testing.Reqnroll](https://www.nuget.org/packages/Corvus.Testing.Reqnroll). Add this one package to a test project, and update this one package to keep everything in step.

It contains no code. It references:

- [Corvus.Testing.Reqnroll](https://www.nuget.org/packages/Corvus.Testing.Reqnroll): container bindings, child objects and teardown exception handling
- [Reqnroll.MSTest](https://www.nuget.org/packages/Reqnroll.MSTest)
- [MSTest](https://www.nuget.org/packages/MSTest) and [Microsoft.NET.Test.Sdk](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk), so that `dotnet test` and IDEs can run the specs
- [coverlet.msbuild](https://www.nuget.org/packages/coverlet.msbuild), for code coverage
- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection), used by the container bindings

Requires .NET 10.

## Getting started

```
dotnet add package Corvus.Testing.Reqnroll.MSTest
```

Then tell Reqnroll to load the Corvus bindings, in your test project's `reqnroll.json`:

```json
{
  "$schema": "https://schemas.reqnroll.net/reqnroll-config-latest.json",
  "stepAssemblies": [
    { "assembly": "Corvus.Testing.Reqnroll" }
  ]
}
```

See the [Corvus.Testing.Reqnroll readme](https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll#readme) for how to use the bindings, and the [changelog](https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/blob/main/CHANGELOG.md) for what's new.

Maintained by [endjin](https://endjin.com) and licensed under Apache 2.0.
