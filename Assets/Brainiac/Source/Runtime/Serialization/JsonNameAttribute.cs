#region License
/*---------------------------------------------------------------------------------*\

	Distributed under the terms of an MIT-style license:

	The MIT License

	Copyright (c) 2006-2009 Stephen M. McKamey

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.

\*---------------------------------------------------------------------------------*/
#endregion License

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
	/// <summary>
	/// Specifies the naming to use for a property or field when serializing
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple=false)]
	public class JsonNameAttribute : Attribute
	{
		private string jsonName = null;

		/// <summary>
		/// Ctor
		/// </summary>
		public JsonNameAttribute()
		{
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="jsonName"></param>
		public JsonNameAttribute(string jsonName)
		{
			this.jsonName = jsonName;
		}

		/// <summary>
		/// Gets and sets the name to be used in JSON
		/// </summary>
		public string Name
		{
			get { return this.jsonName; }
			set { this.jsonName = value; }
		}

		/// <summary>
		/// Gets the name specified for use in Json serialization.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetJsonName(object value)
		{
			if (value == null)
			{
				return null;
			}

			Type type = value.GetType();
			MemberInfo memberInfo = null;

			if (TCU.GetTypeInfo(type).IsEnum)
			{
				string name = Enum.GetName(type, value);
				if (String.IsNullOrEmpty(name))
				{
					return null;
				}
				memberInfo = TCU.GetTypeInfo(type).GetField(name);
			}
			else
			{
				memberInfo = value as MemberInfo;
			}

			if (MemberInfo.Equals (memberInfo, null)) {
				throw new ArgumentException ();
			}

#if WINDOWS_STORE
			JsonNameAttribute nameAttribute = memberInfo.GetCustomAttribute<JsonNameAttribute> (true);
			JsonMemberAttribute memberAttribute = memberInfo.GetCustomAttribute<JsonMemberAttribute> (true);
#else
			JsonNameAttribute nameAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(JsonNameAttribute)) as JsonNameAttribute;
			JsonMemberAttribute memberAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(JsonMemberAttribute)) as JsonMemberAttribute;
#endif
			if(nameAttribute != null)
				return nameAttribute.Name;
			else if(memberAttribute != null)
				return memberAttribute.Name;
			else
				return null;
		}
	}
}
