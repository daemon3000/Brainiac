using System;

namespace Brainiac
{
	public class AddNodeMenuAttribute : Attribute
	{
		public readonly string MenuPath;

		public AddNodeMenuAttribute(string menuPath)
		{
			MenuPath = menuPath;
		}
	}

	public class AddServiceMenuAttribute : Attribute
	{
		public readonly string MenuPath;

		public AddServiceMenuAttribute(string menuPath)
		{
			MenuPath = menuPath;
		}
	}

	public class AddConstraintMenuAttribute : Attribute
	{
		public readonly string MenuPath;

		public AddConstraintMenuAttribute(string menuPath)
		{
			MenuPath = menuPath;
		}
	}
}