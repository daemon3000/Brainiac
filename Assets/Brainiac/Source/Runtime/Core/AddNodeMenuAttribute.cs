using UnityEngine;
using System;
using System.Collections;

namespace Brainiac
{
	public class AddBehaviourNodeMenuAttribute : Attribute
	{
		public readonly string MenuPath;

		public AddBehaviourNodeMenuAttribute(string menuPath)
		{
			MenuPath = menuPath;
		}
	}
}