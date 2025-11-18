using UnityEngine;

[System.Serializable]
public class LineaDialogo
{
    [Header("¿Quién habla?")]
    public string nombrePersonaje = "Esmeralda";
    public Color colorNombre = Color.white;

    [Header("¿Qué dice?")]
    [TextArea(3, 5)]
    public string texto = "Escribe aquí el diálogo...";

    [Header("⚙️ Configuración de Personajes")]
    [Tooltip("¿Cuántos personajes aparecen en esta escena?")]
    public ModoPersonajes modoPersonajes = ModoPersonajes.UnPersonaje;

    [Header("Personaje Principal (o Único)")]
    public Sprite personajePrincipal;
    [Tooltip("Solo si hay UN personaje")]
    public PosicionPersonaje posicionPersonajePrincipal = PosicionPersonaje.Centro;
    [Tooltip("¿Este personaje está hablando?")]
    public bool principalHabla = true;

    [Header("Segundo Personaje (solo si hay DOS)")]
    public Sprite personajeSecundario;
    public PosicionPersonaje posicionPersonajeSecundario = PosicionPersonaje.Derecha;
    [Tooltip("¿Este personaje está hablando?")]
    public bool secundarioHabla = false;

    [Header("Fondo (Opcional)")]
    public Sprite imagenFondo;
}

public enum ModoPersonajes
{
    SinPersonajes,  // Solo texto/narración
    UnPersonaje,    // Un personaje (puedes elegir posición)
    DosPersonajes   // Dos personajes (puedes elegir sus posiciones)
}

public enum PosicionPersonaje
{
    Izquierda,
    Centro,
    Derecha
}