using UnityEngine;
using UnityEditor;
using Brainiac;
using UnityEditor.Callbacks;

namespace BrainiacEditor
{
	public class BehaviourTreeEditor : EditorWindow
	{
		[SerializeField]
		private Texture m_gridTexture;
		[SerializeField]
		private GUISkin m_editorSkin;
		[SerializeField]
		private BTAsset m_btAsset;

		private BTEditorGrid m_grid;
		private BTEditorGraph m_graph;
		private BTEditorCanvas m_canvas;
		private bool m_isDisposed;

		private void OnEnable()
		{
			if(m_gridTexture == null)
			{
				m_gridTexture = Resources.Load<Texture>("Brainiac/background");
			}
			if(m_editorSkin == null)
			{
				m_editorSkin = Resources.Load<GUISkin>("Brainiac/editor_style");
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
			if(m_grid == null)
			{
				m_grid = new BTEditorGrid(m_gridTexture);
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
				BTEditorStyle.EnsureStyle(m_editorSkin);
				m_grid.DrawGUI();
				m_graph.DrawGUI();
				m_canvas.HandleEvents(this);
			}
		}

		public static void Open(BTAsset behaviourTree)
		{
			var window = EditorWindow.GetWindow<BehaviourTreeEditor>("Braniac");
			window.SetBTAsset(behaviourTree);
		}

		//[OnOpenAsset(0)]
		//private static bool Open(int instanceID, int line)
		//{
		//	var asset = EditorUtility.InstanceIDToObject(instanceID);
		//	if(asset is BTAsset)
		//	{
		//		Open(asset as BTAsset);
		//		return true;
		//	}

		//	return false;
		//}
	}
}