using System;

namespace Brainiac
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class BTPropertyAttribute : Attribute
	{
		public string PropertyName { get; set; }

		public BTPropertyAttribute()
		{
			PropertyName = null;
		}

		public BTPropertyAttribute(string propertyName)
		{
			PropertyName = propertyName;
		}
	}
}