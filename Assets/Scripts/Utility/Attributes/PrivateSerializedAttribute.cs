using UnityEngine;

namespace VideoWars.Utility.Attributes
{
	public class PrivateSerializedAttribute : PropertyAttribute
	{
		public readonly string name;

		public PrivateSerializedAttribute(string name)
		{
			this.name = name;
		}
	}
}