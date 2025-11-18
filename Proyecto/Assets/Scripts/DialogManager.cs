using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("🎨 Referencias UI")]
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI textoNombrePersonaje;
    public GameObject indicadorContinuar;

    // 🆕 Tres posiciones para personajes
    public Image imagenPersonajeIzquierda;
    public Image imagenPersonajeCentro;
    public Image imagenPersonajeDerecha;
    public Image imagenFondo;

    [Header("⚙️ Configuración")]
    public bool efectoEscritura = true;
    public float velocidadEscritura = 0.05f;

    [Header("📖 ESCENAS DE DIÁLOGO - Configura aquí")]
    [Space(10)]
    public EscenaDialogo introNivel1;
    public EscenaDialogo introNivel2;
    public EscenaDialogo introNivel3;

    [Space(10)]
    public EscenaDialogo victoriaFinal;
    public EscenaDialogo derrota;

    // Variables internas
    private LineaDialogo[] dialogosActuales;
    private int indiceActual = 0;
    private bool escribiendo = false;
    private Coroutine coroutineEscritura;

    void Start()
    {
        CargarDialogos();

        if (dialogosActuales != null && dialogosActuales.Length > 0)
        {
            MostrarDialogo();
        }
        else
        {
            Debug.LogError("❌ No hay diálogos configurados");
            Finalizar();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (escribiendo)
            {
                CompletarTexto();
            }
            else
            {
                Siguiente();
            }
        }

        if (indicadorContinuar != null)
        {
            indicadorContinuar.SetActive(!escribiendo);
        }
    }

    void CargarDialogos()
    {
        EscenaDialogo escenaSeleccionada = null;

        switch (GameData.tipoDialogo)
        {
            case TipoDialogo.Intro:
                if (GameData.nivelActual == 1)
                    escenaSeleccionada = introNivel1;
                else if (GameData.nivelActual == 2)
                    escenaSeleccionada = introNivel2;
                else if (GameData.nivelActual == 3)
                    escenaSeleccionada = introNivel3;
                break;

            case TipoDialogo.Victoria:
                escenaSeleccionada = victoriaFinal;
                break;

            case TipoDialogo.Derrota:
                escenaSeleccionada = derrota;
                break;
        }

        if (escenaSeleccionada != null)
        {
            dialogosActuales = escenaSeleccionada.dialogos;

            if (escenaSeleccionada.fondoInicial != null && imagenFondo != null)
            {
                imagenFondo.sprite = escenaSeleccionada.fondoInicial;
            }

            Debug.Log($"✓ Cargada escena: {escenaSeleccionada.nombreEscena}");
        }
        else
        {
            Debug.LogWarning($"⚠️ No hay escena configurada para Nivel {GameData.nivelActual}, Tipo: {GameData.tipoDialogo}");
        }
    }

    void MostrarDialogo()
    {
        if (indiceActual >= dialogosActuales.Length) return;

        LineaDialogo lineaActual = dialogosActuales[indiceActual];

        // 🔧 RESETEAR TODAS LAS POSICIONES PRIMERO
        OcultarTodasLasImagenes();

        // 🔧 CONFIGURAR SEGÚN EL MODO
        switch (lineaActual.modoPersonajes)
        {
            case ModoPersonajes.SinPersonajes:
                break;

            case ModoPersonajes.UnPersonaje:
                MostrarPersonajeEnPosicion(
                    lineaActual.personajePrincipal,
                    lineaActual.posicionPersonajePrincipal,
                    lineaActual.principalHabla
                );
                break;

            case ModoPersonajes.DosPersonajes:
                MostrarPersonajeEnPosicion(
                    lineaActual.personajePrincipal,
                    lineaActual.posicionPersonajePrincipal,
                    lineaActual.principalHabla
                );
                MostrarPersonajeEnPosicion(
                    lineaActual.personajeSecundario,
                    lineaActual.posicionPersonajeSecundario,
                    lineaActual.secundarioHabla
                );
                break;
        }

        // Cambiar fondo si hay uno nuevo
        if (imagenFondo != null && lineaActual.imagenFondo != null)
        {
            imagenFondo.sprite = lineaActual.imagenFondo;
        }

        // Efecto de escritura para el DIÁLOGO
        if (efectoEscritura)
        {
            if (coroutineEscritura != null)
                StopCoroutine(coroutineEscritura);

            coroutineEscritura = StartCoroutine(EfectoEscritura(lineaActual.texto));
        }
        else
        {
            textoDialogo.text = lineaActual.texto;
            escribiendo = false;
        }

        // 🔧 MOSTRAR NOMBRE AL FINAL (como el diálogo)
        if (textoNombrePersonaje != null)
        {
            if (!string.IsNullOrEmpty(lineaActual.nombrePersonaje))
            {
                textoNombrePersonaje.text = lineaActual.nombrePersonaje;
                textoNombrePersonaje.gameObject.SetActive(true);
            }
            else
            {
                textoNombrePersonaje.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator EfectoEscritura(string textoCompleto)
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in textoCompleto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
    }

    void CompletarTexto()
    {
        if (coroutineEscritura != null)
        {
            StopCoroutine(coroutineEscritura);
        }

        textoDialogo.text = dialogosActuales[indiceActual].texto;
        escribiendo = false;
    }

    void Siguiente()
    {
        indiceActual++;

        if (indiceActual < dialogosActuales.Length)
        {
            MostrarDialogo();
        }
        else
        {
            Finalizar();
        }
    }

    void Finalizar()
    {
        if (GameData.tipoDialogo == TipoDialogo.Intro)
        {
            SceneManager.LoadScene("Juego");
        }
        else if (GameData.tipoDialogo == TipoDialogo.Victoria)
        {
            GameData.ReiniciarProgreso();
            SceneManager.LoadScene("Menu");
        }
        else if (GameData.tipoDialogo == TipoDialogo.Derrota)
        {
            GameData.ReiniciarProgreso();
            SceneManager.LoadScene("Menu");
        }
    }

    void OcultarTodasLasImagenes()
    {
        if (imagenPersonajeIzquierda != null)
            imagenPersonajeIzquierda.gameObject.SetActive(false);

        if (imagenPersonajeCentro != null)
            imagenPersonajeCentro.gameObject.SetActive(false);

        if (imagenPersonajeDerecha != null)
            imagenPersonajeDerecha.gameObject.SetActive(false);
    }

    //Auxiliares

    void MostrarPersonajeEnPosicion(Sprite sprite, PosicionPersonaje posicion, bool habla)
    {
        if (sprite == null) return;

        Image imagenSeleccionada = null;

        // Seleccionar la imagen según la posición
        switch (posicion)
        {
            case PosicionPersonaje.Izquierda:
                imagenSeleccionada = imagenPersonajeIzquierda;
                break;
            case PosicionPersonaje.Centro:
                imagenSeleccionada = imagenPersonajeCentro;
                break;
            case PosicionPersonaje.Derecha:
                imagenSeleccionada = imagenPersonajeDerecha;
                break;
        }

        if (imagenSeleccionada != null)
        {
            imagenSeleccionada.sprite = sprite;
            imagenSeleccionada.gameObject.SetActive(true);

            // Destacar si está hablando
            Color color = habla
                ? Color.white
                : new Color(0.6f, 0.6f, 0.6f, 1f); // Atenuado
            imagenSeleccionada.color = color;
        }
    }
}