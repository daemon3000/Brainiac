using Brainiac.Serialization;

namespace Brainiac
{
	[AddNodeMenu("Action/Run Behaviour")]
	public class RunBehaviour : Action
	{
		[BTProperty("BehaviourTreeID")]
		[BTHideInInspector]
		private string m_behaviourTreeID;

		private BTAsset m_behaviourTreeAsset;
		private BehaviourTree m_behaviourTree;

		[BTIgnore]
		public BTAsset BehaviourTreeAsset
		{
			get { return m_behaviourTreeAsset; }
			set { m_behaviourTreeAsset = value; }
		}

		[BTIgnore]
		public BehaviourTree BehaviourTree
		{
			get
			{
				return m_behaviourTree;
			}
		}

		public override string Title
		{
			get
			{
				return "Run Behaviour";
			}
		}

		public override void OnBeforeSerialize(BTAsset btAsset)
		{
			if(string.IsNullOrEmpty(m_behaviourTreeID))
			{
				m_behaviourTreeID = BTUtils.GenerateUniqueStringID();
			}

			btAsset.SetSubtreeAsset(m_behaviourTreeID, m_behaviourTreeAsset);
		}

		public override void OnAfterDeserialize(BTAsset btAsset)
		{
			m_behaviourTreeAsset = btAsset.GetSubtreeAsset(m_behaviourTreeID);
		}

		public override void OnStart(AIAgent agent)
		{
			if(m_behaviourTreeAsset != null)
			{
				m_behaviourTree = m_behaviourTreeAsset.CreateRuntimeTree();
				if(m_behaviourTree != null)
				{
					m_behaviourTree.Root.OnStart(agent);
				}
			}
		}

		public override void OnReset()
		{
			base.OnReset();
			if(m_behaviourTree != null)
			{
				m_behaviourTree.Root.OnReset();
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			if(m_behaviourTree != null)
			{
				return m_behaviourTree.Root.Run(agent);
			}
			else
			{
				return BehaviourNodeStatus.Failure;
			}
		}
	}
}