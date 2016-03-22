namespace Brainiac
{
	public enum DebugOptions
	{
		None = 1 << 1,
		BreakOnEnter = 1 << 2,
		BreakOnExit = 1 << 3,
		BreakOnSuccess = 1 << 4,
		BreakOnFailure = 1 << 5
	}
}