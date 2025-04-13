using UnityEngine;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    public static ModeManager Instance { get; private set; }

    public enum BuildMode { Place, Pickup, Delete }
    public BuildMode currentMode = BuildMode.Place;

    public Color SelectedColor;
    public Color NormalColor;

    public Button Place;
    public Button Pickup;
    public Button Delete;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetMode(int mode)
    {
        ColorBlock cbPlace = Place.colors;
        ColorBlock cbPickup = Pickup.colors;
        ColorBlock cbDelete = Delete.colors;

        cbPlace.normalColor = NormalColor;
        cbPickup.normalColor = NormalColor;
        cbDelete.normalColor = NormalColor;

        Place.colors = cbPlace;
        Pickup.colors = cbPickup;
        Delete.colors = cbDelete;

        switch (mode)
        {
            case 0:
                cbPlace.normalColor = SelectedColor;
                Place.colors = cbPlace;
                break;
            case 1:
                cbPickup.normalColor = SelectedColor;
                Pickup.colors = cbPickup;
                break;
            case 2:
                cbDelete.normalColor = SelectedColor;
                Delete.colors = cbDelete;
                break;
        }

        currentMode = (BuildMode)mode;
        Debug.Log("Switched Mode To: " + currentMode);
    }
}
