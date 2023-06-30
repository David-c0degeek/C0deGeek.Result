# C0deGeek.Result

C0deGeek.Result is a library for handling and chaining the results of operations in a functional style.

## Features

    Represents the result of an operation as a Result or Result<T> object.
    Provides a fluent API for chaining operations and handling their results.
    Distinguishes between different types of operation results, such as success, failure, and not found.

## Usage

The following code snippets demonstrate the usage of this library:

```csharp

using C0deGeek.Result;
using C0deGeek.Result.Extensions;

// ...

// Combine results
var combinedResult = Result.Ok()
.Combine(ValidateSomething(input))
.Combine(ValidateSomethingElse(input2));

// Throw on failure
combinedResult.ThrowFunctionalExceptionOnFailure();

// Extract value or throw on failure
var someClassValue = await GetSomeClassResult().GetValueOrThrowFunctionalException();

// On success, perform another operation
var finalResult = await GetSomeClassResult().OnSuccessAsync(value => PerformAnotherOperation(value));
```

## Description of Methods

* **`Combine`:** The Combine extension method allows you to chain multiple operations together. If any operation fails, the rest are not executed.
* **`ThrowFunctionalExceptionOnFailure`:** The ThrowFunctionalExceptionOnFailure extension method throws a FunctionalException if the result represents a failure.
* **`GetValueOrThrowFunctionalException`:** The GetValueOrThrowFunctionalException extension method extracts the value from a Result<T> or throws a FunctionalException if the result represents a failure.
* **`OnSuccessAsync`:** The OnSuccessAsync extension method allows you to chain another operation that should be performed only if the previous operation was successful.

## Installation

Add a reference to the C0deGeek.Result NuGet package to your project.

## Contributing

Contributions are welcome. Please open an issue or submit a pull request on GitHub.