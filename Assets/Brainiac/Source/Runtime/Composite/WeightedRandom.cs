using UnityEngine;
using System;
using System.Collections;

namespace Brainiac
{
	[AddNodeMenu("Composite/Weighted Random")]
	public class WeightedRandom : Random
	{
		private float[] m_weights;

		public override string Title
		{
			get
			{
				return "Weighted Random";
			}
		}

		public override void OnStart(AIAgent agent)
		{
			Status = BehaviourNodeStatus.None;
			m_weights = new float[m_children.Count];

			for(int i = 0; i < m_children.Count; i++)
			{
				m_children[i].OnStart(agent);
				m_weights[i] = m_children[i].Weight;
			}
		}

		protected override void OnEnter(AIAgent agent)
		{
			m_chosenChild = ChooseRandomChild();
		}

		private BehaviourNode ChooseRandomChild()
		{
			BehaviourNode child = null;

			float rand = UnityEngine.Random.value;
			for(int i = 0; i < m_children.Count; i++)
			{
				if(rand < m_weights[i])
				{
					child = m_children[i];
					break;
				}

				rand -= m_weights[i];
			}

			return child;
		}
	}
}