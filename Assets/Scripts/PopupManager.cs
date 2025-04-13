using System.Collections;
using UnityEngine;
using TMPro;  // Zorg ervoor dat je deze namespace toevoegt voor TMP

public class PopupManager : MonoBehaviour
{
    public GameObject popupPrefab;  // Het popup prefab (moet een Panel bevatten, toegewezen in de Inspector)
    public TextMeshProUGUI popupText;  // Verwijs naar het TextMeshProUGUI component in de Inspector

    public void ShowPopup(string message, Color textColor)
    {
        // Zorg ervoor dat de popup wordt geactiveerd
        if (popupPrefab != null)
        {
            popupPrefab.SetActive(true);  // Zet de popup aan
        }
        else
        {
            Debug.LogError("Popup prefab is niet toegewezen in de Inspector!");
        }

        // Controleer of popupText is toegewezen in de Inspector
        if (popupText != null)
        {
            // Stel de tekstkleur en de boodschap in
            popupText.text = message;
            popupText.color = textColor;  // Kleur van de tekst
        }
        else
        {
            Debug.LogError("Popup TextMeshPro component is niet toegewezen in de Inspector!");
        }

        // Start de coroutine om de popup na 3 seconden te verbergen
        StartCoroutine(HidePopupAfterDelay(3f));
    }

    private IEnumerator HidePopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wacht de opgegeven tijd
        HidePopup();  // Verberg de popup
    }

    private void HidePopup()
    {
        if (popupPrefab != null)
        {
            popupPrefab.SetActive(false);  // Verberg de popup
        }
    }
}
