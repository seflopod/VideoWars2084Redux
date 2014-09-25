using UnityEngine;

namespace VideoWars.Utility.Attributes
{
	public class FlagsSelectAttribute : PropertyAttribute
	{
		public readonly string name;
		public FlagsSelectAttribute(string name)
		{
			this.name = name;
		}
	}
}