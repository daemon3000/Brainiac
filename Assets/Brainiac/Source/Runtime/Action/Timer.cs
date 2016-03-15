using UnityEngine;

namespace Brainiac
{
	[AddBehaviourNodeMenu("Action/Timer")]
	public class Timer : Action
	{
		private float m_duration;

		private float m_startTime;

		public float Duration
		{
			get
			{
				return m_duration;
			}
			set
			{
				m_duration = value;
			}
		}

		public override string Title
		{
			get
			{
				return "Timer";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController ai)
		{
			if(Time.time < m_startTime + m_duration)
				return BehaviourNodeStatus.Running;

			return BehaviourNodeStatus.Success;
		}

		protected override void OnStart(AIController ai)
		{
			m_startTime = Time.time;
		}

		public override void OnGUI()
		{
			base.OnGUI();
#if UNITY_EDITOR
			m_duration = UnityEditor.EditorGUILayout.FloatField("Duration", m_duration);
#endif
		}
	}
}