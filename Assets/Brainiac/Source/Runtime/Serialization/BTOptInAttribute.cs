using System;
namespace Brainiac.Serialization
{
	/** Specifies that members of this class that should be serialized must be explicitly specified.
	 * Classes that this attribute is applied to need to explicitly
	 * declare every member that should be serialized with the JsonMemberAttribute.
	 * \see JsonMemberAttribute
	 */
	public class BTOptInAttribute : Attribute
	{
		public BTOptInAttribute ()
		{
			
		}
	}

	public class BTUseTypeHintAttribute : Attribute
	{
		public BTUseTypeHintAttribute () {}
	}
}

