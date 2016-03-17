using Newtonsoft.Json;
using UnityEngine;

namespace Brainiac
{
	public class BehaviourTree
	{
		private Root m_root;
		
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
			m_root = new Root();
		}
	}
}
