using Xunit;

public sealed class LoopTests
{
	[Theory]
	[InlineData("{let mut a=10 while a>0{a=a-1} a}", 0)]
	[InlineData("{let mut a=10 let b=2 while a>b{a=a-1} a}", 2)]
	[InlineData("{let mut a=10 let mut b=0 while a>0{a=a-1 b=b+1} b}", 10)]
	[InlineData("{let mut a=10 while a>0{a=a-1 break} a}", 9)]
	[InlineData("{let mut a=10 while:loop a>0{a=a-1 break:loop} a}", 9)]
	[InlineData("{let mut a=10 while:outer a>0{while true{a=a-1 break:outer}} a}", 9)]
	public void WhileIntTests(string source, int expected)
	{
		var n = TestHelper.RunExpression<Int>(source, out var a);
		a.AssertSuccessCall();
		Assert.Equal(expected, n.value);
	}

	[Theory]
	[InlineData("{let mut a=0 repeat 10{a=a+1} a}", 10)]
	[InlineData("{let mut a=0 repeat 10{a=a+1 break} a}", 1)]
	[InlineData("{let mut a=0 repeat:loop 10{a=a+1 break:loop} a}", 1)]
	[InlineData("{let mut a=0 repeat 5{a=a+it} a}", 10)]
	public void RepeatIntTests(string source, int expected)
	{
		var n = TestHelper.RunExpression<Int>(source, out var a);
		a.AssertSuccessCall();
		Assert.Equal(expected, n.value);
	}

	[Theory]
	[InlineData("{repeat 10{it=it+1}}")]
	[InlineData("{let _=it}")]
	[InlineData("{let mut a=0 a=a+it {}}")]
	public void RepeatIntTestsError(string source)
	{
		Assert.Throws<CompileErrorException>(() =>
		{
			TestHelper.RunExpression<Unit>(source, out var a);
		});
	}
}