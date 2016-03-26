using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Composite/Random")]
	public class Random : Composite
	{
		protected BehaviourNode m_chosenChild;

		protected override void OnEnter(AIController aiController)
		{
			if(m_children.Count > 0)
			{
				m_chosenChild = m_children[UnityEngine.Random.Range(0, m_children.Count)];
			}
			else
			{
				m_chosenChild = null;
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController aiController)
		{
			if(m_chosenChild != null)
			{
				return m_chosenChild.Run(aiController);
			}

			return BehaviourNodeStatus.Success;
		}
	}
}