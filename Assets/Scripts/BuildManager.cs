using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public GameObject[] buildObjects;  
    public GameObject buttonPrefab;    
    public Transform buildingMenu;     
    public List<GameObject> buttons = new List<GameObject>(); 

    void Awake()
    {
        // Check if buttonPrefab and buildingMenu are assigned
        if (buttonPrefab == null)
        {
            Debug.LogError("Button prefab is not assigned!");
            return;
        }

        if (buildingMenu == null)
        {
            Debug.LogError("Building Menu is not assigned!");
            return;
        }

        // Loop through each object in buildObjects
        foreach (GameObject go in buildObjects)
        {
            if (go == null)
            {
                Debug.LogWarning("Build object is null, skipping...");
                continue;
            }

            GameObject instantiatedButton = Instantiate(buttonPrefab, buildingMenu);
            Debug.Log("Button instantiated for: " + go.name);

            // Check if the instantiated button is valid
            if (instantiatedButton == null)
            {
                Debug.LogError("Failed to instantiate the button!");
                continue;
            }

            // Check if the buttonPrefab has necessary components
            if (instantiatedButton.GetComponent<Button>() == null)
            {
                Debug.LogError("Button component missing on instantiated button prefab!");
                continue;
            }

            if (instantiatedButton.GetComponent<Image>() == null)
            {
                Debug.LogError("Image component missing on instantiated button prefab!");
                continue;
            }

            Sprite objectSprite = go.GetComponent<SpriteRenderer>()?.sprite;
            if (objectSprite != null)
            {
                instantiatedButton.GetComponent<Image>().sprite = objectSprite;
            }
            else
            {
                Debug.LogWarning($"No SpriteRenderer found on {go.name}. Button will not display sprite.");
            }

            Button button = instantiatedButton.GetComponent<Button>();

            // Ensure the ButtonHandler is properly assigned to the button
            ButtonHandler buttonHandler = instantiatedButton.GetComponent<ButtonHandler>();
            if (buttonHandler == null)
            {
                buttonHandler = instantiatedButton.AddComponent<ButtonHandler>();
            }

            buttonHandler.objectToPlace = go;

            // Add the button to the buttons list
            buttons.Add(instantiatedButton);
            Debug.Log($"Button added to the list for: {go.name}");

            button.onClick.AddListener(buttonHandler.OnButtonClick);

            Debug.Log($"Instantiated Button: {instantiatedButton.name}, Is Active: {instantiatedButton.activeSelf}");
        }

        Debug.Log("Total buttons created: " + buttons.Count);
    }
}
