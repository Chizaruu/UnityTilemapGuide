using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UTG.Character;

namespace UTG.Map
{
    //Adapted from Velvarys's Tilemap Editing Tutorial (https://www.youtube.com/playlist?list=PLJBcv4t1EiSz-wA35-dWpcI98pNiyK6an)
    public class LevelEditor : MonoBehaviour
    {
        public enum BrushType{
            Single, 
            Line, 
            Rectangle
        }

        [Header("Event System")]
        [SerializeField]private EventSystem eventSystem; // The event system of the level editor.

        [Header("Level Editor Tool")]
        [SerializeField]private BrushType brushType; // The type of the brush.
        [SerializeField]private GameObject eraseTool; // This should be named "Erase"
        [SerializeField]private bool isEraser; // Is the eraser tool selected?
        [SerializeField]private TileBase eraserTile; // The eraser tile.

        [Header("Tile Asset Parents")]
        [SerializeField]private GameObject floorTilesParent; // The parent of the floor tiles.
        [SerializeField]private GameObject obstacleTilesParent; // The parent of the obstacle tiles.

        [Header("Current & Last Selected Buttons")]
        [SerializeField, ReadOnly]private GameObject selectedButtonTile; // The selected button tile.
        [SerializeField, ReadOnly]private GameObject lastSelectedButtonTile; // The last selected button tile.

        [Header("Grid Positions")]
        [SerializeField, ReadOnly]private Vector3Int currentGridPosition; // The current grid position.
        [SerializeField, ReadOnly]private Vector3Int lastGridPosition; // The last grid position.

        [Header("Current TileBase")]
        [SerializeField, ReadOnly]private TileBase tileBase; // The current tilebase.

        [Header("Current Tilemap & Preview Tilemap")]
        [SerializeField, ReadOnly]private Tilemap tilemap; // The current tilemap.
        [SerializeField, ReadOnly]private Tilemap previewTilemap; // The preview tilemap.

        [Header("Context Related Variables")]
        [SerializeField, ReadOnly]private bool holdActive; // Is the context menu active?
        [SerializeField, ReadOnly]private Vector3Int holdStartPosition; // The start position of the context menu.

        private Camera _camera; // The camera of the level editor.
        private Vector2 mousePos; // The mouse position.
        private Controls controls; // The controls of the level editor.

        private BoundsInt bounds; 

        private bool isPointerOverGameObject; // Is the mouse over a gameobject?

        public void SetBrushType (int value) => brushType = (BrushType)value; // Sets the brush type.

        public void SetIsEraser(bool value) => isEraser = value; // Sets the is eraser value.

        [SerializeField]private List<Tilemap> tilemaps = new List<Tilemap>(); // The list of tilemaps.

        private void Awake() {
            controls = new Controls(); // Creates the controls.
            _camera = Camera.main; // Gets the main camera.
        }

        private void OnEnable () {
            controls.Enable(); // Enables the controls.

            controls.UI.Point.performed += OnMouseMove; // Adds the mouse move event.

            controls.UI.Click.started += OnLeftClick; // Adds the left click start event.
            controls.UI.Click.performed += OnLeftClick; // Adds the left click event.
            controls.UI.Click.canceled += OnLeftClick; // Adds the left click cancel event.
        }

        private void OnDisable () {
            controls.Disable(); // Disables the controls.

            controls.UI.Point.performed -= OnMouseMove; // Removes the mouse move event.

            controls.UI.Click.started -= OnLeftClick; // Removes the left click start event.
            controls.UI.Click.performed -= OnLeftClick; // Removes the left click event.
            controls.UI.Click.canceled -= OnLeftClick; // Removes the left click cancel event.
        }

