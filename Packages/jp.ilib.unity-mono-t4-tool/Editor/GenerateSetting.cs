using System;
using UnityEngine;

namespace ILib.UnityMonoT4Tool
{
	public class GenerateSetting : ScriptableObject
	{
		[Serializable]
		public class Parameter
		{
			public string Key;
			public string Value;
		}
		public string InputRootDirectory;
		public bool AutoRefAssemblies = true;
		public string[] RefAssemblies = Array.Empty<string>();
		public string[] Using = Array.Empty<string>();
		public Parameter[] Parameters = Array.Empty<Parameter>();
	}
}