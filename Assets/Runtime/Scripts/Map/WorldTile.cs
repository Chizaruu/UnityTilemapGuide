using UnityEngine;

namespace UTG.Map
{
    /// <summary> A tile in the world. </summary>
    [System.Serializable]
    public class WorldTile
    {
        public string tileBase; // The base tile name.
        public bool isExplored; // Whether the tile has been explored.
        public bool isVisible; // Whether the tile is visible.
        public Vector3Int localPlace; // The local place of the tile.
        public Vector3 gridLocation; // The grid location of the tile.
        public Color color; // The color of the tile.
    }
}

