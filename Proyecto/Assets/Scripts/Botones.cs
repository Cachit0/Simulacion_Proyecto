using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotonNivel : MonoBehaviour
{
    [Header("Configuración")]
    public int numeroNivel = 1; // 1, 2 o 3

    [Header("Visual")]
    public GameObject iconoCandado; // Opcional: sprite de candado
    public Color colorBloqueado = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    public Color colorDesbloqueado = Color.white;

    private Button boton;
    private Image imagen;
    private TextMeshProUGUI texto;

    void Start()
    {
        boton = GetComponent<Button>();
        imagen = GetComponent<Image>();
        texto = GetComponentInChildren<TextMeshProUGUI>();

        ActualizarEstado();
    }

    void OnEnable()
    {
        // Actualizar cada vez que se active la escena
        ActualizarEstado();
    }

    void ActualizarEstado()
    {
        bool desbloqueado = GameData.NivelDesbloqueado(numeroNivel);
        bool intentado = GameData.nivelesIntentados[numeroNivel - 1];
        bool completado = GameData.nivelesCompletados[numeroNivel - 1];

        // Actualizar botón
        if (boton != null)
        {
            boton.interactable = desbloqueado;
        }

        // Actualizar color del contenedor/imagen
        if (imagen != null)
        {
            if (completado)
            {
                imagen.color = Color.green; // Verde si completó
            }
            else if (intentado)
            {
                imagen.color = new Color(1f, 0.3f, 0.3f); // Rojo si falló
            }
            else if (desbloqueado)
            {
                imagen.color = colorDesbloqueado; // Color normal
            }
            else
            {
                imagen.color = colorBloqueado; // Gris si bloqueado
            }
        }

        // Actualizar candado (si existe)
        if (iconoCandado != null)
        {
            iconoCandado.SetActive(!desbloqueado);
        }

        // Actualizar texto SIN cambiar color
        if (texto != null)
        {
            if (completado)
            {
                texto.text = $"Nivel {numeroNivel}"; // Checkmark si completó
            }
            else if (intentado)
            {
                texto.text = $"Nivel {numeroNivel}"; // X si intentó pero falló
            }
            else if (desbloqueado)
            {
                texto.text = $"Nivel {numeroNivel}";
            }
            else
            {
                texto.text = $"Nivel {numeroNivel}"; // Candado si bloqueado
            }
            // NO cambiar texto.color - mantener el color original
        }
    }
}