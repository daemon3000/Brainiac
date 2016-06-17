using UnityEngine;
using UnityEditor;
using System.Text;

namespace BrainiacEditor
{
	public static class BTEditorUtils
	{
		private const int BEZIER_H_OFFSET = 250;
		private const int BEZIER_WIDTH = 3;
		private const float MIN_V_DISTANCE = 40.0f;
		private const float MIN_H_DISTANCE = 300.0f;

		private static StringBuilder m_stringBuilder = new StringBuilder();

		public static void DrawBezier(Rect a, Rect b, Color color)
		{
			Vector2 start = Vector3.zero;
			Vector2 end = Vector3.zero;

			if(a.center.x <= b.center.x)
			{
				start = a.center;
				end = b.center;
			}
			else
			{
				start = b.center;
				end = a.center;
			}

			float vertDistance = Mathf.Abs(start.y - end.y);
			float horzDistance = Mathf.Abs(start.x - end.x);
			float lerp = Mathf.Min(Mathf.Clamp01(vertDistance / MIN_V_DISTANCE), Mathf.Clamp01(horzDistance / MIN_H_DISTANCE));
			float offset = Mathf.Lerp(0.0f, BEZIER_H_OFFSET, lerp);

			Vector3 startTangent = new Vector3(start.x + offset, start.y, 0);
			Vector3 endTangent = new Vector3(end.x - offset, end.y, 0);

			Handles.DrawBezier(start, end, startTangent, endTangent, color, null, BEZIER_WIDTH);
		}

		public static void DrawBezier(Vector2 a, Vector2 b, Color color)
		{
			Vector2 start = a.x <= b.x ? a : b;
			Vector2 end = a.x <= b.x ? b : a;

			float vertDistance = Mathf.Abs(start.y - end.y);
			float horzDistance = Mathf.Abs(start.x - end.x);
			float lerp = Mathf.Min(Mathf.Clamp01(vertDistance / MIN_V_DISTANCE), Mathf.Clamp01(horzDistance / MIN_H_DISTANCE));
			float offset = Mathf.Lerp(0.0f, BEZIER_H_OFFSET, lerp);

			Vector3 startTangent = new Vector3(start.x + offset, start.y, 0);
			Vector3 endTangent = new Vector3(end.x - offset, end.y, 0);

			Handles.DrawBezier(start, end, startTangent, endTangent, color, null, BEZIER_WIDTH);
		}

		public static void DrawLine(Rect a, Rect b, Color color)
		{
			Handles.DrawBezier(a.center, b.center, a.center, b.center, color, null, BEZIER_WIDTH);
		}

		public static void DrawLine(Vector2 a, Vector2 b, Color color)
		{
			Handles.DrawBezier(a, b, a, b, color, null, BEZIER_WIDTH);
		}

		public static string MakePrettyName(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new System.ArgumentException("Name is null or empty", "name");

			int startIndex = 0;

			if(name.StartsWith("m_", System.StringComparison.InvariantCultureIgnoreCase))
				startIndex = 2;

			m_stringBuilder.Length = 0;
			for(int i = startIndex; i < name.Length; i++)
			{
				if(!char.IsLetterOrDigit(name[i]))
				{
					if(m_stringBuilder.Length > 0)
						m_stringBuilder.Append(" ");

					continue;
				}

				if(m_stringBuilder.Length == 0)
				{
					m_stringBuilder.Append(char.ToUpper(name[i]));
				}
				else
				{
					if(i > startIndex)
					{
						if((char.IsUpper(name[i]) && !char.IsUpper(name[i - 1])) ||
							(char.IsLetter(name[i]) && char.IsDigit(name[i - 1])) ||
							(char.IsDigit(name[i]) && char.IsLetter(name[i - 1])))
						{
							m_stringBuilder.Append(" ");
						}
					}

					m_stringBuilder.Append(name[i]);
				}
			}

			return m_stringBuilder.Length > 0 ? m_stringBuilder.ToString() : name;
		}

		public static string GetResourcePath(UnityEngine.Object target)
		{
			string finalPath = "";

			if(target != null)
			{
				string path = AssetDatabase.GetAssetPath(target);
				int i = path.IndexOf("Resources");

				if(i >= 0 && i + 10 < path.Length)
				{
					path = path.Substring(i + 10);
					i = path.LastIndexOf('.');
					if(i > 0)
					{
						path = path.Substring(0, i);
					}

					finalPath = path;
				}
			}

			return finalPath;
		}
	}
}