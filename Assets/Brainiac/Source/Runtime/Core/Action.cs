using System;
using UnityEngine;

namespace Brainiac
{
	public abstract class Action : BehaviourNode
	{
		public override Vector2 Size
		{
			get
			{
				return new Vector2(100, 30);
			}
		}
	}
}
