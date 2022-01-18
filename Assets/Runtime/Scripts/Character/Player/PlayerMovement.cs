using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UTG.Map;

namespace UTG.Character
{
    /// <summary> Player Movement. </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private Controls controls; //input controls

        [SerializeField]private FOV fov;

        private bool holdActive;

        [SerializeField, ReadOnly]private Vector3 vec;

        /// <summary> Initialize controls. </summary>
        private void Awake() => controls = new Controls();

        /// <summary> Start listening to input. </summary>
        private void OnEnable(){
            controls.Enable();

            /*
            controls.Player.Movement.started += OnMovement;
            controls.Player.Movement.performed += OnMovement;
            controls.Player.Movement.canceled += OnMovement;
            */
        }

        /// <summary> Stop listening to input. </summary>
        private void OnDisable(){
            controls.Disable();

            /*
            controls.Player.Movement.started -= OnMovement;
            controls.Player.Movement.performed -= OnMovement;
            controls.Player.Movement.canceled -= OnMovement;
            */
        }

        /*
        private void Update() {
            if(holdActive)
            {
                MovePlayer();
            }
        }
        */
        
        /// <summary> Update player movement. </summary>
        private void FixedUpdate()
        {
            //Used for repeated 8-direction movement
            //PC, Linux, *Gasps* Mac
            
            //Arrowkeys
            var up = Keyboard.current.upArrowKey.IsPressed(); 
            var down = Keyboard.current.downArrowKey.IsPressed(); 
            var left = Keyboard.current.leftArrowKey.IsPressed(); 
            var right = Keyboard.current.rightArrowKey.IsPressed(); 

            //Used to stop all movement if 3 arrowkeys are pressed at once.
            if(up && down && left) return;
            if(up && down && right) return;
            if(up && left && right) return;
            if(down && left && right) return;

            //Player moves when an arrow key is pressed
            if(up || down || right || left)
            {
                //Invoke because we want to give some time to read values if we want diagonal movement.
                Invoke("MovePlayer", 0.1f);
            }
        }
        
        /*
        private void OnMovement(InputAction.CallbackContext ctx)
        {
            if(ctx.performed)
            {
                holdActive = true;
            }
            else if(ctx.interaction is SlowTapInteraction || ctx.interaction is TapInteraction)
            {
                holdActive = false;
                MovePlayer();
            }
        }
        */

        /// <summary> Move player </summary>
        private void MovePlayer()
        {
            vec = (Vector3)controls.Player.Movement.ReadValue<Vector2>();
            //Round the vector to the nearest integer.
            vec = new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), 0);

            //Get the future position of the player.
            Vector3 futureVec = transform.position + vec;

            //Check if the game is playing, if it is, return.
            if(GameManager.instance.IsPlaying) return;

            //Check if the future position is valid.
            if(!IsValidPosition(futureVec)) return;

            //Move the player.
            transform.position = futureVec;
            fov.RefreshFieldOfView();
            GameManager.instance.TurnChange();
        }

        /// <summary> Check if player can move. </summary>
        private bool IsValidPosition(Vector3 futureVec)
        {
            Vector3Int gridPosition = MapManager.instance.floorMap.WorldToCell(futureVec); //grid position of player
            if (!MapManager.instance.floorMap.HasTile(gridPosition) || MapManager.instance.obstacleMap.HasTile(gridPosition) || futureVec == transform.position)
                return false; //if player can't move, return false
            return true;//if player can move, return true
        } 
    } 
}


