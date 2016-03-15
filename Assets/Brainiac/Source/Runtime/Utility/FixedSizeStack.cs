using System;
using System.Collections;
using System.Collections.Generic;

//	Credits: http://ntsblog.homedev.com.au/index.php/2010/05/06/c-stack-with-maximum-limit/
namespace Brainiac
{
	/// <summary>
	/// Generic stack implementation with a maximum limit
	/// When something is pushed on the last item is removed from the list
	/// </summary>
	public class FixedSizeStack<T>
	{
		private int m_limit;
		private LinkedList<T> m_list;

		public FixedSizeStack(int maxSize)
		{
			m_limit = maxSize;
			m_list = new LinkedList<T>();

		}

		public void Push(T value)
		{
			if(m_list.Count == m_limit)
			{
				m_list.RemoveLast();
			}
			m_list.AddFirst(value);
		}

		public T Pop()
		{
			if(m_list.Count > 0)
			{
				T value = m_list.First.Value;
				m_list.RemoveFirst();
				return value;
			}
			else
			{
				throw new InvalidOperationException("The Stack is empty");
			}


		}

		public T Peek()
		{
			if(m_list.Count > 0)
			{
				T value = m_list.First.Value;
				return value;
			}
			else
			{
				throw new InvalidOperationException("The Stack is empty");
			}

		}

		public void Clear()
		{
			m_list.Clear();

		}

		public int Count
		{
			get { return m_list.Count; }
		}

		/// <summary>
		/// Checks if the top object on the stack matches the value passed in
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool IsTop(T value)
		{
			bool result = false;
			if(this.Count > 0)
			{
				result = Peek().Equals(value);
			}
			return result;
		}

		public bool Contains(T value)
		{
			bool result = false;
			if(this.Count > 0)
			{
				result = m_list.Contains(value);
			}
			return result;
		}

		public IEnumerator GetEnumerator()
		{
			return m_list.GetEnumerator();
		}
	}
}