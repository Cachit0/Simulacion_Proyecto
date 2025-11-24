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

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}