using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ApiClient : MonoBehaviour
{
    private string deploymentUrl = "avansict2213114.azurewebsites.net/";

    public TMP_InputField rEMailField;
    public TMP_InputField rPasswordField;
    public TMP_InputField lEmailField;
    public TMP_InputField lPasswordField;
    public TMP_InputField WorldNameField;

    public GameObject worldItemPrefab;
    public Transform worldListContainer;
    public WorldBuilder worldBuilder;
    public ObjectManager objectManager;
    public PopupManager popupManager;

    public int currentWorldId;
    public static ApiClient apiclient { get; private set; }

    public static string bearerToken = "";
    private static string _username;

    public ScreenManager screenManager;

    void Awake()
    {
        if (apiclient == null)
        {
            apiclient = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void Register()
    {
        string email = rEMailField.text;
        string password = rPasswordField.text;

        // Wachtwoordvalidatie
        if (!ValidatePassword(password))
        {
            Debug.LogError("Het wachtwoord voldoet niet aan de vereisten.");
            return;
        }

        // Maak de aanvraag voor registratie
        var request = new PostRegisterRequestDto()
        {
            email = email,
            password = password,
        };

        var jsonData = JsonUtility.ToJson(request);
        await PerformApiCall("account/register", "POST", jsonData);
    }

    // Methode voor wachtwoordvalidatie
    private bool ValidatePassword(string password)
    {
        // Wachtwoord moet minimaal 10 tekens lang zijn
        if (password.Length < 10)
        {
            Debug.LogError("Wachtwoord moet minimaal 10 tekens lang zijn.");
            popupManager.ShowPopup("Wachtwoord moet minimaal 10 tekens lang zijn." , Color.red);
            return false;
        }

        // Reguliere expressie om te controleren op minimaal 1 lowercase, 1 uppercase, 1 cijfer en 1 niet-alfa-numeriek karakter
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s]).{10,}$";
        Regex regex = new Regex(pattern);

        if (!regex.IsMatch(password))
        {
            popupManager.ShowPopup("Wachtwoord moet minimaal 1 lowercase, 1 uppercase, 1 cijfer en 1 speciaal karakter bevatten.", Color.red);
            Debug.LogError("Wachtwoord moet minimaal 1 lowercase, 1 uppercase, 1 cijfer en 1 speciaal karakter bevatten.");
            return false;
        }

        return true;
    }

public async void Login()
    {
        var request = new PostLoginRequestDto()
        {
            email = lEmailField.text,
            password = lPasswordField.text
        };

        var jsonData = JsonUtility.ToJson(request);
        var response = await PerformApiCall("account/login", "POST", jsonData);

        if (!string.IsNullOrEmpty(response))
        {
            var responseDto = JsonUtility.FromJson<PostLoginResponseDto>(response);
            bearerToken = responseDto.accessToken;
            _username = lEmailField.text;
            popupManager.ShowPopup("Succesvolle login!", Color.green);
            screenManager.LoadScreen("Menu");
        }
        else
        {
            popupManager.ShowPopup("Verkeerde inloggegevens", Color.red);
            Debug.LogError("Login failed: No response or invalid credentials.");
        }
    }

    private string userName
    {
        get { return _username; }
        set { _username = value; }
    }

    public string GetUserName() => userName;

    public async void OnCreateWorldButtonClicked()
    {
        string naam = WorldNameField.text;
        int maxHeight = 100;
        int maxWidth = 100;

        await CreateWorld(naam, maxHeight, maxWidth);
    }

    public async Task<WorldDTO> CreateWorld(string naam, int height, int width)
    {
        if (string.IsNullOrEmpty(bearerToken))
        {
            Debug.LogError("Bearer token is missing.");
            return null;
        }

        // Controleer of de naam van de wereld geldig is
        if (string.IsNullOrEmpty(naam) || naam.Length < 1 || naam.Length > 25)
        {
            Debug.LogError("De naam van de wereld moet tussen de 1 en 25 tekens zijn.");
            popupManager.ShowPopup("De naam van de wereld moet tussen de 1 en 25 tekens zijn.", Color.red);
            return null;
        }

        // Verkrijg de bestaande werelden van de gebruiker om te controleren op duplicaten
        List<WorldDTO> existingWorlds = await GetWorlds();
        if (existingWorlds == null)
        {
            Debug.LogError("Failed to retrieve worlds.");
            return null;
        }

        if (existingWorlds.Count >= 5)
        {
            Debug.LogError("Je kunt niet meer dan 5 werelden hebben.");
            popupManager.ShowPopup("Je kunt niet meer dan 5 werelden hebben.", Color.red);
            return null;
        }

        // Controleer of de naam al bestaat bij de gebruiker
        foreach (var world in existingWorlds)
        {
            if (world.Naam.Equals(naam, StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogError("De naam voor de nieuwe wereld bestaat al.");
                popupManager.ShowPopup("De naam voor de nieuwe wereld bestaat al.", Color.red);
                return null;
            }
        }

        // Maak de nieuwe wereld aan
        var request = new CreateWorldRequestDTO()
        {
            Naam = naam,
            Username = _username,
            MaxHeight = height,
            MaxWidth = width
        };

        var jsonData = JsonUtility.ToJson(request);
        string response = await PerformApiCall("api/environment2d", "POST", jsonData, bearerToken);

        if (!string.IsNullOrEmpty(response))
        {
            WorldDTO worldData = JsonConvert.DeserializeObject<WorldDTO>(response);
            currentWorldId = worldData.Id;
            LoadIntoWorld();
            return worldData;
        }
        else
        {
            Debug.LogError("Er is een fout opgetreden bij het aanmaken van de wereld.");
            return null;
        }
    }





    public void LoadIntoWorld()
    {
        screenManager.LoadScreen("Game");
    }

    public async Task<List<WorldDTO>> GetWorlds()
    {
        if (string.IsNullOrEmpty(bearerToken))
        {
            Debug.LogError("Bearer token is null or empty.");
            return null;
        }

        string apiUrl = $"api/environment2d/userworlds?UserName={_username}";
        Debug.Log($"Making API call to: {apiUrl}");

        string response = await PerformApiCall(apiUrl, "GET", token: bearerToken);

        if (string.IsNullOrEmpty(response))
        {
            Debug.LogWarning("Received empty response from API.");
            return null;
        }

        Debug.Log("API response received. Attempting to deserialize.");

        try
        {
            List<WorldDTO> worlds = JsonConvert.DeserializeObject<List<WorldDTO>>(response);

            if (worlds == null || worlds.Count == 0)
            {
                Debug.LogWarning("No worlds found in the response.");
            }

            return worlds;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deserializing worlds response: {e.Message}\nStackTrace: {e.StackTrace}");
        }

        return null;
    }


    public async void LoadWorlds()
    {
        List<WorldDTO> worlds = await GetWorlds();
        DisplayWorlds(worlds);
    }

    public void DisplayWorlds(List<WorldDTO> worlds)
    {
        if (worldListContainer.childCount > 0)
        {
            var exampleObject = worldListContainer.GetChild(0).gameObject;
            exampleObject.SetActive(false);
        }

        // Clear previous world items
        foreach (Transform child in worldListContainer)
        {
            if (child != worldListContainer.GetChild(0))
                Destroy(child.gameObject);
        }

        if (worlds == null || worlds.Count == 0) return;

        foreach (var world in worlds)
        {
            GameObject worldItem = Instantiate(worldItemPrefab, worldListContainer);
            worldItem.SetActive(true);

            TMP_Text worldNameText = worldItem.GetComponentInChildren<TMP_Text>();
            if (worldNameText != null)
                worldNameText.text = world.Naam;

            foreach (var component in worldItem.GetComponentsInChildren<MonoBehaviour>())
                component.enabled = true;

            // Set up Load Button
            Button LoadButton = worldItem.transform.Find("LoadButton")?.GetComponent<Button>();  
            if (LoadButton != null)
            {
                var capturedWorld = world;
                LoadButton.onClick.RemoveAllListeners(); 
                LoadButton.onClick.AddListener(() => LoadWorld(capturedWorld));  
            }

            // Set up Delete Button
            Button DeleteButton = worldItem.transform.Find("DeleteButton")?.GetComponent<Button>();
            if (DeleteButton != null)
            {
                var capturedWorldForDelete = world;
                DeleteButton.onClick.RemoveAllListeners(); 
                DeleteButton.onClick.AddListener(() => DeleteWorld(capturedWorldForDelete));  
            }
        }
    }

    public async void LoadWorld(WorldDTO world)
    {
        worldBuilder.currentWorldId = world.Id;
        currentWorldId = world.Id;
        Debug.Log("Current worldId: " + currentWorldId);

        // Fetch the objects for the current world using the API
        List<ObjectDTO> objectsInWorld = await GetObjectsInWorld(currentWorldId);

        // If there are any objects, place them in the world
        if (objectsInWorld != null && objectsInWorld.Count > 0)
        {
            foreach (var objectData in objectsInWorld)
            {
                Debug.Log($"Object Data - PrefabId: {objectData.PrefabId}, Position: ({objectData.PositionX}, {objectData.PositionY}), Rotation: {objectData.RotationZ}, Scale: ({objectData.ScaleX}, {objectData.ScaleY})");
            }
        }

        ObjectManager.Instance.PlaceAllObjects(objectsInWorld);

        screenManager.LoadScreen("Game");
    }

    public async Task<List<ObjectDTO>> GetObjectsInWorld(int worldId)
    {
        // Ensure the bearer token is available
        if (string.IsNullOrEmpty(bearerToken))
        {
            Debug.LogError("Bearer token is null or empty.");
            return null;
        }

        // Construct the API URL to get objects by world ID
        string apiUrl = $"api/objects/{worldId}"; 

        try
        {
            // Make the API call with the bearer token
            string response = await PerformApiCall(apiUrl, "GET", token: bearerToken);

            // If the response is not null or empty, deserialize the JSON
            if (!string.IsNullOrEmpty(response))
            {
                // Deserialize JSON response to a list of ObjectDTO
                List<ObjectDTO> objects = JsonConvert.DeserializeObject<List<ObjectDTO>>(response);

                // Return the list of objects
                return objects;
            }
            else
            {
                Debug.LogWarning("Received an empty response from the API.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during API call or deserialization: {e.Message}");
        }

        return null;
    }

    public async void DeleteWorld(WorldDTO world)
    {
        if (string.IsNullOrEmpty(bearerToken)) return;

        string apiUrl = $"api/environment2d/{world.Id}";
        string response = await PerformApiCall(apiUrl, "DELETE", token: bearerToken);

        if (string.IsNullOrEmpty(response))
        {
            // Handle successful deletion or empty response
            popupManager.ShowPopup("De wereld is verwijderd. Vernieuw de pagina om de wijziging te zien.", Color.red);
            Debug.Log("World deleted successfully.");
        }

        currentWorldId = 0; 
    }


    public async Task<bool> DeleteAllObjectsFromWorld(int worldId)
    {
        string url = $"api/objects/{worldId}";
        string response = await PerformApiCall(url, "DELETE", token: bearerToken);

        if (string.IsNullOrEmpty(response))
        {
            
            Debug.Log("No objects to delete.");
            return true; 
        }

        if (response.Contains("404"))
        {
           
            Debug.Log("No objects found for this environmentId (404 response).");
            return true; 
        }

      
        Debug.LogError("Failed to delete objects, unexpected response.");
        return false;
    }




    public void SavePlacedObjectsButtonClicked()
    {
        _ = SavePlacedObjects(); 
    }

    public async Task<bool> SavePlacedObjects()
    {
        if (ObjectManager.Instance == null)
        {
            Debug.LogError("ObjectManager instance is null!");
            return false;
        }

        if (currentWorldId == 0)
        {
            Debug.LogError("Invalid currentWorldId!");
            return false;
        }

        if (string.IsNullOrEmpty(bearerToken))
        {
            Debug.LogError("Bearer token is missing.");
            return false;
        }

        bool deleteSuccess = await DeleteAllObjectsFromWorld(currentWorldId);
        if (!deleteSuccess)
        {
            Debug.LogError("Failed to delete old objects.");
            return false;
        }

        List<GameObject> placedGameObjects = ObjectManager.Instance.GetPlacedObjects();

        if (placedGameObjects == null || placedGameObjects.Count == 0)
        {
            Debug.LogError("No placed objects to save!");
            return false;
        }

        List<ObjectDTO> objectDTOList = new List<ObjectDTO>();

        foreach (GameObject obj in placedGameObjects)
        {
            var objectDTO = new ObjectDTO()
            {
                EnvironmentId = currentWorldId,
                PrefabId = obj.name.Replace("(Clone)", "").Trim(),
                PositionX = obj.transform.position.x,
                PositionY = obj.transform.position.y,
                ScaleX = obj.transform.localScale.x,
                ScaleY = obj.transform.localScale.y,
                RotationZ = obj.transform.rotation.eulerAngles.z
            };

            objectDTOList.Add(objectDTO);
        }

        string jsonData = JsonConvert.SerializeObject(objectDTOList);
        string response = await PerformApiCall("api/objects", "POST", jsonData, bearerToken);

        if (string.IsNullOrEmpty(response))
        {
            Debug.LogError("Failed to save placed objects.");
            return false;
        }
        else
        {
            popupManager.ShowPopup("Wereld opgeslagen!", Color.green);
            Debug.Log("Objects successfully saved.");

        }
        return true;
    }



    private async Task<string> PerformApiCall(string apiUrl, string method, string jsonData = null, string token = "")
    {
        UnityWebRequest request = new UnityWebRequest(deploymentUrl + apiUrl, method);

        if (method != "GET" && jsonData != null)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.SetRequestHeader("Content-Type", "application/json");
        }

        if (!string.IsNullOrEmpty(token))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        await request.SendWebRequest();

        // Check if the result is successful
        if (request.result == UnityWebRequest.Result.Success)
        {
            // If the response body is empty, treat it as a success and return an empty string
            if (string.IsNullOrEmpty(request.downloadHandler.text))
            {
                Debug.LogWarning("Received empty response, assuming success.");
                return "";  // Gracefully handle empty response as successful
            }

            return request.downloadHandler.text;
        }
        else
        {
            Debug.LogError($"API call failed: {request.error}, Code: {request.responseCode}, Body: {request.downloadHandler.text}");

            if (request.responseCode == 404)
            {
                Debug.LogWarning("The resource was not found. Continuing without error.");
                return ""; 
            }
            if (request.responseCode == 400)
            {
                popupManager.ShowPopup("Account bestaat al.", Color.red);
                Debug.LogWarning("Account bestaat al");
            }

            return null; 
        }
    }


}
