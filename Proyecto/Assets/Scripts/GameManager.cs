using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoEncargo;
    public TextMeshProUGUI textoObjetivos; // ⭐ CAMBIADO: Un solo texto en lugar de panel
    public GameObject panelResultado; // ⭐ NUEVO: Panel de Victoria/Derrota
    public TextMeshProUGUI textoResultado; // ⭐ NUEVO: Texto "VICTORIA" o "DERROTA"

    [Header("Estado del Juego")]
    private float tiempoRestante;
    private bool juegoActivo = false;
    private bool juegoTerminado = false;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Ocultar panel de resultado al inicio
        if (panelResultado != null)
        {
            panelResultado.SetActive(false);
        }

        IniciarNivel();
    }

    void Update()
    {
        if (juegoActivo && !juegoTerminado)
        {
            // Actualizar temporizador
            tiempoRestante -= Time.deltaTime;
            ActualizarUITiempo();

            // Verificar si se acabó el tiempo
            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                TerminarJuego(false); // Perdió por tiempo
            }

            // Verificar si completó todos los objetivos
            if (VerificarObjetivosCompletados())
            {
                TerminarJuego(true); // Ganó
            }
        }
    }

    void IniciarNivel()
    {
        if (GameData.configuracionActual == null)
        {
            Debug.LogError("No hay configuración de nivel cargada!");
            return;
        }

        // Inicializar tiempo
        tiempoRestante = GameData.configuracionActual.tiempoLimite;

        // Actualizar UI
        ActualizarUIEncargo();
        ActualizarUIObjetivos(); // ⭐ SIMPLIFICADO
        ActualizarUITiempo();

        // Iniciar el juego después de un pequeño delay
        Invoke(nameof(ActivarJuego), 1f);
    }

    void ActivarJuego()
    {
        juegoActivo = true;
    }

    void ActualizarUITiempo()
    {
        if (textoTiempo != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            // Cambiar color si queda poco tiempo
            if (tiempoRestante <= 10f)
            {
                textoTiempo.color = Color.red;
            }
            else if (tiempoRestante <= 30f)
            {
                textoTiempo.color = Color.yellow;
            }
            else
            {
                textoTiempo.color = Color.black;
            }
        }
    }

    void ActualizarUIEncargo()
    {
        if (textoEncargo != null)
        {
            textoEncargo.text = "Encargo: " + GameData.configuracionActual.nombreEncargo;
        }
    }

    void ActualizarUIObjetivos()
    {
        if (textoObjetivos == null) return;

        // ⭐ Asegurar tamaño de fuente correcto
        textoObjetivos.fontSize = 24;
        textoObjetivos.enableAutoSizing = false;

        // Construir texto con todos los objetivos
        string textoCompleto = "Objetivos:\n";

        foreach (var objetivo in GameData.configuracionActual.objetivos)
        {
            string colorHex = objetivo.EstaCompletado() ? "00FF00" : "000000"; // Verde si completado, blanco si no
            textoCompleto += $"<color=#{colorHex}>• Bola Nivel {objetivo.nivelBola}: {objetivo.cantidadActual}/{objetivo.cantidadRequerida}</color>\n";
        }

        textoObjetivos.text = textoCompleto;
    }

    // Este método se llamará desde Sistema.cs cuando se fusione una bola
    public void RegistrarFusion(int nivelBola)
    {
        if (!juegoActivo || juegoTerminado) return;

        // Buscar si este nivel está en los objetivos
        foreach (var objetivo in GameData.configuracionActual.objetivos)
        {
            if (objetivo.nivelBola == nivelBola)
            {
                objetivo.cantidadActual++;
                ActualizarUIObjetivos();
                Debug.Log($"¡Objetivo actualizado! Bola nivel {nivelBola}: {objetivo.cantidadActual}/{objetivo.cantidadRequerida}");
                break;
            }
        }
    }

    bool VerificarObjetivosCompletados()
    {
        foreach (var objetivo in GameData.configuracionActual.objetivos)
        {
            if (!objetivo.EstaCompletado())
            {
                return false;
            }
        }
        return true;
    }

    void TerminarJuego(bool victoria)
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        juegoActivo = false;

        // ⭐ MARCAR que intentó este nivel
        GameData.nivelesIntentados[GameData.nivelActual - 1] = true;

        // ⭐ GUARDAR resultado del nivel actual
        if (victoria)
        {
            GameData.nivelesCompletados[GameData.nivelActual - 1] = true;
            Debug.Log($"¡Nivel {GameData.nivelActual} COMPLETADO!");
        }
        else
        {
            Debug.Log($"Nivel {GameData.nivelActual} FALLADO");
        }

        // ⭐ MOSTRAR resultado en pantalla (no ir a diálogos todavía)
        MostrarResultadoEnPantalla(victoria);

        // ⭐ Esperar antes de decidir qué hacer
        Invoke(nameof(DecidirSiguientePaso), 3f);
    }

    void MostrarResultadoEnPantalla(bool victoria)
    {
        if (panelResultado != null && textoResultado != null)
        {
            panelResultado.SetActive(true);

            if (victoria)
            {
                textoResultado.text = "¡VICTORIA!";
                textoResultado.color = Color.green;
            }
            else
            {
                textoResultado.text = "DERROTA";
                textoResultado.color = Color.red;
            }
        }
    }

    void DecidirSiguientePaso()
    {
        // ⭐ Verificar si ya intentó los 3 niveles
        bool intentoTodos = GameData.IntentóTodosLosNiveles();

        // Debug para ver el estado
        Debug.Log($"=== DECISIÓN SIGUIENTE PASO ===");
        Debug.Log($"Intentó todos: {intentoTodos}");
        Debug.Log($"Nivel 1 - Intentado: {GameData.nivelesIntentados[0]}, Completado: {GameData.nivelesCompletados[0]}");
        Debug.Log($"Nivel 2 - Intentado: {GameData.nivelesIntentados[1]}, Completado: {GameData.nivelesCompletados[1]}");
        Debug.Log($"Nivel 3 - Intentado: {GameData.nivelesIntentados[2]}, Completado: {GameData.nivelesCompletados[2]}");

        if (intentoTodos)
        {
            // Ya jugó los 3 niveles, ahora verificar resultado final
            if (GameData.TodosLosNivelesCompletados())
            {
                // ⭐ Ganó los 3 → Victoria Final
                Debug.Log("→ IR A DIÁLOGO DE VICTORIA");
                GameData.tipoDialogo = TipoDialogo.Victoria;
                GameData.gano = true;
                SceneManager.LoadScene("Dialogos");
            }
            else
            {
                // ⭐ Falló al menos uno → Derrota Final
                Debug.Log("→ IR A DIÁLOGO DE DERROTA");
                GameData.tipoDialogo = TipoDialogo.Derrota;
                GameData.gano = false;
                SceneManager.LoadScene("Dialogos");
            }
        }
        else
        {
            // ⭐ Aún no ha intentado los 3 niveles → Volver a selección
            Debug.Log("→ VOLVER A NIVELES (Aún faltan niveles por jugar)");
            SceneManager.LoadScene("Niveles");
        }
    }

    void IrADialogos()
    {
        SceneManager.LoadScene("Dialogos");
    }

    // Método público para pausar/despausar (por si lo necesitas)
    public void PausarJuego(bool pausar)
    {
        juegoActivo = !pausar;
    }
}