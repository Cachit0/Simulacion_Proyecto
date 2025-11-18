using UnityEngine;

public static class GameData
{
    // Información del nivel actual
    public static int nivelActual = 1; // 1, 2 o 3

    // Tipo de diálogo a mostrar
    public static TipoDialogo tipoDialogo = TipoDialogo.Intro;

    // Resultado del juego
    public static bool gano = false;

    // Configuración del nivel (se establece al seleccionar nivel)
    public static ConfiguracionNivel configuracionActual;

    // Rastreo de progreso de todos los niveles
    public static bool[] nivelesCompletados = new bool[3]; // [nivel1, nivel2, nivel3]
    public static bool[] nivelesIntentados = new bool[3];   // Para saber si ya intentó el nivel

    // Método para verificar si completó todos los niveles
    public static bool TodosLosNivelesCompletados()
    {
        return nivelesCompletados[0] && nivelesCompletados[1] && nivelesCompletados[2];
    }

    // Verificar si algún nivel fue fallado
    public static bool AlgunNivelFallado()
    {
        for (int i = 0; i < 3; i++)
        {
            if (nivelesIntentados[i] && !nivelesCompletados[i])
            {
                return true;
            }
        }
        return false;
    }

    // Verificar si un nivel está desbloqueado
    public static bool NivelDesbloqueado(int nivel)
    {
        if (nivel == 1) return true; // Nivel 1 siempre desbloqueado
        if (nivel == 2) return nivelesIntentados[0]; // Nivel 2 si INTENTÓ nivel 1
        if (nivel == 3) return nivelesIntentados[0] && nivelesIntentados[1]; // Nivel 3 si INTENTÓ 1 y 2
        return false;
    }

    // Verificar si ya intentó los 3 niveles
    public static bool IntentóTodosLosNiveles()
    {
        return nivelesIntentados[0] && nivelesIntentados[1] && nivelesIntentados[2];
    }

    // Reiniciar progreso (para empezar desde cero)
    public static void ReiniciarProgreso()
    {
        nivelesCompletados[0] = false;
        nivelesCompletados[1] = false;
        nivelesCompletados[2] = false;
        nivelesIntentados[0] = false;
        nivelesIntentados[1] = false;
        nivelesIntentados[2] = false;
        nivelActual = 1;
        tipoDialogo = TipoDialogo.Intro;
        gano = false;
    }
}

// Enums y clases existentes
public enum TipoDialogo
{
    Intro,
    Victoria,
    Derrota
}

[System.Serializable]
public class ConfiguracionNivel
{
    public int numeroNivel;
    public string nombreEncargo; // "Pociones de Curación", "Elixires Mágicos", etc.
    public float tiempoLimite; // En segundos
    public ObjetivoNivel[] objetivos; // Qué debe conseguir

    public ConfiguracionNivel(int nivel, string nombre, float tiempo, ObjetivoNivel[] objs)
    {
        numeroNivel = nivel;
        nombreEncargo = nombre;
        tiempoLimite = tiempo;
        objetivos = objs;
    }
}

[System.Serializable]
public class ObjetivoNivel
{
    public int nivelBola; // Nivel de la bola que necesita (ej: nivel 5 = bola grande)
    public int cantidadRequerida; // Cuántas necesita
    public int cantidadActual; // Cuántas lleva

    public ObjetivoNivel(int nivel, int cantidad)
    {
        nivelBola = nivel;
        cantidadRequerida = cantidad;
        cantidadActual = 0;
    }

    public bool EstaCompletado()
    {
        return cantidadActual >= cantidadRequerida;
    }
}