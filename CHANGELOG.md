# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [5.0.0] - 2026-11-15

### Changed

- **Breaking:** All packages now target .NET 10.0 instead of .NET 8.0.
- **Breaking:** Upgraded Reqnroll from 2.4.1 to 3.3.4. Projects that use these packages must move to Reqnroll 3.
- Updated Microsoft.Extensions.* to 10.0.15, Microsoft.NET.Test.Sdk to 18.12.0, MSTest to 4.5.3, NUnit3TestAdapter to 6.3.1 and coverlet.msbuild to 10.1.0.
- Package versions are managed centrally in `Solutions/Directory.Packages.props` (NuGet Central Package Management).
- The StyleCop and Roslynator analyzers now run on every build of the library and specs.
- Migrated the solution file from `.sln` to `.slnx`.
- Upgraded the ZeroFailed build to 2.0.1, with a committed extensions lock file that CI installs in Frozen mode.
- Updated the GitHub Actions workflows. Dependabot now also updates GitHub Actions.
- Updated the package icon.

## [4.0.5] - 2025-06-04

### Changed

- Migrated the build process to ZeroFailed.

## [4.0.4] - 2025-06-04

### Changed

- Updated NuGet package dependencies, including MSTest, coverlet.msbuild and Endjin.RecommendedPractices.GitHub.

## [4.0.3] - 2024-11-30

### Added

- `Corvus.Testing.ReqnRoll.MSTest` metapackage, for projects that use Reqnroll with MSTest.

## [4.0.2] - 2024-11-28

### Fixed

- Reqnroll and NUnit3TestAdapter assemblies are now available to projects that reference the `Corvus.Testing.ReqnRoll.NUnit` metapackage.

## [4.0.1] - 2024-11-28

### Changed

- `Corvus.Testing.ReqnRoll.NUnit` now references Reqnroll.NUnit and NUnit3TestAdapter, instead of NUnit directly.

### Removed

- Duplicate package references.

## [4.0.0] - 2024-11-25

### Added

- First release of `Corvus.Testing.ReqnRoll` and the `Corvus.Testing.ReqnRoll.NUnit` metapackage, built on Reqnroll and targeting .NET 8.0.

[Unreleased]: https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/compare/5.0.0...HEAD
[5.0.0]: https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/compare/4.0.5...5.0.0
[4.0.5]: https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/compare/4.0.4...4.0.5
[4.0.4]: https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/compare/4.0.3...4.0.4
[4.0.3]: https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/compare/4.0.2...4.0.3
[4.0.2]: https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/compare/4.0.1...4.0.2
[4.0.1]: https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/compare/4.0.0...4.0.1
[4.0.0]: https://github.com/corvus-dotnet/Corvus.Testing.ReqnRoll/releases/tag/4.0.0
