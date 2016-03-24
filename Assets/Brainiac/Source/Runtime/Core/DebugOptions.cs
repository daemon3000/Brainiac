namespace Brainiac
{
	public enum Breakpoint
	{
		None = 1 << 1,
		OnEnter = 1 << 2,
		OnExit = 1 << 3,
		OnSuccess = 1 << 4,
		OnFailure = 1 << 5
	}
}