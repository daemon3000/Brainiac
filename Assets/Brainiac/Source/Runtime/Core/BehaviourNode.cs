using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	public abstract class BehaviourNode
	{
		private Vector2 m_position;
		private string m_comment;
		private string m_name;
		private float m_weight;
		private Breakpoint m_breakpoint;
		private BehaviourNodeStatus m_status;

		[BTHideInInspector]
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

		[BTHideInInspector]
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

		[BTHideInInspector]
		public string Comment
		{
			get
			{
				return m_comment;
			}
			set
			{
				m_comment = value;
			}
		}

		[BTHideInInspector]
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

		[BTHideInInspector]
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

		[BTIgnore]
		public virtual string Title
		{
			get { return GetType().Name; }
		}

		[BTIgnore]
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

		public virtual void OnBeforeSerialize(BTAsset btAsset) { }
		public virtual void OnAfterDeserialize(BTAsset btAsset) { }
		public virtual void OnStart(AIAgent agent) { }
		protected virtual void OnEnter(AIAgent agent) { }
		protected virtual void OnExit(AIAgent agent) { }
		protected abstract BehaviourNodeStatus OnExecute(AIAgent agent);

		public virtual void OnReset()
		{
			m_status = BehaviourNodeStatus.None;
		}

		public BehaviourNodeStatus Run(AIAgent agent)
		{
			if(m_status != BehaviourNodeStatus.Running)
			{
				OnEnter(agent);
#if UNITY_EDITOR
				if(agent.DebugMode)
				{
					if(m_breakpoint.Has(Breakpoint.OnEnter))
					{
						Debug.Break();
					}
				}
#endif
			}

			m_status = OnExecute(agent);

			if(m_status != BehaviourNodeStatus.Running)
			{
				OnExit(agent);
#if UNITY_EDITOR
				if(agent.DebugMode)
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
