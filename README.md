# OpenDBDiff
A database comparison tool for Microsoft SQL Server 2005+ that reports schema differences and creates a synchronization script.

[![ci](https://github.com/OpenDBDiff/OpenDBDiff/actions/workflows/ci.yml/badge.svg?branch=develop)](https://github.com/OpenDBDiff/OpenDBDiff/actions/workflows/ci.yml)

[💾 Download latest release](https://github.com/OpenDBDiff/OpenDBDiff/releases/latest)

[💾 Download develop CI build](https://github.com/OpenDBDiff/OpenDBDiff/actions/workflows/ci.yml?query=branch%3Adevelop)

The CI build for `develop` is attached to every workflow run as `OpenDBDiff-WinFormsGUI-*` and
`OpenDBDiff-CLI-*` artifacts (keep a run's download for 14 days).

Open DBDiff can synchronize
* Tables (including Table Options like vardecimal, text in row, etc.)
* Columns (including Computed Columns, XML options, Identities, etc.)
* Constraints
* Indexes (and XML Indexes)
* XML Schemas
* Table Types
* User Data Types (UDT)
* CLR Objects (Assemblies, CLR-UDT, CLR-Store Procedure, CLR-Triggers)
* Triggers (including DDL Triggers)
* Synonyms
* Schemas
* File groups
* Views
* Functions 
* Store Procedures
* Partition Functions/Schemes
* Users
* Roles

## Building and testing

```
dotnet build -c Release
dotnet test  -c Release
```

The test project targets `net48` and `net10.0`, so the suite runs on Windows (both legs) and on Linux
(`net10.0`). The WinForms projects are Windows-only; the schema libraries and the `net10.0` CLI build on
either platform. `dotnet build` needs the .NET 10 SDK.

## Releasing

1. Bump `<Version>` in `Directory.Build.props` on `develop`, so local builds agree with the release.
2. Tag that commit: `git tag v0.14.0 && git push origin v0.14.0`.
3. The `release` workflow builds, tests, packages `OpenDBDiff_<version>.zip` and
   `OpenDBDiff.CLI_<version>.zip` with a `SHA256SUMS.txt`, attests their build provenance, and opens a
   **draft** GitHub release with generated notes.
4. Review the notes and publish the draft.

The tag wins over `Directory.Build.props`: the workflow passes `-p:Version=<tag>`, which is what ends up
in every assembly. To rehearse without tagging, run the `release` workflow manually with the *dry run*
input - it builds and packages the artifacts but publishes nothing.


![image](https://cloud.githubusercontent.com/assets/145854/24702579/9e657426-19ff-11e7-9722-9d2f4a54d2ed.png)
![image](https://cloud.githubusercontent.com/assets/145854/24702592/abc1b094-19ff-11e7-9e1b-d4ed581105a3.png)

Icons provided from http://www.fatcow.com/free-icons under under a Creative Commons Attribution 3.0 License.
