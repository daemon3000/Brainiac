using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	public class BehaviourTree
	{
		[BTProperty("Root")]
		private Root m_root;

		[BTIgnore]
		public Root Root
		{
			get
			{
				return m_root;
			}
		}

		[BTIgnore]
		public bool ReadOnly { get; set; }

		public BehaviourTree()
		{
			if(m_root == null)
			{
				m_root = new Root();
				m_root.Position = new Vector2(0, 0);
			}
		}
	}
}
