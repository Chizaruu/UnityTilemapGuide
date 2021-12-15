using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    [Header("Event System")]
    [SerializeField]private EventSystem eventSystem;

    [Header("Level Editor Tool")]
    [SerializeField]private GameObject eraseTool; // This should be named "Erase"
    [SerializeField]private GameObject eraseToolSelected; 

    [Header("Tile Asset Parents")]
    [SerializeField]private GameObject floorTilesParent;
    [SerializeField]private GameObject obstacleTilesParent;

    [Header("Last Selected Button")]
    [SerializeField, ReadOnly]private GameObject lastSelectedGameObject;
    private GameObject currentSelectedGameObject_Recent;

    private void Start() {
        GetTilesFromResources("Floor", floorTilesParent);
        GetTilesFromResources("Obstacle", obstacleTilesParent);
    }

    public void GetLastGameObjectSelected() {
        if (eventSystem.currentSelectedGameObject != currentSelectedGameObject_Recent) {

            lastSelectedGameObject = currentSelectedGameObject_Recent;

            currentSelectedGameObject_Recent = eventSystem.currentSelectedGameObject;
        }
        
        if(lastSelectedGameObject != null) {
            switch (lastSelectedGameObject.name) {
                case "Erase":
                    eraseTool.SetActive(true);
                    eraseToolSelected.SetActive(false);
                    break;
                default:
                    lastSelectedGameObject.transform.GetChild(0).gameObject.SetActive(false);
                    break;
            }
        } 
    }

    private void GetTilesFromResources(string path, GameObject parent) {
        // Get all the floor tiles
        Tile[] tiles = Resources.LoadAll<Tile>("Tilemap/" + path);
        foreach (Tile tile in tiles) {
            GameObject tileButton = Instantiate(Resources.Load<GameObject>("Prefabs/UI/LevelEditor/TileButton"));
            tileButton.transform.SetParent(parent.transform);
            tileButton.transform.localScale = Vector3.one;
            tileButton.transform.localPosition = Vector3.zero;
            tileButton.name = tile.name;
            tileButton.GetComponent<Image>().sprite = tile.sprite;
            
            tileButton.GetComponent<Button>().onClick.AddListener(() => {
                GetLastGameObjectSelected();
            });
        }
    }
}
