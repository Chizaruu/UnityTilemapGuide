using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
    [SerializeField]private EventSystem eventSystem;

    [SerializeField]private GameObject eraseTool; // This should be named "Erase"
    [SerializeField]private GameObject eraseToolSelected; 
    [SerializeField]private GameObject lastSelectedGameObject;
    private GameObject currentSelectedGameObject_Recent;

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
}
