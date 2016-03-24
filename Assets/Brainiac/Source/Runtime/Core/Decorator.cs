using UnityEngine;
using System;
using Brainiac.Serialization;

namespace Brainiac
{
	public abstract class Decorator : BehaviourNode
	{
		[JsonMember][JsonName("Child")]
		protected BehaviourNode m_child;

		public override void OnAwake()
		{
			m_child.OnAwake();
		}

		public override void OnReset()
		{
			base.OnReset();
			m_child.OnReset();
		}

		public void SetChild(BehaviourNode node)
		{
			m_child = node;
		}

		public BehaviourNode GetChild()
		{
			return m_child;
		}
	}
}