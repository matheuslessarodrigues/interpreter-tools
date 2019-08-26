public readonly struct RuntimeError
{
	public readonly int instructionIndex;
	public readonly Slice slice;
	public readonly string message;

	public RuntimeError(int instructionIndex, Slice slice, string message)
	{
		this.instructionIndex = instructionIndex;
		this.slice = slice;
		this.message = message;
	}
}

internal struct CallFrame
{
	public int functionIndex;
	public int codeIndex;
	public int baseStackIndex;

	public CallFrame(int functionIndex, int codeIndex, int baseStackIndex)
	{
		this.functionIndex = functionIndex;
		this.codeIndex = codeIndex;
		this.baseStackIndex = baseStackIndex;
	}
}

public sealed class VirtualMachine
{
	internal ByteCodeChunk chunk;
	internal Buffer<ValueData> valueStack = new Buffer<ValueData>(256);
	internal Buffer<CallFrame> callframeStack = new Buffer<CallFrame>(64);
	internal Buffer<object> nativeObjects;
	internal Option<RuntimeError> error;

	public void Load(ByteCodeChunk chunk)
	{
		this.chunk = chunk;
		error = Option.None;

		valueStack.count = 0;
		callframeStack.count = 0;

		nativeObjects = new Buffer<object>
		{
			buffer = new object[chunk.stringLiterals.buffer.Length],
			count = chunk.stringLiterals.count
		};
		for (var i = 0; i < nativeObjects.count; i++)
			nativeObjects.buffer[i] = chunk.stringLiterals.buffer[i];
	}

	public void CallTopFunction()
	{
		VirtualMachineInstructions.Run(this);
	}

	public void Error(string message)
	{
		var ip = -1;
		if (callframeStack.count > 0)
			ip = callframeStack.buffer[callframeStack.count - 1].codeIndex;

		error = Option.Some(new RuntimeError(
			ip,
			ip >= 0 ? chunk.slices.buffer[ip] : new Slice(),
			message
		));
	}
}