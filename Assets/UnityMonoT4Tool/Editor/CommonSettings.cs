using UnityEngine;

namespace ILib.UnityMonoT4Tool
{
	public class CommonSettings : ScriptableObject
	{
		[SerializeField]
		Config m_Config = new Config();
		public Config Config => m_Config;
	}
}