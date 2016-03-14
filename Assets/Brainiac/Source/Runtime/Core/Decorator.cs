using UnityEngine;
using Newtonsoft.Json;

namespace Brainiac
{
	public abstract class Decorator : BehaviourNode
	{
		[JsonProperty(PropertyName = "Child")]
		protected BehaviourNode m_child;

		public override void OnInitialize()
		{
			base.OnInitialize();
			if(m_child != null)
			{
				m_child.OnInitialize();
			}
		}

		public override void OnReset()
		{
			if(m_child != null)
			{
				m_child.OnReset();
			}
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