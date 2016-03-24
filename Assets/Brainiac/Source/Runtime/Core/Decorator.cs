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
			base.OnAwake();
			m_child.OnAwake();
		}

		public void ReplaceChild(BehaviourNode node)
		{
			m_child = node;
		}

		public void RemoveChild()
		{
			m_child = null;
		}

		public BehaviourNode GetChild()
		{
			return m_child;
		}
	}
}