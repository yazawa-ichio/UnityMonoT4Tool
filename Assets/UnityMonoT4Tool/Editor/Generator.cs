using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using ProcessWindowStyle = System.Diagnostics.ProcessWindowStyle;

namespace ILib.UnityMonoT4Tool
{
	public class Generator
	{
		GenerateSetting m_Settings;
		public Generator(GenerateSetting setting)
		{
			m_Settings = setting;
		}

		public void Run(bool dryrun = false)
		{
			var baseArgs = GetBaseArguments(dryrun);
			string root = Path.Combine(Application.dataPath, m_Settings.InputRootDirectory ?? "");
			var paths = Directory.GetFiles(root, "*.tt", System.IO.SearchOption.AllDirectories);
			try
			{
				for (int i = 0; i < paths.Length; i++)
				{
					EditorUtility.DisplayProgressBar("UnityMonoT4Tool " + i + "/" + paths.Length, "Generate " + paths[i], (float)i / paths.Length);
					var args = baseArgs.Replace("{INPUT}", paths[i]);
					Debug.Log("generate " + paths[i]);
					StartProcess(args);
				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
			if (!dryrun)
			{
				AssetDatabase.Refresh();
			}
		}

		string GetBaseArguments(bool dryrun)
		{
			var builder = new StringBuilder();
			builder.Append(" \"{INPUT} \" ");
			var assemblies = new HashSet<string>(m_Settings.RefAssemblies);
			if (m_Settings.AutoRefAssemblies)
			{
				// TODO:プロジェクトに紐づくものだけにする
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						assemblies.Add(assembly.Location);
					}
					catch { }
				}
			}
			foreach (var assembly in assemblies)
			{
				builder.Append($"-r \"{assembly}\" ");
			}
			foreach (var _using in m_Settings.Using)
			{
				builder.Append($"-u \"{_using}\" ");
			}
			foreach (var prm in m_Settings.Parameters)
			{
				builder.Append($"-p {prm.Key}:{prm.Value} ");
			}
			if (dryrun)
			{
				builder.Append("-o - ");
			}
			return builder.ToString();
		}

		void StartProcess(string args)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo()
			{
				FileName = "t4",
				Arguments = args,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8,
				WorkingDirectory = UnityEngine.Application.dataPath,
			};
			using (Process process = Process.Start(startInfo))
			{
				process.WaitForExit();

				var output = process.StandardOutput.ReadToEnd();
				var error = process.StandardError.ReadToEnd();
				if (!string.IsNullOrEmpty(output))
				{
					UnityEngine.Debug.Log(output);
				}
				if (!string.IsNullOrEmpty(error))
				{
					UnityEngine.Debug.LogError(error);
				}
			}
		}

	}

}