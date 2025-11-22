using UnityEngine;

public class PropiedadesBasura : MonoBehaviour
{
    [Header("Propiedades de Basura")]
    public float radio = 0.3f;
    public float masa = 0.5f;
    public float rangoEliminacion = 2f; // Distancia a la que una fusión puede eliminarla

    [Header("Visual")]
    public Color colorBasura = new Color(0.3f, 0.3f, 0.3f, 1f); // Gris oscuro

    void Start()
    {
        // Aplicar color oscuro
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = colorBasura;
        }
    }
}