        // Start is called before the first frame update
        private void Start() {
            GetTilesFromResources("Floor", floorTilesParent); // Get all floor tiles from resources
            GetTilesFromResources("Obstacle", obstacleTilesParent); // Get all obstacle tiles from resources

            List<Tilemap> maps = FindObjectsOfType<Tilemap>().ToList(); // Gets all tilemaps.

            maps.ForEach(map => { // For each tilemap.
                tilemap = map; // Set the tilemap.
            });

            tilemaps.Sort((a, b) => {
                TilemapRenderer aRenderer = a.GetComponent<TilemapRenderer>(); // Gets the tilemap renderer of the tilemap.
                TilemapRenderer bRenderer = b.GetComponent<TilemapRenderer>(); // Gets the tilemap renderer of the tilemap.

                return bRenderer.sortingOrder.CompareTo(aRenderer.sortingOrder); // Compare the sorting orders.
            });
        }

        private void Update () {
            isPointerOverGameObject = eventSystem.IsPointerOverGameObject(); // Is the mouse over a gameobject?

            // if something is selected - show preview
            if (selectedButtonTile != null) {
                Vector3 pos = _camera.ScreenToWorldPoint(mousePos); // Gets the mouse position.
                Vector3Int gridPos = MapManager.instance.grid.WorldToCell(pos); // Gets the grid position.

                if (gridPos != currentGridPosition) {
                    lastGridPosition = currentGridPosition; // Sets the last grid position.
                    currentGridPosition = gridPos; // Sets the current grid position.
                
                    UpdateSingleTilePreview(); 

                    if (holdActive) {
                        HandleDrawing();
                    }
                }
            }
        }

        /// <summary> Updates the preview tilemap with a single tile. </summary>
        private void UpdateSingleTilePreview() {
            // Remove old tile if existing
            previewTilemap.SetTile(lastGridPosition, null);
            // Set current tile to current mouse positions tile
            previewTilemap.SetTile(currentGridPosition, tileBase);
        }

        /// <summary> Gets mouse position. </summary>
        private void OnMouseMove (InputAction.CallbackContext ctx) {
            mousePos = ctx.ReadValue<Vector2> ();
        }

        /// <summary> Gets the left click start event. </summary>
        private void OnLeftClick(InputAction.CallbackContext ctx) {
            if (selectedButtonTile != null && !isPointerOverGameObject) {
                if (ctx.phase == InputActionPhase.Started) {
                    holdActive = true; 

                    if (ctx.interaction is TapInteraction) { 
                        holdStartPosition = currentGridPosition;
                    }
                    HandleDrawing(); 
                } else if (ctx.interaction is SlowTapInteraction || ctx.interaction is TapInteraction && ctx.phase == InputActionPhase.Performed) {
                    holdActive = false;
                    HandleDrawRelease(); 
                }
            }
        }

        private void HandleDrawing() {
            switch (brushType) {
                case BrushType.Line:
                    LineRenderer();
                    break;
                case BrushType.Rectangle:
                    RectangleRenderer();
                    break;
            }
        }

        private void HandleDrawRelease() {
            switch (brushType) {
                case BrushType.Line:
                case BrushType.Rectangle:
                    DrawBounds(tilemap, isEraser); // Draws the bounds on the tilemap.
                    previewTilemap.ClearAllTiles();
                    break;
                case BrushType.Single:
                default:
                    DrawOrEraseTile(tilemap, currentGridPosition, isEraser);
                    break;
            }
        }

        /// <summary> Draws a rectangle based on the start and end positions. </summary>
        private void RectangleRenderer() {
            previewTilemap.ClearAllTiles();

            bounds.xMin = currentGridPosition.x < holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
            bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
            bounds.yMin = currentGridPosition.y < holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
            bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;

            DrawBounds(previewTilemap, false); // Draw the bounds on the preview tilemap.
        }

        /// <summary> Draws a line based on the start and end positions. </summary>
        private void LineRenderer() {
            previewTilemap.ClearAllTiles();

            float diffX = Mathf.Abs(currentGridPosition.x - holdStartPosition.x);
            float diffY = Mathf.Abs(currentGridPosition.y - holdStartPosition.y);

            bool lineIsHorizontal = diffX >= diffY;

            if (lineIsHorizontal) {
                bounds.xMin = currentGridPosition.x < holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
                bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
                bounds.yMin = holdStartPosition.y;
                bounds.yMax = holdStartPosition.y;
            } else {
                bounds.xMin = holdStartPosition.x;
                bounds.xMax = holdStartPosition.x;
                bounds.yMin = currentGridPosition.y < holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
                bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
            }

            DrawBounds(previewTilemap, false); // Draw the bounds on the preview tilemap.
        }
        
