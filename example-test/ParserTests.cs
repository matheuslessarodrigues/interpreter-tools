using Xunit;

public sealed class ParserTest
{
	private readonly ExampleParser parser = new ExampleParser(new ExampleTokenizer());

	[Theory]
	[InlineData("-1;")]
	[InlineData("1 + 2;")]
	[InlineData("1 * -2;")]
	[InlineData("1 + 2 * 3;")]
	[InlineData("(1 + 2) + 3 * 4 + 5;")]
	[InlineData("(1 + 2) + 3 == 4 + 5;")]
	[InlineData("1 < 2 != 3 >= 4;")]
	[InlineData("true == !false;")]
	[InlineData("\"text\" != nil;")]
	[InlineData("true or false;")]
	[InlineData("true and false or 3 > 2;")]
	[InlineData("assign = true or false;")]
	public void TestExpressions(string source)
	{
		var result = parser.Parse(source);
		Assert.True(result.isOk, result.error);
	}

	[Theory]
	[InlineData("while true { 1 + 2 }")]
	public void TestStatements(string source)
	{
		var result = parser.Parse(source);
		Assert.True(result.isOk, result.error);
	}
}
