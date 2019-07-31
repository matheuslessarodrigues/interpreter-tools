using Xunit;

public sealed class LangTests
{
	public static string RunExpression(string source, out ValueData value, out ValueType type)
	{
		LangParseRules.InitRules();

		value = new ValueData();
		type = new ValueType();

		var tokenizer = new Tokenizer();
		var compiler = new LangCompiler();

		var compileResult = compiler.CompileExpression(source, tokenizer);
		if (!compileResult.isOk)
			return "COMPILE ERROR: " + CompilerHelper.FormatError(source, compileResult.error, 1, 8);

		var vm = new VirtualMachine();
		var runResult = vm.Run(source, compileResult.ok);
		if (!runResult.isOk)
			return "RUNTIME ERROR: " + VirtualMachineHelper.FormatError(source, runResult.error, 1, 8);

		type = vm.PeekType();
		value = vm.PopValue();
		return null;
	}

	[Theory]
	[InlineData("{}")]
	[InlineData("{{}}")]
	[InlineData("{let a=4 a=a+1 {}}")]
	public void BlockNilTests(string source)
	{
		var error = RunExpression(source, out var v, out var t);
		Assert.Null(error);
		Assert.Equal(ValueType.Nil, t);
	}

	[Theory]
	[InlineData("{0}", 0)]
	[InlineData("{4}", 4)]
	[InlineData("{{4}}", 4)]
	[InlineData("{let a=4 a}", 4)]
	[InlineData("{let a=4 a+5}", 9)]
	[InlineData("{let a=4 {let a=2 a+1} a+5}", 9)]
	public void BlockIntTests(string source, int expected)
	{
		var error = RunExpression(source, out var v, out var t);
		Assert.Null(error);
		Assert.Equal(ValueType.Int, t);
		Assert.Equal(expected, v.asInt);
	}

	[Theory]
	[InlineData("if true {}")]
	[InlineData("if true {} else {}")]
	[InlineData("if true {} else if false {}")]
	[InlineData("if true {} else if false {} else {}")]
	[InlineData("if true {let a=0 a+1 nil}")]
	[InlineData("if true {nil}")]
	public void IfNilTests(string source)
	{
		var error = RunExpression(source, out var v, out var t);
		Assert.Null(error);
		Assert.Equal(ValueType.Nil, t);
	}

	[Theory]
	[InlineData("if true {4} else {5}", 4)]
	[InlineData("if 2>3 {4} else {5}", 5)]
	[InlineData("if 3>3 {4} else if 3<3 {-4} else {5}", 5)]
	[InlineData("if if false{true}else{false} {4} else {5}", 5)]
	[InlineData("if true {4} else {5} + 10", 14)]
	[InlineData("20 + if true {4} else {5}", 24)]
	public void IfIntTests(string source, int expected)
	{
		var error = RunExpression(source, out var v, out var t);
		Assert.Null(error);
		Assert.Equal(ValueType.Int, t);
		Assert.Equal(expected, v.asInt);
	}

	[Theory]
	[InlineData("true and true", true)]
	[InlineData("true and false", false)]
	[InlineData("false and true", false)]
	[InlineData("false and false", false)]
	[InlineData("true or true", true)]
	[InlineData("true or false", true)]
	[InlineData("false or true", true)]
	[InlineData("false or false", false)]
	public void LogicalTests(string source, bool expected)
	{
		var error = RunExpression(source, out var v, out var t);
		Assert.Null(error);
		Assert.Equal(ValueType.Bool, t);
		Assert.Equal(expected, v.asBool);
	}

	[Theory]
	[InlineData("{let a=4 a=a+1 a}", 5)]
	[InlineData("{let a=4 a=a=5 a}", 5)]
	[InlineData("{let a=4 a=a=a+1 a}", 5)]
	[InlineData("{let a=4 a=a=a+1 a}", 5)]
	[InlineData("{let a=4 let b=5 b+1 a=b=7 a}", 7)]
	public void AssignmentIntTests(string source, int expected)
	{
		var error = RunExpression(source, out var v, out var t);
		Assert.Null(error);
		Assert.Equal(ValueType.Int, t);
		Assert.Equal(expected, v.asInt);
	}
}