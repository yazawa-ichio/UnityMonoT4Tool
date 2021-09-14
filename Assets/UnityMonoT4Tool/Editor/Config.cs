using System;

namespace ILib.UnityMonoT4Tool
{
	[Serializable]
	public class Config
	{
		[Serializable]
		public class Parameter
		{
			public string Key;
			public string Value;
		}

		public bool AutoInputOutput = true;
		public string InputDir;

		public bool AutoRefAssemblies = true;
		public string[] RefAssemblies = Array.Empty<string>();

		public string[] Using = Array.Empty<string>();
		public Parameter[] Parameters = Array.Empty<Parameter>();
	}

}