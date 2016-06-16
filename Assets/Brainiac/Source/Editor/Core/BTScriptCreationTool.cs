using UnityEditor;
using System.Text;

namespace BrainiacEditor
{
	public class BTScriptCreationTool
	{
		private enum ScriptType
		{
			Composite, Decorator, Action, Constraint, Service
		}

		private static StringBuilder m_stringBuilder;

		static BTScriptCreationTool()
		{
			m_stringBuilder = new StringBuilder();
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
			m_stringBuilder.Length = 0;
			m_stringBuilder.AppendLine("using UnityEngine;");
			m_stringBuilder.AppendLine("using System;");
			m_stringBuilder.AppendLine("using Brainiac;");
			m_stringBuilder.AppendLine();

			if(scriptType == ScriptType.Composite || scriptType == ScriptType.Decorator || scriptType == ScriptType.Action)
			{
				m_stringBuilder.AppendFormat("[AddNodeMenu(\"{0}/{1}\")]\n", scriptType.ToString(), scriptName);
			}
			else if(scriptType == ScriptType.Constraint)
			{
				m_stringBuilder.AppendFormat("[AddConstraintMenu(\"{0}\")]\n", scriptName);
			}
			else if(scriptType == ScriptType.Service)
			{
				m_stringBuilder.AppendFormat("[AddServiceMenu(\"{0}\")]\n", scriptName);
			}

			m_stringBuilder.AppendFormat("public class {0} : Brainiac.{1}\n", scriptName, scriptType.ToString());
			m_stringBuilder.AppendLine("{");

			if(scriptType == ScriptType.Composite || scriptType == ScriptType.Decorator || scriptType == ScriptType.Action)
			{
				m_stringBuilder.AppendLine("\tprotected override BehaviourNodeStatus OnExecute(AIAgent agent)");
				m_stringBuilder.AppendLine("\t{");
				m_stringBuilder.AppendLine("\t\tthrow new NotImplementedException();");
				m_stringBuilder.AppendLine("\t}");
			}
			else if(scriptType == ScriptType.Constraint)
			{
				m_stringBuilder.AppendLine("\tpublic override bool OnEvaluate(AIAgent agent)");
				m_stringBuilder.AppendLine("\t{");
				m_stringBuilder.AppendLine("\t\tthrow new NotImplementedException();");
				m_stringBuilder.AppendLine("\t}");
			}
			else if(scriptType == ScriptType.Service)
			{
				m_stringBuilder.AppendLine("\tpublic override void OnExecute(AIAgent agent)");
				m_stringBuilder.AppendLine("\t{");
				m_stringBuilder.AppendLine("\t\tthrow new NotImplementedException();");
				m_stringBuilder.AppendLine("\t}");
			}

			m_stringBuilder.AppendLine("}");

			return m_stringBuilder.ToString();
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