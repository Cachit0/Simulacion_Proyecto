using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        // Si ya existe una instancia, destruye este objeto
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Establece esta instancia como la única
        instance = this;

        // Evita que se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);

        // Obtiene el componente AudioSource
        audioSource = GetComponent<AudioSource>();

        // Configura el AudioSource para que se repita
        if (audioSource != null)
        {
            audioSource.loop = true;

            // Si no está sonando, inicia la música
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    // Métodos opcionales para controlar la música
    public void PausarMusica()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ReanudarMusica()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }

    public void CambiarVolumen(float volumen)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volumen);
        }
    }

    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (audioSource != null && nuevaMusica != null)
        {
            audioSource.clip = nuevaMusica;
            audioSource.Play();
        }
    }
}