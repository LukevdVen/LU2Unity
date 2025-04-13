using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;
    public GameObject objectToPlace;
    public static List<GameObject> placedObjects = new List<GameObject>();
    [SerializeField] private GameObject instantiatedObject;
    [SerializeField] private bool isDragging;
    private bool hasLoggedObjects = false;
    private ApiClient apiClient;
    public PrefabManager prefabManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            if (instantiatedObject != null)
            {
                instantiatedObject.transform.position = GetMousePos();
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject();
            }
        }
        else if (ModeManager.Instance.currentMode == ModeManager.BuildMode.Pickup && Input.GetMouseButtonDown(0))
        {
            PickupObject();
        }
        else if (ModeManager.Instance.currentMode == ModeManager.BuildMode.Delete && Input.GetMouseButtonDown(0))
        {
            DeleteObject();
        }

        if (Input.GetKeyDown(KeyCode.P) && !hasLoggedObjects)
        {
            DisplayPlacedObjects();
            hasLoggedObjects = true;
        }
        else if (!Input.GetKey(KeyCode.P))
        {
            hasLoggedObjects = false;
        }
    }

    public void StartPlacingObject(GameObject objectToPlace)
    {
        if (!isDragging && ModeManager.Instance.currentMode == ModeManager.BuildMode.Place)
        {
            if (objectToPlace != null)
            {
                Vector3 mousePos = GetMousePos();
                mousePos.z = 0;
                instantiatedObject = Instantiate(objectToPlace, mousePos, Quaternion.identity);
                isDragging = true;
            }
        }
    }

    public void PlaceObject()
    {
        if (isDragging && instantiatedObject != null)
        {
            isDragging = false;
            AudioManager.Instance.PlaceItem();

            // Object aan lijst toevoegen als het er nog niet in zit
            if (!placedObjects.Contains(instantiatedObject))
            {
                placedObjects.Add(instantiatedObject);
            }

            instantiatedObject = null;
        }
    }

    public void PickupObject()
    {
        if (isDragging) return;

        RaycastHit2D hit = Physics2D.Raycast(GetMousePos(), Vector2.zero);
        if (hit.collider != null)
        {
            // Object oppakken dat al geplaatst is
            instantiatedObject = hit.collider.gameObject;
            isDragging = true;
        }
    }

    public void DeleteObject()
    {
        if (isDragging) return;

        RaycastHit2D hit = Physics2D.Raycast(GetMousePos(), Vector2.zero);
        if (hit.collider != null)
        {
            GameObject objectToDelete = hit.collider.gameObject;
            placedObjects.Remove(objectToDelete);
            Destroy(objectToDelete);
        }
    }

    public List<GameObject> GetPlacedObjects()
    {
        return placedObjects;
    }

    public void DisplayPlacedObjects()
    {
        Debug.Log("Placed objects:");
        foreach (var obj in placedObjects)
        {
            Debug.Log($"Object: {obj.name}, Position: {obj.transform.position}");
        }
    }

    public void PlaceAllObjects(List<ObjectDTO> objects)
    {
        if (objects == null) return;
        if (placedObjects == null) return;
        if (prefabManager == null) return;

        foreach (var objectData in objects)
        {
            if (objectData == null) continue;

            GameObject prefab = prefabManager.GetPrefabById(objectData.PrefabId);
            if (prefab == null) continue;

            Vector3 position = new Vector3(objectData.PositionX, objectData.PositionY, 0);
            Quaternion rotation = Quaternion.Euler(0, 0, objectData.RotationZ);
            Vector3 scale = new Vector3(objectData.ScaleX, objectData.ScaleY, 1);

            GameObject placedObject = Instantiate(prefab, position, rotation);
            placedObject.transform.localScale = scale;
            placedObjects.Add(placedObject);
        }
    }

    public void DeleteAll()
    {
        // Alle geplaatste objecten verwijderen
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        placedObjects.Clear();
    }

    public Vector3 GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseposX = Mathf.RoundToInt(mousePos.x);
        float mousePosY = Mathf.RoundToInt(mousePos.y);
        return new Vector3(mouseposX, mousePosY, 0);
    }
}
