using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	public class BehaviourTree
	{
		[JsonMember][JsonName("Root")]
		private Root m_root;

		[JsonIgnore]
		public Root Root
		{
			get
			{
				return m_root;
			}
		}

		[JsonIgnore]
		public bool ReadOnly { get; set; }

		public BehaviourTree()
		{
			if(m_root == null)
			{
				m_root = new Root();
				m_root.Position = new Vector2(50, 50);
			}
		}
	}
}
