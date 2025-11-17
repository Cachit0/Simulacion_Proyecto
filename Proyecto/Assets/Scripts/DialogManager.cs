using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nombrePersonaje; // Opcional
    public GameObject panelDialogo;
    public GameObject indicadorContinuar; // ⭐ NUEVO: El objeto con "Click para continuar"

    [Header("Efectos Visuales (Opcional)")]
    public bool usarEfectoEscritura = true;
    public float velocidadEscritura = 0.05f;

    [Header("Diálogos Introducción Niveles")]
    public string[] introNivel1;
    public string[] introNivel2;
    public string[] introNivel3;

    [Header("Diálogos Victoria Final")]
    public string[] victoriaFinal; // ⭐ Cuando completa los 3 niveles

    [Header("Diálogos Derrota")]
    public string[] derrota; // ⭐ Cuando falla cualquier nivel

    private string[] dialogosActuales;
    private int indiceActual = 0;
    private bool mostrandoTexto = false;

    void Start()
    {

        CargarDialogos();
        MostrarDialogo();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            // Si está mostrando texto con efecto, completarlo instantáneamente
            SiguienteDialogo();
        }

        // ⭐ Mostrar/ocultar indicador según estado
        if (indicadorContinuar != null)
        {
            indicadorContinuar.SetActive(!mostrandoTexto);
        }
    }

    void CargarDialogos()
    {
        // Selecciona los diálogos según nivel y tipo
        if (GameData.tipoDialogo == TipoDialogo.Intro)
        {
            // Diálogos de introducción por nivel
            switch (GameData.nivelActual)
            {
                case 1:
                    dialogosActuales = introNivel1;
                    break;
                case 2:
                    dialogosActuales = introNivel2;
                    break;
                case 3:
                    dialogosActuales = introNivel3;
                    break;
                default:
                    dialogosActuales = new string[] { "..." };
                    break;
            }
        }
        else if (GameData.tipoDialogo == TipoDialogo.Victoria)
        {
            // ⭐ Victoria FINAL (completó los 3 niveles)
            dialogosActuales = victoriaFinal;
        }
        else if (GameData.tipoDialogo == TipoDialogo.Derrota)
        {
            // ⭐ Derrota (falló un nivel)
            dialogosActuales = derrota;
        }

        // Validación
        if (dialogosActuales == null || dialogosActuales.Length == 0)
        {
            dialogosActuales = new string[] { "..." };
            Debug.LogWarning("No hay diálogos configurados para este nivel/tipo");
        }
    }

    void MostrarDialogo()
    {
        if (indiceActual < dialogosActuales.Length)
        {
            string textoActual = dialogosActuales[indiceActual];
            dialogueText.text = textoActual;
            mostrandoTexto = false;

        }
    }

    void SiguienteDialogo()
    {
        indiceActual++;

        if (indiceActual < dialogosActuales.Length)
        {
            MostrarDialogo();
        }
        else
        {
            FinalizarDialogos();
        }
    }

    void FinalizarDialogos()
    {
        // Decide a dónde ir después
        if (GameData.tipoDialogo == TipoDialogo.Intro)
        {
            // Termina la intro, va al juego
            SceneManager.LoadScene("Juego");
        }
        else if (GameData.tipoDialogo == TipoDialogo.Victoria)
        {
            // ⭐ Victoria final → Reinicia progreso y vuelve al menú
            GameData.ReiniciarProgreso();
            SceneManager.LoadScene("Menu");
        }
        else // Derrota
        {
            // ⭐ Derrota final → Reinicia progreso y vuelve al menú
            GameData.ReiniciarProgreso();
            SceneManager.LoadScene("Menu");
        }
    }
}

// ⭐ EJEMPLO de cómo configurar los diálogos en el Inspector:
/*
INTRO NIVEL 1:
[0] "¡Buenos días! Soy Esmeralda, la nueva aprendiz de la tienda."
[1] "Mi primer encargo es preparar pociones de curación."
[2] "Debo conseguir los ingredientes correctos... ¡Vamos allá!"

INTRO NIVEL 2:
[0] "Segundo día de trabajo. Hoy el encargo es más complejo."
[1] "Necesito preparar elixires mágicos más potentes."
[2] "¡No puedo fallar ahora!"

INTRO NIVEL 3:
[0] "¡El último encargo! Este es el más importante."
[1] "Si logro completarlo, podré quedarme en la tienda."
[2] "¡Todo depende de esto!"

VICTORIA FINAL:
[0] "¡Lo logré! ¡Completé los tres encargos!"
[1] "La dueña de la tienda está impresionada."
[2] "Me ofreció quedarme como asistente permanente."
[3] "¡Este es el inicio de mi carrera como bruja de tienda!"

DERROTA:
[0] "Oh no... no pude completar el encargo a tiempo."
[1] "La dueña de la tienda parece decepcionada."
[2] "Tendré que intentarlo de nuevo si quiero quedarme aquí."
[3] "¡No me rendiré!"
*/