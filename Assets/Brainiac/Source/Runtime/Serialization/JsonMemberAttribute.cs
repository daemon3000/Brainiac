using System;
namespace Brainiac.Serialization
{
	/** Explicitly declare this member to be serialized.
	 * \see JsonOptInAttribute
	 */
	public class JsonMemberAttribute : Attribute
	{
		/// <summary>
		/// Gets and sets the name to be used in JSON
		/// </summary>
		public string Name { get; set; }

		public JsonMemberAttribute()
		{
			Name = null;
		}

		public JsonMemberAttribute(string name)
		{
			Name = name;
		}
	}
}

