using UnityEngine;
using System;
using System.Collections;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Repeat Forever")]
	public class RepeatForever : Decorator
	{
		public override string Title
		{
			get
			{
				return "Repeat Forever";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			if(m_child != null)
			{
				if(m_child.Status != BehaviourNodeStatus.None && m_child.Status != BehaviourNodeStatus.Running)
				{
					m_child.OnReset();
				}

				m_child.Run(agent);
			}

			return BehaviourNodeStatus.Running;
		}
	}
}