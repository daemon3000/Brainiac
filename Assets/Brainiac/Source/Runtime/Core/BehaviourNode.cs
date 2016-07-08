using UnityEngine;
using Brainiac.Serialization;
using System.Collections.Generic;

namespace Brainiac
{
	public abstract class BehaviourNode
	{
		[BTProperty("__UniqueID")]
		private string m_uniqueID;

		private Vector2 m_position;
		private string m_comment;
		private string m_name;
		private float m_weight;
		private Breakpoint m_breakpoint;
		private BehaviourNodeStatus m_status;

		[BTProperty("Constraints")]
		private List<Constraint> m_constraints;
		[BTProperty("Services")]
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

		[BTIgnore]
		public string UniqueID
		{
			get { return m_uniqueID; }
		}

#if UNITY_EDITOR
		/// <summary>
		/// This node's list of Constraints. For use only in the editor code. DO NOT USE IN RUNTIME CODE!!!
		/// </summary>
		[BTIgnore]
		public List<Constraint> Constraints
		{
			get { return m_constraints; }
		}

		/// <summary>
		/// This node's list of Services. For use only in the editor code. DO NOT USE IN RUNTIME CODE!!!
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
			m_constraints = new List<Constraint>();
			m_services = new List<Service>();
		}

		public virtual void OnBeforeSerialize(BTAsset btAsset)
		{
			foreach(var constraint in m_constraints)
				constraint.OnBeforeSerialize(btAsset);
			foreach(var service in m_services)
				service.OnBeforeSerialize(btAsset);
		}

		public virtual void OnAfterDeserialize(BTAsset btAsset)
		{
			if(m_constraints == null)
				m_constraints = new List<Constraint>();
			if(m_services == null)
				m_services = new List<Service>();

			foreach(var constraint in m_constraints)
				constraint.OnAfterDeserialize(btAsset);
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

			if(CheckConstraints(agent))
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

		private bool CheckConstraints(AIAgent agent)
		{
			foreach(var constraint in m_constraints)
			{
				if(!constraint.OnExecute(agent))
					return false;
			}

			return true;
		}
	}
}
