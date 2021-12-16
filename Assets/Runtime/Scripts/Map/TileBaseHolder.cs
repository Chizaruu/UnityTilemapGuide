using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBaseHolder : MonoBehaviour{
    public enum TileBaseType{
        Floor, Obstacle, None
    }
    public TileBaseType tileBaseType;
    public TileBase tileBase;
}