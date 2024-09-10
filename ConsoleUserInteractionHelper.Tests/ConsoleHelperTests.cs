using Xunit.Abstractions;

namespace ConsoleUserInteractionHelper.Tests;

public class ConsoleHelperTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ConsoleHelper _helper;
    private TextWriter _originalOutput;
    private TextReader _originalInput;

    public ConsoleHelperTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _testOutputHelper.WriteLine("test");
        _helper = new ConsoleHelper();
        _originalOutput = Console.Out;
        _originalInput = Console.In;
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        Console.SetIn(_originalInput);
    }

    private static (StringWriter outputWriter, StringReader inputReader) SetupConsoleIO(string input)
    {
        var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        var inputReader = new StringReader(input);
        Console.SetIn(inputReader);

        return (outputWriter, inputReader);
    }

    [Fact]
    public void GetNonEmptyStringFromUser_ValidInput_ReturnsInput()
    {
        var (outputWriter, _) = SetupConsoleIO("Hello");
        var result = _helper.GetNonEmptyStringFromUser();
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void GetNonEmptyStringFromUser_EmptyThenValidInput_ReturnsValidInput()
    {
        var (outputWriter, _) = SetupConsoleIO("\nHello");
        var result = _helper.GetNonEmptyStringFromUser();
        Assert.Equal("Hello", result);
        Assert.Contains("Input cannot be empty", outputWriter.ToString());
    }

    [Fact]
    public void GetNonEmptyStringFromUser_MaxRetriesExceeded_ThrowsException()
    {
        var (_, _) = SetupConsoleIO("\n\n\n");
        Assert.Throws<InvalidOperationException>(() => _helper.GetNonEmptyStringFromUser(maxRetries: 2));
    }

    [Fact]
    public void GetIntWithConstraints_ValidInput_ReturnsInput()
    {
        var (_, _) = SetupConsoleIO("42");
        var result = _helper.GetIntWithConstraints();
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetIntWithConstraints_InvalidThenValidInput_ReturnsValidInput()
    {
        var (outputWriter, _) = SetupConsoleIO("not a number\n42");
        var result = _helper.GetIntWithConstraints();
        Assert.Equal(42, result);
        Assert.Contains("Invalid input", outputWriter.ToString());
    }

    [Fact]
    public void GetIntWithConstraints_CustomConstraint_ReturnsValidInput()
    {
        var (outputWriter, _) = SetupConsoleIO("41\n42");
        var result = _helper.GetIntWithConstraints(n => n % 2 == 0, "Please enter an even number");
        Assert.Equal(42, result);
        Assert.Contains("Please enter an even number", outputWriter.ToString());
    }

    [Fact]
    public void GetPositiveInt_ValidInput_ReturnsInput()
    {
        var (_, _) = SetupConsoleIO("5");
        var result = _helper.GetPositiveInt();
        Assert.Equal(5, result);
    }

    [Fact]
    public void GetPositiveInt_NegativeThenValidInput_ReturnsValidInput()
    {
        var (outputWriter, _) = SetupConsoleIO("-5\n5");
        var result = _helper.GetPositiveInt();
        Assert.Equal(5, result);
        Assert.Contains("Please enter a positive integer greater than 0", outputWriter.ToString());
    }

    [Fact]
    public void GetNegativeInt_ValidInput_ReturnsInput()
    {
        var (_, _) = SetupConsoleIO("-5");
        var result = _helper.GetNegativeInt();
        Assert.Equal(-5, result);
    }

    [Fact]
    public void GetIntInRange_ValidInput_ReturnsInput()
    {
        var (_, _) = SetupConsoleIO("5");
        var result = _helper.GetIntInRange(1, 10);
        Assert.Equal(5, result);
    }

    [Fact]
    public void GetNonNegativeInt_ValidInput_ReturnsInput()
    {
        var (_, _) = SetupConsoleIO("0");
        var result = _helper.GetNonNegativeInt();
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetNaturalInt_ValidInput_ReturnsInput()
    {
        var (_, _) = SetupConsoleIO("5");
        var result = _helper.GetNaturalInt();
        Assert.Equal(5, result);
    }

    [Fact]
    public void GetIntInRange_OutOfRangeThenValidInput_ReturnsValidInput()
    {
        var (outputWriter, _) = SetupConsoleIO("15\n5");
        var result = _helper.GetIntInRange(1, 10);
        Assert.Equal(5, result);
        Assert.Contains("Please enter an integer between 1 and 10", outputWriter.ToString());
    }

    [Fact]
    public void GetIntWithConstraints_MaxRetriesExceeded_ThrowsException()
    {
        var (_, _) = SetupConsoleIO("not a number\nnot a number\nnot a number");
        Assert.Throws<InvalidOperationException>(() => _helper.GetIntWithConstraints(maxRetries: 2));
    }

    [Fact]
    public void GetInt_ValidInput_ReturnsInput()
    {
        var (_, _) = SetupConsoleIO("-42");
        var result = _helper.GetInt();
        Assert.Equal(-42, result);
    }

    [Fact]
    public void GetDateFromUser_ValidInput_ReturnsDate()
    {
        var (_, _) = SetupConsoleIO("2023-05-15");
        var result = _helper.GetDateFromUser();
        Assert.Equal(new DateTime(2023, 5, 15), result);
    }

    [Fact]
    public void GetDateFromUser_InvalidThenValidInput_ReturnsValidDate()
    {
        var (outputWriter, _) = SetupConsoleIO("not a date\n2023-05-15");
        var result = _helper.GetDateFromUser();
        Assert.Equal(new DateTime(2023, 5, 15), result);
        Assert.Contains("Invalid date format", outputWriter.ToString());
    }

    [Fact]
    public void GetDateFromUser_CustomFormat_ReturnsCorrectDate()
    {
        var (_, _) = SetupConsoleIO("15/05/2023");
        var result = _helper.GetDateFromUser("dd/MM/yyyy");
        Assert.Equal(new DateTime(2023, 5, 15), result);
    }
}