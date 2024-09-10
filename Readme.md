## Package Description

ConsoleHelper is a versatile .NET library designed to simplify console-based user interactions in your applications. It provides a rich set of methods to handle various input scenarios, from simple string inputs to complex numeric constraints.

### Key Features

- **Robust Input Handling**: Gracefully manage user inputs with built-in validation and error handling.
- **Flexible Numeric Inputs**: Easily obtain integer values with custom constraints.
- **Secure String Input**: Collect sensitive information without displaying it on the console.
- **Progress Indication**: Display spinner animations for long-running operations.
- **Customizable Retry Logic**: Control the number of retry attempts for each input operation.

### Installation

Install ConsoleHelper via NuGet Package Manager:

```
Install-Package ConsoleHelper
```

Or via .NET CLI:

```
dotnet add package ConsoleHelper
```

### Usage Examples

Here are some examples to demonstrate the versatility of ConsoleHelper:

```csharp
using ConsoleHelper;

var helper = new ConsoleHelper();

// Get a non-empty string
string name = helper.GetNonEmptyStringFromUser();

// Get a positive integer with max 3 retry attempts
int age = helper.GetPositiveInt(maxRetries: 3);

// Use custom constraints for integer input
int evenNumber = helper.GetIntWithConstraints(
    n => n % 2 == 0, 
    "Please enter an even number."
);

// Collect a password securely
string password = helper.GetSecretStringFromUser();

// Display a spinner during a long operation
Task longRunningTask = SomeLongRunningOperation();
helper.ShowSpinnerUntilTaskIsRunning(longRunningTask);

// Get a file path with specific extension
string filePath = helper.GetPathToExistingFileFromUser(".txt");
```

### Extensibility

ConsoleHelper is designed with extensibility in mind. You can easily create custom input methods using the generic `GetIntWithConstraints` method:

```csharp
// Custom method to get a prime number
public int GetPrimeNumber(int? maxRetries = null)
{
    return GetIntWithConstraints(
        n => IsPrime(n),
        "Please enter a prime number.",
        maxRetries
    );
}

private bool IsPrime(int number)
{
    if (number < 2) return false;
    for (int i = 2; i <= Math.Sqrt(number); i++)
    {
        if (number % i == 0) return false;
    }
    return true;
}
```

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](Contributing.md) for more details.

## License

ConsoleHelper is released under the [MIT License](LICENSE).