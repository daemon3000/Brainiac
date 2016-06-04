using UnityEngine;

namespace BrainiacEditor
{
	public class BTEditorGrid
	{
		private Texture m_gridTexture;

		public BTEditorGrid(Texture gridTexture)
		{
			m_gridTexture = gridTexture;
		}

		public void DrawGUI(Vector2 windowSize)
		{
			BTEditorCanvas canvas = BTEditorCanvas.Current;
			Rect position = new Rect(0, 0, windowSize.x, windowSize.y);
			Rect texCoords = new Rect(-canvas.Position.x / m_gridTexture.width,
									  (1.0f - windowSize.y / m_gridTexture.height) + canvas.Position.y / m_gridTexture.height,
									  windowSize.x / m_gridTexture.width,
									  windowSize.y / m_gridTexture.height);

			GUI.DrawTextureWithTexCoords(position, m_gridTexture, texCoords);
		}
	}
}