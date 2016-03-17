using UnityEngine;
using System;
using System.Collections;
using Brainiac;

namespace BrainiacEditor
{
	public class BTGraphNodeStyle
	{
		private GUIStyle m_standardNormalStyle;
		private GUIStyle m_standardSelectedStyle;
		private GUIStyle m_failNormalStyle;
		private GUIStyle m_failSelectedStyle;
		private GUIStyle m_runninfNormalStyle;
		private GUIStyle m_runningSelectedStyle;
		private GUIStyle m_successNormalStyle;
		private GUIStyle m_successSelectedStyle;

		private string m_standardNormalStyleName;
		private string m_standardSelectedStyleName;
		private string m_failNormalStyleName;
		private string m_failSelectedStyleName;
		private string m_runninfNormalStyleName;
		private string m_runningSelectedStyleName;
		private string m_successNormalStyleName;
		private string m_successSelectedStyleName;

		public BTGraphNodeStyle(string standardNormalStyleName, string standardSelectedStyleName, 
								string failNormalStyleName, string failSelectedStyleName,
								string runninfNormalStyleName, string runningSelectedStyleName, 
								string successNormalStyleName, string successSelectedStyleName)
		{
			m_standardNormalStyleName = standardNormalStyleName;
			m_standardSelectedStyleName = standardSelectedStyleName;
			m_failNormalStyleName = failNormalStyleName;
			m_failSelectedStyleName = failSelectedStyleName;
			m_runninfNormalStyleName = runninfNormalStyleName;
			m_runningSelectedStyleName = runningSelectedStyleName;
			m_successNormalStyleName = successNormalStyleName;
			m_successSelectedStyleName = successSelectedStyleName;

			EnsureStyle();
		}

		public void EnsureStyle()
		{
			if(m_standardNormalStyle == null)
			{
				m_standardNormalStyle = (GUIStyle)m_standardNormalStyleName;
			}
			if(m_standardSelectedStyle == null)
			{
				m_standardSelectedStyle = (GUIStyle)m_standardSelectedStyleName;
			}

			if(m_failNormalStyle == null)
			{
				m_failNormalStyle = (GUIStyle)m_failNormalStyleName;
			}
			if(m_failSelectedStyle == null)
			{
				m_failSelectedStyle = (GUIStyle)m_failSelectedStyleName;
			}

			if(m_runninfNormalStyle == null)
			{
				m_runninfNormalStyle = (GUIStyle)m_runninfNormalStyleName;
			}
			if(m_runningSelectedStyle == null)
			{
				m_runningSelectedStyle = (GUIStyle)m_runningSelectedStyleName;
			}

			if(m_successNormalStyle == null)
			{
				m_successNormalStyle = (GUIStyle)m_successNormalStyleName;
			}
			if(m_successSelectedStyle == null)
			{
				m_successSelectedStyle = (GUIStyle)m_successSelectedStyleName;
			}
		}

		public GUIStyle GetStyle(BehaviourNodeStatus? status, bool isSelected)
		{
			if(status.HasValue)
			{
				switch(status)
				{
				case BehaviourNodeStatus.Failure:
					return !isSelected ? m_failNormalStyle : m_failSelectedStyle;
				case BehaviourNodeStatus.Running:
					return !isSelected ? m_runninfNormalStyle : m_runningSelectedStyle;
				case BehaviourNodeStatus.Success:
					return !isSelected ? m_successNormalStyle : m_successSelectedStyle;
				}
			}

			return !isSelected ? m_standardNormalStyle : m_standardSelectedStyle;
		}
	}
}