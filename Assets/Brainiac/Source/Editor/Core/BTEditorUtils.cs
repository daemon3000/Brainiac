using UnityEngine;
using UnityEditor;

namespace BrainiacEditor
{
	public static class BTEditorUtils
	{
		private const int BEZIER_H_OFFSET = 150;
		private const int BEZIER_WIDTH = 3;

		public static void DrawBezier(Rect a, Rect b, Color color)
		{
			Handles.DrawBezier(a.center, b.center,
							   new Vector3(a.center.x + BEZIER_H_OFFSET, a.center.y, 0),
							   new Vector3(b.center.x - BEZIER_H_OFFSET, b.center.y, 0),
							   color, null, BEZIER_WIDTH);
		}

		public static void DrawBezier(Vector2 a, Vector2 b, Color color)
		{
			Handles.DrawBezier(a, b, new Vector3(a.x + BEZIER_H_OFFSET, a.y, 0),
							   new Vector3(b.x - BEZIER_H_OFFSET, b.y, 0),
							   color, null, BEZIER_WIDTH);
		}

		public static void DrawLine(Rect a, Rect b, Color color)
		{
			Handles.DrawBezier(a.center, b.center, a.center, b.center, color, null, BEZIER_WIDTH);
		}

		public static void DrawLine(Vector2 a, Vector2 b, Color color)
		{
			Handles.DrawBezier(a, b, a, b, color, null, BEZIER_WIDTH);
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