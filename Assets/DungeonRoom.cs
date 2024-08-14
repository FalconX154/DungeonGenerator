using UnityEngine;

public partial class DungeonGenerator
{
    public abstract class DungeonRoom : MonoBehaviour
    {
        public abstract void SetRoomType(ERoomType _eRoomType);
    }
}
