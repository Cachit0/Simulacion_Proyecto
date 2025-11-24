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
        //Opcional: Reiniciar progreso al volver a niveles desde menú
        //GameData.ReiniciarProgreso();
        SceneManager.LoadSceneAsync("Niveles");
    }

    public void Juego()
    {
        SceneManager.LoadSceneAsync("Juego");
    }

    //NIVEL 1: Límite fijo en -2.7f
    public void SeleccionarNivel1()
    {
        GameData.nivelActual = 1;
        GameData.tipoDialogo = TipoDialogo.Intro;

        GameData.configuracionActual = new ConfiguracionNivel(
            1,
            "Lobo Hombre",
            180f,
            new ObjetivoNivel[]
            {
                new ObjetivoNivel(9, 1),
                new ObjetivoNivel(7, 3)
            },
            false,      // ← limiteDinamico = false (FIJO)
            0f,         // ← velocidadDescenso = 0 (no se mueve)
            -2.7f,      // ← margenInicial = -2.7
            -2.7f       // ← margenMinimo = -2.7 (se queda ahí)
        );

        SceneManager.LoadScene("Dialogos");
    }

    //NIVEL 2: Límite dinámico (desciende)
    public void SeleccionarNivel2()
    {
        if (!GameData.NivelDesbloqueado(2))
        {
            Debug.Log("¡Debes jugar el Nivel 1 primero!");
            return;
        }

        GameData.nivelActual = 2;
        GameData.tipoDialogo = TipoDialogo.Intro;

        GameData.configuracionActual = new ConfiguracionNivel(
            2,
            "Ectoplasma ? ? ?",
            190f,
            new ObjetivoNivel[]
            {
                new ObjetivoNivel(8, 2),
                new ObjetivoNivel(7, 3)
            },
            true,        // ← limiteDinamico = true (SE MUEVE)
            0.3f,        // ← velocidadDescenso = 0.3 unidades/segundo
            -2.7f,       // ← margenInicial = -2.7
            -9.5f        // ← margenMinimo = -9.5
        );

        SceneManager.LoadScene("Dialogos");
    }

    //NIVEL 3: Límite fijo + basura
    public void SeleccionarNivel3()
    {
        if (!GameData.NivelDesbloqueado(3))
        {
            Debug.Log("¡Debes jugar los niveles anteriores primero!");
            return;
        }

        GameData.nivelActual = 3;
        GameData.tipoDialogo = TipoDialogo.Intro;

        GameData.configuracionActual = new ConfiguracionNivel(
            3,
            "Todo por el dinero",
            300f,
            new ObjetivoNivel[]
            {
                new ObjetivoNivel(10, 1),
                new ObjetivoNivel(9, 1),
                new ObjetivoNivel(4, 8)
            },
            false,
            0f,
            -2.7f,
            -2.7f
        );

        GameData.configuracionActual.tieneBasura = true;
        GameData.configuracionActual.probabilidadBasura = 0.3f;
        GameData.configuracionActual.rangoEliminacionBasura = 2.5f;

        SceneManager.LoadScene("Dialogos");
    }
}