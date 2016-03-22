using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Brainiac
{
	public abstract class Decorator : BehaviourNode
	{
		[JsonProperty(PropertyName = "Child")]
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