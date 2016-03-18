using System;

namespace BrainiacEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CustomNodeInspectorAttribute : Attribute
	{
		public readonly Type NodeType;

		public CustomNodeInspectorAttribute(Type nodeType)
		{
			NodeType = nodeType;
		}
	}
}