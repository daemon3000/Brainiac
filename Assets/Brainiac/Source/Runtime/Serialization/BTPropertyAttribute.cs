using System;
using System.Reflection;

#if WINDOWS_STORE
using TP = System.Reflection.TypeInfo;
#else
using TP = System.Type;
#endif

using TCU = Brainiac.Serialization.TypeCoercionUtility;

namespace Brainiac.Serialization
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
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

		/// <summary>
		/// Gets the name specified for use in serialization.
		/// </summary>
		/// <returns></returns>
		public static string GetPropertyName(object value)
		{
			if(value == null)
			{
				return null;
			}

			Type type = value.GetType();
			MemberInfo memberInfo = null;

			if(TCU.GetTypeInfo(type).IsEnum)
			{
				string name = Enum.GetName(type, value);
				if(String.IsNullOrEmpty(name))
				{
					return null;
				}
				memberInfo = TCU.GetTypeInfo(type).GetField(name);
			}
			else
			{
				memberInfo = value as MemberInfo;
			}

			if(MemberInfo.Equals(memberInfo, null))
			{
				throw new ArgumentException();
			}

#if WINDOWS_STORE
			BTPropertyAttribute attribute = memberInfo.GetCustomAttribute<BTPropertyAttribute>(true);
#else
			BTPropertyAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(BTPropertyAttribute)) as BTPropertyAttribute;
#endif
			return attribute != null ? attribute.PropertyName : null;
		}
	}
}