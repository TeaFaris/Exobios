using System;

public sealed class InteractArgs : EventArgs
{
	public readonly Entity User;

	public InteractArgs(Entity user)
	{
		User = user;
	}
}