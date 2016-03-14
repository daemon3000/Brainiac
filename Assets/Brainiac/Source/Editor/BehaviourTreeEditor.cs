using UnityEngine;
using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	public class BehaviourTreeEditor : EditorWindow
	{
		[SerializeField]
		private Texture m_grid;
		[SerializeField]
		private BTAsset m_btAsset;

		private BTEditorGraph m_graph;
		private BTEditorCanvas m_canvas;
		private bool m_isDisposed;

		private void OnEnable()
		{
			if(m_grid == null)
			{
				m_grid = Resources.Load<Texture>("Brainiac/background");
			}
			if(m_graph == null)
			{
				m_graph = BTEditorGraph.Create();
			}
			if(m_canvas == null)
			{
				m_canvas = new BTEditorCanvas();
				BTEditorCanvas.Current = m_canvas;
			}

			if(m_btAsset != null)
			{
				m_btAsset.Dispose();
				m_btAsset.Deserialize();
				m_graph.SetBehaviourTree(m_btAsset.BehaviourTree);
				m_canvas.Position = m_btAsset.CanvasPosition;
				m_canvas.Size = m_btAsset.CanvasSize;
			}

			m_isDisposed = false;
			m_canvas.OnRepaint += Repaint;
		}

		private void OnDisable()
		{
			Disposed();
		}

		private void OnDestroy()
		{
			Disposed();
		}

		private void Disposed()
		{
			if(!m_isDisposed)
			{
				if(m_graph != null)
				{
					BTEditorGraph.DestroyImmediate(m_graph);
					m_graph = null;
				}
				if(m_btAsset != null)
				{
					m_btAsset.CanvasPosition = m_canvas.Position;
					m_btAsset.CanvasSize = m_canvas.Size;
					m_btAsset.Serialize();
					m_btAsset.Dispose();
					EditorUtility.SetDirty(m_btAsset);
				}

				m_isDisposed = true;
			}
		}

		private void SetBTAsset(BTAsset asset)
		{
			if(asset != m_btAsset)
			{
				if(m_btAsset != null)
				{
					m_btAsset.Serialize();
					m_btAsset.Dispose();
					EditorUtility.SetDirty(m_btAsset);
				}

				m_btAsset = asset;
				if(m_btAsset != null)
				{
					m_btAsset.Dispose();
					m_btAsset.Deserialize();
					m_graph.SetBehaviourTree(m_btAsset.BehaviourTree);
					m_canvas.Position = m_btAsset.CanvasPosition;
					m_canvas.Size = m_btAsset.CanvasSize;
				}
			}
		}

		private void OnGUI()
		{
			if(m_btAsset != null)
			{
				BTEditorStyle.EnsureStyle();
				DrawGrid();
				m_graph.DrawGUI();
				m_canvas.HandleEvents(this);
				DrawLogo();
			}
		}

		private void DrawGrid()
		{
			float width = Mathf.Max(m_grid.width, m_canvas.Size.x);
			float height = Mathf.Max(m_grid.height, m_canvas.Size.y);
			Rect position = new Rect(m_canvas.Position.x, m_canvas.Position.y, width, height);
			Rect texCoords = new Rect(0.0f, 0.0f, width / m_grid.width, height / m_grid.height);

			GUI.DrawTextureWithTexCoords(position, m_grid, texCoords);
		}

		private void DrawLogo()
		{
			//EditorGUI.LabelField(new Rect(position.width - 260, position.height - 60, 260, 60), "BRAINIAC", BTEditorStyle.LogoStyle);
		}

		public static void Open(BTAsset behaviourTree)
		{
			var window = EditorWindow.GetWindow<BehaviourTreeEditor>("Braniac");
			window.SetBTAsset(behaviourTree);
		}
	}
}