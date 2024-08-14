using UnityEngine;
using static DungeonGenerator;

public sealed class DungeonSpecialRoom : DungeonRoom
{
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private MaterialEntry[] materialEntries;

    public override void SetRoomType(ERoomType _eRoomType)
    {
        for (int i = 0; i < materialEntries.Length; i++)
        {
            if (_eRoomType == materialEntries[i].ERoomType)
            {
                objectRenderer.sharedMaterial = materialEntries[i].Material;
                break;
            }
        }
    }

    [System.Serializable]
    private class MaterialEntry
    {
        public ERoomType ERoomType;
        public Material Material;
    }
}
