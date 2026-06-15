# ConsoleUserInteractionHelper

[![CI](https://github.com/PFalkowski/ConsoleUserInteractionHelper/actions/workflows/ci.yml/badge.svg)](https://github.com/PFalkowski/ConsoleUserInteractionHelper/actions/workflows/ci.yml)
[![NuGet version](https://img.shields.io/nuget/v/ConsoleUserInteractionHelper.svg)](https://www.nuget.org/packages/ConsoleUserInteractionHelper/)
[![NuGet downloads](https://img.shields.io/nuget/dt/ConsoleUserInteractionHelper.svg)](https://www.nuget.org/packages/ConsoleUserInteractionHelper/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=PFalkowski_ConsoleUserInteractionHelper&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=PFalkowski_ConsoleUserInteractionHelper)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=PFalkowski_ConsoleUserInteractionHelper&metric=coverage)](https://sonarcloud.io/summary/new_code?id=PFalkowski_ConsoleUserInteractionHelper)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://choosealicense.com/licenses/mit/)
[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-support-yellow.svg)](https://www.buymeacoffee.com/piotrfalkowski)

A .NET library that simplifies console-based user interactions. All input methods support an optional `maxRetries` parameter — pass `null` (default) to retry indefinitely, or a positive integer to limit attempts.

## Input helpers

```csharp
var helper = new ConsoleHelper();

// strings
string name   = helper.GetNonEmptyStringFromUser(maxRetries: 3);
string path   = helper.GetPathToExistingFileFromUser(".csv");
string secret = helper.GetSecretStringFromUser();   // masked input

// integers
int positive = helper.GetPositiveInt();
int any      = helper.GetInt();
int inRange  = helper.GetIntInRange(1, 100);
int custom   = helper.GetIntWithConstraints(n => n % 2 == 0, "Enter an even number");

// other
DateTime date = helper.GetDateFromUser("dd/MM/yyyy");
bool yes      = helper.GetBinaryDecisionFromUser();

// option picker
var options = new List<string> { "Alpha", "Beta", "Gamma" };
string picked = helper.GetOptionValue(options, "Select environment:");
```

## Spinner

```csharp
TimeSpan elapsed = helper.ShowSpinnerUntilConditionTrue(() => !isReady);
// or
TimeSpan elapsed = helper.ShowSpinnerUntilTaskIsRunning(myTask);
```

## Progress bar

`ConsoleProgressReporter` wraps `ProgressReporting.ProgressReporter` and renders a terminal progress bar via [Goblinfactory.Konsole](https://github.com/goblinfactory/konsole).

```csharp
var reporter = new ConsoleProgressReporter();
reporter.Start(100);
for (int i = 0; i <= 100; i++)
    reporter.ReportProgress(i);
```

## CLI argument helpers

```csharp
bool verbose  = helper.GetFlagValue(args, "--verbose");
string output = helper.GetOptionValue(args, "--output", defaultValue: "out.csv");
```
