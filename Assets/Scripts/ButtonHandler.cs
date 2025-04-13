using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public GameObject objectToPlace; 

    public void OnButtonClick()
    {
        if (ObjectManager.Instance == null)
        {
            Debug.LogError("ObjectManager instance is null! Make sure it's initialized.");
            return;
        }

        ObjectManager.Instance.StartPlacingObject(objectToPlace);
    }
}
