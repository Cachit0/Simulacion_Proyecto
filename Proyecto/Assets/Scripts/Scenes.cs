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
        // ⭐ Opcional: Reiniciar progreso al volver a niveles desde menú
        // GameData.ReiniciarProgreso();
        SceneManager.LoadSceneAsync("Niveles");
    }

    public void Juego()
    {
        SceneManager.LoadSceneAsync("Juego");
    }

    // ⭐ NUEVOS MÉTODOS para seleccionar niveles
    public void SeleccionarNivel1()
    {
        // Nivel 1 siempre disponible
        GameData.nivelActual = 1;
        GameData.tipoDialogo = TipoDialogo.Intro;

        GameData.configuracionActual = new ConfiguracionNivel(
            1,
            "Pociones de Curación",
            10f,
            new ObjetivoNivel[]
            {
                new ObjetivoNivel(3, 1)
            }
        );

        SceneManager.LoadScene("Dialogos");
    }

    public void SeleccionarNivel2()
    {
        // ⭐ VERIFICAR: Solo permite jugar si intentó el nivel 1
        if (!GameData.NivelDesbloqueado(2))
        {
            Debug.Log("¡Debes jugar el Nivel 1 primero!");
            return;
        }

        GameData.nivelActual = 2;
        GameData.tipoDialogo = TipoDialogo.Intro;

        GameData.configuracionActual = new ConfiguracionNivel(
            2,
            "Elixires Mágicos",
            10f,
            new ObjetivoNivel[]
            {
                new ObjetivoNivel(3, 1)
            }
        );

        SceneManager.LoadScene("Dialogos");
    }

    public void SeleccionarNivel3()
    {
        // ⭐ VERIFICAR: Solo permite jugar si intentó el nivel 1 y 2
        if (!GameData.NivelDesbloqueado(3))
        {
            Debug.Log("¡Debes jugar los niveles anteriores primero!");
            return;
        }

        GameData.nivelActual = 3;
        GameData.tipoDialogo = TipoDialogo.Intro;

        GameData.configuracionActual = new ConfiguracionNivel(
            3,
            "Tónicos Supremos",
            10f,
            new ObjetivoNivel[]
            {
                new ObjetivoNivel(3, 1)
            }
        );

        SceneManager.LoadScene("Dialogos");
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}