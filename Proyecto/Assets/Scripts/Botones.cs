using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotonNivel : MonoBehaviour
{
    [Header("Configuración")]
    public int numeroNivel = 1;

    [Header("Visual")]
    public GameObject iconoCandado;//No se uso
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
                imagen.color = Color.green;
            }
            else if (intentado)
            {
                imagen.color = new Color(1f, 0.3f, 0.3f);
            }
            else if (desbloqueado)
            {
                imagen.color = colorDesbloqueado;
            }
            else
            {
                imagen.color = colorBloqueado;
            }
        }

        //Actualizar candado (si existe)
        if (iconoCandado != null)
        {
            iconoCandado.SetActive(!desbloqueado);
        }
    }
}