        private void DrawBounds(Tilemap map, bool canErase) {
            // Draws bounds on given map
            for (int x = bounds.xMin; x <= bounds.xMax; x++) {
                for (int y = bounds.yMin; y <= bounds.yMax; y++) {
                    DrawOrEraseTile(map, new Vector3Int(x, y, 0), canErase);
                }
            }
        }

        private void DrawOrEraseTile(Tilemap map, Vector3Int gridpos, bool canErase) {
            if(canErase) {   
                tilemaps.Any(map => {
                    if(map.HasTile(gridpos)) {
                        map.SetTile(gridpos, null);
                        return true;
                    }

                    return false;
                });  
            } else {
                map.SetTile(gridpos, tileBase); // Draws the tile on the map.
            }
        }

        public void SetTilemapAndTileBase(){
            //Remove preview tile if existing
            if(previewTilemap != null) {
                previewTilemap.SetTile(currentGridPosition, null);
            }

            if(isEraser) {
                tileBase = eraserTile;
                tilemap = null;
                previewTilemap = MapManager.instance.erasurePreviewMap;
            }
            else{
                //Set the tilemap and tilebase based on the selected button
                if(selectedButtonTile.GetComponent<TileBaseHolder>() != null){
                    tileBase = selectedButtonTile.GetComponent<TileBaseHolder>().tileBase;
                    switch(selectedButtonTile.GetComponent<TileBaseHolder>().tileBaseType){
                        case TileBaseHolder.TileBaseType.Floor:
                            tilemap = MapManager.instance.floorMap;
                            previewTilemap = MapManager.instance.floorPreviewMap;
                            break;
                        case TileBaseHolder.TileBaseType.Obstacle:
                            tilemap = MapManager.instance.obstacleMap;
                            previewTilemap = MapManager.instance.obstaclePreviewMap;
                            break;
                    }
                } else {
                    Debug.LogError("TileBaseHolder component not found on selected tile");
                } 
            }
        }

        /// <summary> Get the last selected button tile </summary>
        public void GetLastButtonTileSelected() {
            if (eventSystem.currentSelectedGameObject != selectedButtonTile) {

                lastSelectedButtonTile = selectedButtonTile; // Set the last selected button tile.

                selectedButtonTile = eventSystem.currentSelectedGameObject; // Set the selected button tile.
            }
            
            if(lastSelectedButtonTile != null) {
                lastSelectedButtonTile.transform.GetChild(0).gameObject.SetActive(false); 
            } 
        }

        /// <summary> Get all the tiles from resources </summary>
        private void GetTilesFromResources(string path, GameObject parent) {
            Tile[] tiles = Resources.LoadAll<Tile>("Tilemap/" + path);
            foreach (Tile tile in tiles) {
                GameObject tileButton = Instantiate(Resources.Load<GameObject>("Prefabs/UI/LevelEditor/TileButton"));
                tileButton.transform.SetParent(parent.transform);
                tileButton.transform.localScale = Vector3.one;
                tileButton.name = tile.name;
                switch(path){
                    case "Floor":
                        tileButton.GetComponent<TileBaseHolder>().tileBaseType = TileBaseHolder.TileBaseType.Floor;
                        break;
                    case "Obstacle":
                        tileButton.GetComponent<TileBaseHolder>().tileBaseType = TileBaseHolder.TileBaseType.Obstacle;
                        break;
                }
                tileButton.GetComponent<TileBaseHolder>().tileBase = tile;
                tileButton.GetComponent<Image>().sprite = tile.sprite;
                
                tileButton.GetComponent<Button>().onClick.AddListener(() => {
                    GetLastButtonTileSelected(); 
                    SetTilemapAndTileBase();
                });
            }
        }
    }
}