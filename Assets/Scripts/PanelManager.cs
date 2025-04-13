using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels; 
    private int activePanelIndex = 0;

    public Color SelectedColor;
    public Color NormalColor;

    public Button Nature;
    public Button Wood;

    public void SwitchPanel(int panelIndex)
    {
        ColorBlock cbNature = Nature.colors;
        ColorBlock cbWood = Wood.colors;

        cbNature.normalColor = NormalColor;
        cbWood.normalColor = NormalColor;

        Nature.colors = cbNature;
        Wood.colors = cbWood;

        switch (panelIndex)
        {
            case 0:
                cbNature.normalColor = SelectedColor;
                Nature.colors = cbNature;
                break;
            case 1:
                cbWood.normalColor = SelectedColor;
                Wood.colors= cbWood;    
                break;
        }

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == panelIndex);
        }

        activePanelIndex = panelIndex;
    }
}
