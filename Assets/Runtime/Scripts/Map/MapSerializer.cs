using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using Sirenix.OdinSerializer;

namespace UTG.Map
{
    /// <summary> Map serializer both for saving and loading tilemaps. </summary>
    public class MapSerializer
    {
        /// <summary> Save the Unity Tilemap to file. </summary>
        /// <param name="map"> The tilemap to save. </param>
        /// <param name="path"> The path to save the map to. </param>
        public static void SaveMap(string path, Tilemap map, Dictionary<Vector3, WorldTile> tiles)
        {
            path = Path.Combine(path, GameManager.instance.SceneName);

            if(!Directory.Exists(path)){
                Directory.CreateDirectory (path); // Create the directory if it doesn't exist
                Debug.Log("Created directory: " + path);
            }
            
            path = Path.Combine(path, "Maps"); //path = C:\Users\User\AppData\LocalLow\HumbleRPG\Saves\save\scene\Maps\
        
            if(!Directory.Exists(path)){
                Directory.CreateDirectory (path); // Create the directory if it doesn't exist
                Debug.Log("Created directory: " + path);
            }

            path = Path.Combine(path, map.name + ".humble"); //path = C:\Users\User\AppData\LocalLow\HumbleRPG\Saves\save\scene\Maps\mapname.humble

            byte[] saveJson = SerializationUtility.SerializeValue(tiles, DataFormat.JSON); //Serialize the state to JSON
            File.WriteAllBytes(path, saveJson); //Save the state to a file
        }

        /// <summary> Load the Unity Tilemap from file. </summary>
        /// <param name="map"> The tilemap to load. </param>
        /// <param name="path"> The path to load the map from. </param>
        public static  Dictionary<Vector3, WorldTile> LoadMap(string mapName, string path, Tilemap map, Dictionary<Vector3, WorldTile> tiles)
        {
            path = Path.Combine(path, mapName + ".humble"); //path = C:\Users\User\AppData\LocalLow\HumbleRPG\Saves\save\scene\Maps\mapname.humble

            if (!File.Exists(path))
            {
                Debug.LogError("File does not exist");
                return null;
            } 

            byte[] loadJson = File.ReadAllBytes(path); //loadJson = byte[]
            tiles = SerializationUtility.DeserializeValue<Dictionary<Vector3, WorldTile>>(loadJson, DataFormat.JSON); //DeserializeValue<T> is a generic method that takes in a byte array and a data format and returns a T.

            Tile[] tileAsset = Resources.LoadAll<Tile>("Tilemap"); //Load all the tiles from the resources folder
            map.ClearAllTiles(); //Clear the tilemap

            //Loop through the tiles and add them to the tilemap
            foreach (WorldTile tile in tiles.Values)
            {
                //Get the tile from the tile asset
                for(int i = 0; i <= tileAsset.Length; i++)
                {
                    //If the tileasset name matches the tile name
                    if(tileAsset[i].name == tile.tileBase)
                    {
                        map.SetTile(tile.localPlace, tileAsset[i]); //Set the tile at the position
                        map.SetTileFlags(tile.localPlace, TileFlags.None); //Set the tile flags to none

                        if(mapName == "FogMap")
                        {
                            switch(tile.isExplored && !tile.isVisible)
                            {
                                case true:
                                    map.SetColor(tile.localPlace, new Color(tile.color.r, tile.color.g, tile.color.b, 0.5f)); //Set the color of the tile to 50% transparent
                                    break;
                                case false:
                                    map.SetColor(tile.localPlace, tile.color); //Set the color of the tile
                                    break;
                            }
                            break;
                        }
                        i = tileAsset.Length; //Break out of the loop
                    }
                }
            }
            Resources.UnloadUnusedAssets(); //Unload the unused assets
            return tiles; //Return the tiles
        }
    }
}
