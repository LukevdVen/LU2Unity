using UnityEngine;
using UnityEngine.Profiling;

public class ScreenManager : MonoBehaviour
{
    public Canvas Login;
    public Canvas Register;
    public Canvas Menu;
    public Canvas Create;
    public Canvas Edit;
    public Canvas Game;

    public void LoadScreen(string scene)
    {
        Login.gameObject.SetActive(false);
        Register.gameObject.SetActive(false);
        Menu.gameObject.SetActive(false);
        Create.gameObject.SetActive(false);
        Edit.gameObject.SetActive(false);
        Game.gameObject.SetActive(false);

        switch (scene)
        {
            case "Login":
                Login.gameObject.SetActive(true);
                break;
            case "Register":
                Register.gameObject.SetActive(true);
                break;
            case "Menu":
                Menu.gameObject.SetActive(true);
                break;
            case "Create":
                Create.gameObject.SetActive(true);
                break;
            case "Edit":
                Edit.gameObject.SetActive(true);
                break;
            case "Game":
                Game.gameObject.SetActive(true);
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

