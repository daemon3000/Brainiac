using UnityEngine;

namespace Brainiac
{
	public class Move : Action
	{
		private string m_destination;

		private Vector3 m_destinationValue;
		private bool m_isMoving;

		public string Destination
		{
			get
			{
				return m_destination;
			}

			set
			{
				m_destination = value;
			}
		}

		public override string Title
		{
			get
			{
				return "Move";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController ai)
		{
			if(m_destination == null)
				return BehaviourNodeStatus.Failure;

			if(m_isMoving)
			{
				if(ai.Navigator.IsAtDestination(m_destinationValue))
				{
					m_isMoving = false;
					return BehaviourNodeStatus.Success;
				}
			}
			else
			{
				m_isMoving = true;
				ai.Navigator.SetDestination(m_destinationValue);
			}

			return BehaviourNodeStatus.Running;
		}

		protected override void OnStart(AIController ai)
		{
			m_isMoving = false;
			m_destinationValue = ai.Memory.GetVector3(m_destination, Vector3.zero);
		}
	}
}