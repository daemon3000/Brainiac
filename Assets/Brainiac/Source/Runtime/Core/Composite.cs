using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Brainiac
{
	public abstract class Composite : BehaviourNode
	{
		[JsonProperty(PropertyName = "Children")]
		protected List<BehaviourNode> m_children;

		public override Vector2 Size
		{
			get
			{
				return new Vector2(120, 40);
			}
		}

		[JsonIgnore]
		public int ChildCount
		{
			get
			{
				return m_children.Count;
			}
		}

		public Composite()
		{
			m_children = new List<BehaviourNode>();
		}

		public void AddChild(BehaviourNode child)
		{
			if(child != null)
			{
				m_children.Add(child);
			}
		}

		public void InsertChild(int index, BehaviourNode child)
		{
			if(child != null)
			{
				m_children.Insert(index, child);
			}
		}

		public void RemoveChild(BehaviourNode child)
		{
			if(child != null)
			{
				m_children.Remove(child);
			}
		}

		public void RemoveChild(int index)
		{
			if(index >= 0 && index < m_children.Count)
			{
				m_children.RemoveAt(index);
			}
		}

		public void RemoveAllChildren()
		{
			m_children.Clear();
		}

		public BehaviourNode GetChild(int index)
		{
			if (index >= 0 && index < m_children.Count)
				return m_children[index];
			else
				return null;
		}
	}
}
