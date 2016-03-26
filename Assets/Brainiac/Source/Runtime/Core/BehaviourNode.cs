using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	public abstract class BehaviourNode
	{
		private Vector2 m_position;
		private string m_description;
		private string m_name;
		private float m_weight;
		private Breakpoint m_breakpoint;
		private BehaviourNodeStatus m_status;

		public Vector2 Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		public Breakpoint Breakpoint
		{
			get
			{
				return m_breakpoint;
			}
			set
			{
				m_breakpoint = value;
			}
		}
		
		public float Weight
		{
			get
			{
				return m_weight;
			}
			set
			{
				m_weight = value;
			}
		}

		[JsonIgnore]
		public virtual string Title
		{
			get { return GetType().Name; }
		}

		[JsonIgnore]
		public BehaviourNodeStatus Status
		{
			get { return m_status; }
			protected set { m_status = value; }
		}

		public BehaviourNode()
		{
			m_breakpoint = Breakpoint.None;
			m_status = BehaviourNodeStatus.None;
		}

		public virtual void OnStart(AIController aiController) { }
		protected virtual void OnEnter(AIController aiController) { }
		protected virtual void OnExit(AIController aiController) { }
		protected abstract BehaviourNodeStatus OnExecute(AIController aiController);

		public virtual void OnReset()
		{
			m_status = BehaviourNodeStatus.None;
		}

		public BehaviourNodeStatus Run(AIController aiController)
		{
			if(m_status != BehaviourNodeStatus.Running)
			{
				OnEnter(aiController);
#if UNITY_EDITOR
				if(aiController.DebugMode)
				{
					if(m_breakpoint.Has(Breakpoint.OnEnter))
					{
						Debug.Break();
					}
				}
#endif
			}

			m_status = OnExecute(aiController);

			if(m_status != BehaviourNodeStatus.Running)
			{
				OnExit(aiController);
#if UNITY_EDITOR
				if(aiController.DebugMode)
				{
					if((m_status == BehaviourNodeStatus.Success && m_breakpoint.Has(Breakpoint.OnSuccess)) ||
						(m_status == BehaviourNodeStatus.Failure && m_breakpoint.Has(Breakpoint.OnFailure)) ||
						m_breakpoint.Has(Breakpoint.OnExit))
					{
						Debug.Break();
					}
				}
#endif
			}

			return m_status;
		}
	}
}
