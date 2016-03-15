using UnityEngine;
using System;
using System.Collections;
using UnityEditor;

namespace BrainiacEditor
{
	public class BTEditorGrid
	{
		private Texture m_gridTexture;

		public BTEditorGrid(Texture gridTexture)
		{
			m_gridTexture = gridTexture;
		}

		public void DrawGUI()
		{
			BTEditorCanvas canvas = BTEditorCanvas.Current;
			float width = Mathf.Max(m_gridTexture.width, canvas.Size.x);
			float height = Mathf.Max(m_gridTexture.height, canvas.Size.y);
			Rect position = new Rect(canvas.Position.x, canvas.Position.y, width, height);
			Rect texCoords = new Rect(0.0f, 0.0f, width / m_gridTexture.width, height / m_gridTexture.height);

			GUI.DrawTextureWithTexCoords(position, m_gridTexture, texCoords);
		}
	}
}