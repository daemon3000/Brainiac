using UnityEngine;
using System;
using System.Collections;

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
}