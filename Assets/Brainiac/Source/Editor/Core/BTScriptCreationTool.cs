using UnityEngine;
using UnityEditor;

namespace BrainiacEditor
{
	public class BTScriptCreationTool
	{
		private enum ScriptType
		{
			Composite, Decorator, Action, Constraint, Service
		}

		private static void CreateScript(ScriptType scriptType, string defaultFilename = "MyScript")
		{
			string path = EditorUtility.SaveFilePanel("Save script", "", defaultFilename, "cs");
			if(!string.IsNullOrEmpty(path))
			{
				string scriptName = System.IO.Path.GetFileNameWithoutExtension(path);
				using(var writer = System.IO.File.CreateText(path))
				{
					writer.Write(GenerateScriptContent(scriptType, scriptName));
				}

				AssetDatabase.Refresh();
			}
		}

		private static string GenerateScriptContent(ScriptType scriptType, string scriptName)
		{
			string content = string.Empty;

			try
			{
				TextAsset template = null;
				switch(scriptType)
				{
				case ScriptType.Composite:
					template = Resources.Load<TextAsset>("Brainiac/Templates/Composite");
					break;
				case ScriptType.Decorator:
					template = Resources.Load<TextAsset>("Brainiac/Templates/Decorator");
					break;
				case ScriptType.Action:
					template = Resources.Load<TextAsset>("Brainiac/Templates/Action");
					break;
				case ScriptType.Constraint:
					template = Resources.Load<TextAsset>("Brainiac/Templates/Constraint");
					break;
				case ScriptType.Service:
					template = Resources.Load<TextAsset>("Brainiac/Templates/Service");
					break;
				}

				if(template != null)
					content = string.Format(template.text, scriptName);
			}
			catch(System.Exception ex)
			{
				Debug.LogException(ex);
				content = string.Empty;
			}

			return content;
		}

		[MenuItem("Assets/Create/Brainiac/Composite")]
		private static void CreateCompositeScript()
		{
			CreateScript(ScriptType.Composite, "MyComposite");
		}

		[MenuItem("Assets/Create/Brainiac/Decorator")]
		private static void CreateDecoratorScript()
		{
			CreateScript(ScriptType.Decorator, "MyDecorator");
		}

		[MenuItem("Assets/Create/Brainiac/Action")]
		private static void CreateActionScript()
		{
			CreateScript(ScriptType.Action, "MyAction");
		}

		[MenuItem("Assets/Create/Brainiac/Constraint")]
		private static void CreateConstraintScript()
		{
			CreateScript(ScriptType.Constraint, "MyConstraint");
		}

		[MenuItem("Assets/Create/Brainiac/Service")]
		private static void CreateServiceScript()
		{
			CreateScript(ScriptType.Service, "MyService");
		}
	}
}