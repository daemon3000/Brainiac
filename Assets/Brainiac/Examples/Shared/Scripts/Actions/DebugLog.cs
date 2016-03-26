using UnityEngine;

namespace Brainiac
{
	public enum LogLevel
	{
		Info, Warning, Error
	}

	[AddNodeMenu("Action/Debug")]
	public class DebugLog : Action
	{
		private LogLevel m_level;
		private string m_message;

		[BTProperty]
		public LogLevel Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}

		[BTProperty]
		public string Message
		{
			get
			{
				return m_message;
			}
			set
			{
				m_message = value;
			}
		}

		public override string Title
		{
			get
			{
				return "Debug";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController aiController)
		{
			switch (m_level)
			{
				case LogLevel.Info:
					Debug.Log(m_message);
					break;
				case LogLevel.Warning:
					Debug.LogWarning(m_message);
					break;
				case LogLevel.Error:
					Debug.LogError(m_message);
					break;
			}

			return BehaviourNodeStatus.Success;
		}
	}
}
