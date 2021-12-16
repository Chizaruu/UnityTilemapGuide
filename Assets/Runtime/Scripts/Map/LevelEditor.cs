using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UTG.Character;
using UTG.Map;

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
    [SerializeField]private GameObject floorTilesParent;
    [SerializeField]private GameObject obstacleTilesParent;

    [Header("Current & Last Selected Buttons")]
    [SerializeField, ReadOnly]private GameObject selectedButtonTile;
    [SerializeField, ReadOnly]private GameObject lastSelectedButtonTile;

    [Header("Grid Positions")]
    [SerializeField, ReadOnly]private Vector3Int currentGridPosition;
    [SerializeField, ReadOnly]private Vector3Int lastGridPosition;

    [Header("Current TileBase")]
    [SerializeField, ReadOnly]private TileBase tileBase;

    [Header("Current Tilemap & Preview Tilemap")]
    [SerializeField, ReadOnly]private Tilemap tilemap;
    [SerializeField, ReadOnly]private Tilemap previewTilemap;

    [Header("Context Related Variables")]
    [SerializeField, ReadOnly]private bool holdActive;
    [SerializeField, ReadOnly]private Vector3Int holdStartPosition;

    private Camera _camera;
    private Vector2 mousePos;
    private Controls controls;

    private BoundsInt bounds;

    public void SetBrushType (int value) => brushType = (BrushType)value;

    public void SetIsEraser(bool value) => isEraser = value;

    private void Awake() {
        controls = new Controls();
        _camera = Camera.main;
    }

    private void OnEnable () {
        controls.Enable();

        controls.UI.Point.performed += OnMouseMove;

        controls.UI.Click.started += OnLeftClick;
        controls.UI.Click.performed += OnLeftClick;
        controls.UI.Click.canceled += OnLeftClick;
    }

    private void OnDisable () {
        controls.Disable();

        controls.UI.Point.performed -= OnMouseMove;

        controls.UI.Click.started -= OnLeftClick;
        controls.UI.Click.performed -= OnLeftClick;
        controls.UI.Click.canceled -= OnLeftClick;
    }

    // Start is called before the first frame update
    private void Start() {
        GetTilesFromResources("Floor", floorTilesParent); // Get all floor tiles from resources
        GetTilesFromResources("Obstacle", obstacleTilesParent); // Get all obstacle tiles from resources
    }

    private void Update () {
        // if something is selected - show preview
        if (selectedButtonTile != null) {
            Vector3 pos = _camera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = MapManager.instance.grid.WorldToCell(pos);

            if (gridPos != currentGridPosition) {
                lastGridPosition = currentGridPosition;
                currentGridPosition = gridPos;
            
                UpdatePreview();

                if (holdActive) {
                    HandleDrawing();
                }
            }
        }
    }

    private void UpdatePreview() {
        // Remove old tile if existing
        previewTilemap.SetTile(lastGridPosition, null);
        // Set current tile to current mouse positions tile
        previewTilemap.SetTile(currentGridPosition, tileBase);
    }

    private void OnMouseMove (InputAction.CallbackContext ctx) {
        mousePos = ctx.ReadValue<Vector2> ();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx) {
        if (selectedButtonTile != null && !EventSystem.current.IsPointerOverGameObject() || isEraser) {
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
        if (selectedButtonTile != null) {
            switch (brushType) {
                case BrushType.Single:
                default:
                    DrawOrEraseTile(tilemap, currentGridPosition, true);
                    break;
                case BrushType.Line:
                    LineRenderer();
                    break;
                case BrushType.Rectangle:
                    RectangleRenderer();
                    break;
            }
        }
    }

    private void HandleDrawRelease() {
        if (selectedButtonTile != null) {
            switch (brushType) {
                case BrushType.Line:
                case BrushType.Rectangle:
                    DrawBounds(tilemap, isEraser);
                    previewTilemap.ClearAllTiles();
                    break;
            }
        }
    }

    private void RectangleRenderer() {
        //  Render Preview on UI Map, draw real one on Release

        previewTilemap.ClearAllTiles();

        bounds.xMin = currentGridPosition.x < holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.yMin = currentGridPosition.y < holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
        bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;

        DrawBounds(previewTilemap, false);
    }

    private void LineRenderer() {
        //  Render Preview on UI Map, draw real one on Release

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

        DrawBounds(previewTilemap, false);
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
        if(isEraser && canErase) {       
            if(MapManager.instance.obstacleMap.HasTile(gridpos)) {
                MapManager.instance.obstacleMap.SetTile(gridpos, null);
            } else if(MapManager.instance.floorMap.HasTile(gridpos)) {
                MapManager.instance.floorMap.SetTile(gridpos, null);
            }
        } else {
            map.SetTile(gridpos, tileBase);
        }
    }

    public void SetTilemapAndTileBase(){
        if(isEraser) {
            tileBase = eraserTile;
            tilemap = null;
            previewTilemap = MapManager.instance.erasurePreviewMap;
        }
        else if(selectedButtonTile != null){
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
        } else {
            tileBase = null;
            tilemap = null;
            previewTilemap = null;
        }
    }

    /// <summary> Get the last selected button tile </summary>
    public void GetLastButtonTileSelected() {
        if (eventSystem.currentSelectedGameObject != selectedButtonTile) {

            lastSelectedButtonTile = selectedButtonTile;

            selectedButtonTile = eventSystem.currentSelectedGameObject;
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
