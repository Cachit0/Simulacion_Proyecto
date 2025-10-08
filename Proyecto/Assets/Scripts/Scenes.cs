using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    public void Niveles()
    {
        SceneManager.LoadSceneAsync("Niveles");
    }

    public void Juego()
    {
        SceneManager.LoadSceneAsync("Juego");
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}