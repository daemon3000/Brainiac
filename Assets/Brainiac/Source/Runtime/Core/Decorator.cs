using UnityEngine;
using System;
using Brainiac.Serialization;

namespace Brainiac
{
	public abstract class Decorator : BehaviourNode
	{
		[BTProperty("Child")]
		[BTHideInInspector]
		protected BehaviourNode m_child;

		public override void OnBeforeSerialize(BTAsset btAsset)
		{
			if(m_child != null)
			{
				m_child.OnBeforeSerialize(btAsset);
			}
		}

		public override void OnAfterDeserialize(BTAsset btAsset)
		{
			if(m_child != null)
			{
				m_child.OnAfterDeserialize(btAsset);
			}
		}

		public override void OnStart(AIAgent agent)
		{
			if(m_child != null)
			{
				m_child.OnStart(agent);
			}
		}

		public override void OnReset()
		{
			base.OnReset();
			if(m_child != null)
			{
				m_child.OnReset();
			}
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