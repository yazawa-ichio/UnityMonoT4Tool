using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ILib.UnityMonoT4Tool
{

	public class T4ToolSettingsProvider : SettingsProvider
	{
		const string SettingPath = "ProjectSettings/ILib.T4Tool.json";

		[SettingsProvider]
		static SettingsProvider Create()
		{
			var path = "Project/T4Tool";
			var provider = new T4ToolSettingsProvider(path, SettingsScope.Project);

			GenerateSetting settings = ScriptableObject.CreateInstance<GenerateSetting>();
			if (File.Exists(SettingPath))
			{
				try
				{
					JsonUtility.FromJsonOverwrite(File.ReadAllText(SettingPath), settings);
				}
				catch (Exception error)
				{
					Debug.LogException(error);
				}
			}
			provider.m_Settings = settings;
			provider.m_SerializedObject = new SerializedObject(settings);
			provider.keywords = GetSearchKeywordsFromSerializedObject(provider.m_SerializedObject);
			return provider;
		}

		GenerateSetting m_Settings;
		SerializedObject m_SerializedObject;

		public T4ToolSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
		{
		}

		public override void OnGUI(string searchContext)
		{
			EditorGUILayout.LabelField("CommonSetting Generate");

			var property = m_SerializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				EditorGUILayout.PropertyField(property, true);
			}
			m_SerializedObject.ApplyModifiedProperties();
			if (GUILayout.Button("Generate"))
			{
				new Generator(m_Settings).Run();
			}
			if (GUILayout.Button("Dryrun"))
			{
				new Generator(m_Settings).Run(dryrun: true);
			}
			if (GUILayout.Button("Save"))
			{
				Save();
			}
		}

		void Save()
		{
			File.WriteAllText(SettingPath, JsonUtility.ToJson(m_Settings, true));
		}


	}

}