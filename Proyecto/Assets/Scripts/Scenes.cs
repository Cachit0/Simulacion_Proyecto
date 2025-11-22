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
            60f,
            new ObjetivoNivel[]
            {
                new ObjetivoNivel(12, 1)
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

        // ⭐ NIVEL 2 con límite dinámico
        GameData.configuracionActual = new ConfiguracionNivel(
            2,
            "Elixires Mágicos",
            60f,
            new ObjetivoNivel[] { new ObjetivoNivel(8, 1) }, // Fusionar hasta nivel 8
            true,        // ← limiteDinamico = true
            0.3f,        // ← velocidadDescenso = 0.3 unidades/segundo
            -2.7f,        // ← margenInicial = 0.5
            -9.5f          // ← margenMinimo = -2 (puede entrar 2 unidades en el contenedor)
        );

        SceneManager.LoadScene("Dialogos");
    }

    public void SeleccionarNivel3()
    {
        if (!GameData.NivelDesbloqueado(3))
        {
            Debug.Log("¡Debes jugar los niveles anteriores primero!");
            return;
        }

        GameData.nivelActual = 3;
        GameData.tipoDialogo = TipoDialogo.Intro;

        // ⭐ NIVEL 3 con basura
        GameData.configuracionActual = new ConfiguracionNivel(
            3,
            "Tónicos Supremos",
            120f,
            new ObjetivoNivel[] { new ObjetivoNivel(10, 1) },
            true,    // ← tieneBasura = true
            0.3f,    // ← probabilidadBasura = 30%
            2.5f     // ← rangoEliminacion = 2.5 unidades
        );

        SceneManager.LoadScene("Dialogos");
    }
}