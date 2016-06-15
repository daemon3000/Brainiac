using UnityEngine;
using Brainiac.Serialization;
using System.Collections.Generic;

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

		[BTProperty]
		private List<Condition> m_conditions;
		[BTProperty]
		private List<Service> m_services;

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

#if UNITY_EDITOR
		/// <summary>
		/// This node's list of conditions. For use only in the editor code. DO NOT USE IN RUNTIME CODE!!!
		/// </summary>
		[BTIgnore]
		public List<Condition> Conditions
		{
			get { return m_conditions; }
		}

		/// <summary>
		/// This node's list of services. For use only in the editor code. DO NOT USE IN RUNTIME CODE!!!
		/// </summary>
		[BTIgnore]
		public List<Service> Services
		{
			get { return m_services; }
		}
#endif

		public BehaviourNode()
		{
			m_breakpoint = Breakpoint.None;
			m_status = BehaviourNodeStatus.None;
			m_conditions = new List<Condition>();
			m_services = new List<Service>();
		}

		public virtual void OnBeforeSerialize(BTAsset btAsset)
		{
			foreach(var condition in m_conditions)
				condition.OnBeforeSerialize(btAsset);
			foreach(var service in m_services)
				service.OnBeforeSerialize(btAsset);
		}

		public virtual void OnAfterDeserialize(BTAsset btAsset)
		{
			if(m_conditions == null)
				m_conditions = new List<Condition>();
			if(m_services == null)
				m_services = new List<Service>();

			foreach(var condition in m_conditions)
				condition.OnAfterDeserialize(btAsset);
			foreach(var service in m_services)
				service.OnAfterDeserialize(btAsset);
		}

		public virtual void OnStart(AIAgent agent)
		{
		}

		public virtual void OnReset()
		{
			m_status = BehaviourNodeStatus.None;
		}

		public BehaviourNodeStatus Run(AIAgent agent)
		{
			if(m_status != BehaviourNodeStatus.Running)
			{
				OnEnter(agent);

				foreach(var service in m_services)
					service.OnEnter(agent);
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

			if(EvaluateConditions(agent))
			{
				m_status = OnExecute(agent);

				foreach(var service in m_services)
					service.OnExecute(agent);
			}
			else
			{
				m_status = BehaviourNodeStatus.Failure;
			}

			if(m_status != BehaviourNodeStatus.Running)
			{
				OnExit(agent);

				foreach(var service in m_services)
					service.OnExit(agent);
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

		protected abstract BehaviourNodeStatus OnExecute(AIAgent agent);

		protected virtual void OnEnter(AIAgent agent)
		{
		}

		protected virtual void OnExit(AIAgent agent)
		{
		}

		private bool EvaluateConditions(AIAgent agent)
		{
			foreach(var condition in m_conditions)
			{
				if(!condition.OnEvaluate(agent))
					return false;
			}

			return true;
		}
	}
}
