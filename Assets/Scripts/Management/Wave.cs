namespace VideoWars.Management
{
	[System.Serializable]
	public class Wave
	{
		public WaveObjectData[] enemies;

		public int GetTotalEnemies()
		{
			int ret = 0;
			for(int i=0;i<enemies.Length;++i)
			{
				ret+=enemies[i].number;
			}
			return ret;
		}
	}
}