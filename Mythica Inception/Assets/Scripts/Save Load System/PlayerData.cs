using _Core.Player;

namespace Save_Load_System
{
    [System.Serializable]
    public class PlayerData
    {
        public float[] position;
        public PlayerData(Player player)
        {
            position = new float[3];
            position[0] = player.transform.position.x;
            position[1] = player.transform.position.y;
            position[2] = player.transform.position.z;
        }
    }
}
