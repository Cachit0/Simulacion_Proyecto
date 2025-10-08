using UnityEngine;

public class Propiedades : MonoBehaviour
{
    [Header("Propiedades de la Bola")]
    public int nivel = 1; // Nivel de la bola (1-12)
    public float radio = 0.5f;
    public float masa = 1f;

    [Header("Visual (Opcional)")]
    public Color colorBola = Color.white;

    void Start()
    {
        // Aplicar color si hay SpriteRenderer
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null && colorBola != Color.white)
        {
            sprite.color = colorBola;
        }
    }
}