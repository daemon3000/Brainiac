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

		public override void OnStart(AIController aiController)
		{
			m_child.OnStart(aiController);
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