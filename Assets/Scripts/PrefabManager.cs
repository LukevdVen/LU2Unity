using UnityEngine;
using System.Collections.Generic;

public class PrefabManager : MonoBehaviour
{

    public static PrefabManager Instance; // Singleton instance

    [System.Serializable]

    public class PrefabMapping
    {
        public GameObject Prefab; // The prefab to associate with the ID
    }

    [SerializeField]
    private List<PrefabMapping> prefabMappings; // List of mappings

    // Dictionary to store the mappings for fast lookup
    private Dictionary<string, GameObject> prefabDict;

    void Awake()
    {
        // Ensure there is only one instance of PrefabManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // If an instance already exists, destroy this duplicate
        }

        prefabDict = new Dictionary<string, GameObject>();

        // Populate the dictionary with the mappings
        foreach (var mapping in prefabMappings)
        {
            // Automatically set the PrefabId to the prefab's name
            string prefabId = mapping.Prefab.name;

            if (!prefabDict.ContainsKey(prefabId))
            {
                prefabDict.Add(prefabId, mapping.Prefab);
            }
            else
            {
                Debug.LogWarning($"Duplicate prefab found with name: {prefabId}");
            }
        }
    }

    // Function to get the prefab for a given prefab ID
    public GameObject GetPrefabById(string prefabId)
    {
        if (prefabDict.TryGetValue(prefabId, out var prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogWarning($"No prefab found for PrefabId: {prefabId}");
            return null;
        }
    }
}
