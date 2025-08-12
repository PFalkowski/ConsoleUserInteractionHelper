using System;
using Xunit;

namespace ConsoleUserInteractionHelper.Tests
{
    public class ConsoleHelperTests
    {
        private readonly ConsoleHelper _consoleHelper = new ConsoleHelper();

        [Theory]
        [InlineData(new[] { "--verbose" }, "--verbose", true)]
        [InlineData(new[] { "-v" }, "-v", true)]
        [InlineData(new[] { "--verbose" }, "-v", false)]
        [InlineData(new string[] { }, "-v", false)]
        public void GetFlagValue_ReturnsCorrectValue(string[] args, string flag, bool expected)
        {
            var result = _consoleHelper.GetFlagValue(args, flag);
            Assert.True(result == expected);
        }

        [Theory]
        [InlineData(new[] { "--file", "test.txt" }, "--file", "test.txt")]
        [InlineData(new[] { "--file", "test.txt" }, "--f", null)]
        [InlineData(new[] { "-f", "test.txt" }, "-f", "test.txt")]
        [InlineData(new string[] { }, "--file", null)]
        [InlineData(new[] { "--file" }, "--file", null)]
        [InlineData(new[] { "--file", "test.txt", "--another", "value" }, "--file", "test.txt")]
        public void GetOptionValue_String_ReturnsCorrectValue(string[] args, string option, string expected)
        {
            var result = _consoleHelper.GetOptionValue<string>(args, option);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetOptionValue_String_ReturnsDefaultValue_WhenOptionNotFound()
        {
            var args = new[] { "--another", "value" };
            var result = _consoleHelper.GetOptionValue<string>(args, "--file", "default.txt");
            Assert.Equal("default.txt", result);
        }

        [Theory]
        [InlineData(new[] { "--count", "123" }, "--count", 123)]
        [InlineData(new[] { "--count", "abc" }, "--count", 0)] // Default for int
        [InlineData(new string[] { }, "--count", 0)]
        public void GetOptionValue_Int_ReturnsCorrectValue(string[] args, string option, int expected)
        {
            var result = _consoleHelper.GetOptionValue<int>(args, option);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetOptionValue_Int_ReturnsDefaultValue_WhenOptionNotFound()
        {
            var args = new[] { "--another", "value" };
            var result = _consoleHelper.GetOptionValue<int>(args, "--count", 999);
            Assert.Equal(999, result);
        }

        [Theory]
        [InlineData(new[] { "--enabled", "true" }, "--enabled", true)]
        [InlineData(new[] { "--enabled", "false" }, "--enabled", false)]
        [InlineData(new[] { "--enabled", "True" }, "--enabled", true)]
        [InlineData(new[] { "--enabled", "False" }, "--enabled", false)]
        [InlineData(new[] { "--enabled", "1" }, "--enabled", true)]
        [InlineData(new[] { "--enabled", "0" }, "--enabled", false)]
        [InlineData(new[] { "--enabled", "abc" }, "--enabled", false)] // Default for bool
        [InlineData(new string[] { }, "--enabled", false)]
        public void GetOptionValue_Bool_ReturnsCorrectValue(string[] args, string option, bool expected)
        {
            var result = _consoleHelper.GetOptionValue<bool>(args, option);
            Assert.True(result == expected);
        }

        [Fact]
        public void GetOptionValue_Bool_ReturnsDefaultValue_WhenOptionNotFound()
        {
            var args = new[] { "--another", "value" };
            var result = _consoleHelper.GetOptionValue<bool>(args, "--enabled", true);
            Assert.True(result);
        }
    }
}