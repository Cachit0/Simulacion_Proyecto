using UnityEngine;
using System.Collections.Generic;

public class Sistema : MonoBehaviour
{
    [Header("Lista de Prefabs de Bolas (12 tipos)")]
    public List<GameObject> prefabsBolas = new List<GameObject>();

    [Header("Spawn Configuration")]
    public float alturaSpawn = 4.5f;
    public int orderInLayer = 10;
    [Range(1, 12)]
    public int maxNivelSpawn = 5;

    [Header("Contenedor (ajustar manualmente en la escena)")]
    public float contenedorAncho = 8f;
    public float contenedorAlto = 9f;
    public Vector3 contenedorPosicion = Vector3.zero;

    [Header("Física")]
    public float gravedad = 9.81f;
    public float coefRestitucion = 0.4f;

    [Header("Control")]
    public KeyCode teclaSpawn = KeyCode.Mouse0;

    [Header("Fusión")]
    public float tiempoEsperaFusion = 0.1f;

    [Header("Game Over")]
    public float margenSuperior = -2.7f;
    public float tiempoGracia = 2f;

    // Variables límite dinámico
    private bool usarLimiteDinamico = false;
    private float margenDinamicoActual = -2.7f;
    private float margenInicial = -2.7f;
    private float margenMinimo = -9.5f;
    private float velocidadDescenso = 0.5f;
    private GameObject lineaLimiteVisual;

    // Variables basura
    [Header("Basura (Nivel 3)")]
    public GameObject prefabBasura;
    private bool usarBasura = false;
    private float probabilidadBasura = 0.3f;
    private float rangoEliminacionBasura = 2f;
    private List<Esfera> esferasBasura = new List<Esfera>();

    private List<Esfera> esferas = new List<Esfera>();
    private Esfera esferaPreview;
    private bool puedeSpawnear = true;
    private GameObject prefabActual;
    private List<Esfera> esferasAEliminar = new List<Esfera>();
    private List<FusionPendiente> fusionesACrear = new List<FusionPendiente>();
    private float tiempoSobrepasando = 0f;
    private bool haySobrepaso = false;

    private class FusionPendiente
    {
        public Vector3 posicion;
        public int nivel;

        public FusionPendiente(Vector3 pos, int nv)
        {
            posicion = pos;
            nivel = nv;
        }
    }

    private class Esfera
    {
        public GameObject objeto;
        public Vector2 velocidad;
        public float radio;
        public float masa;
        public int nivel;
        public bool enJuego;
        public bool marcadaParaFusion;
        public float tiempoCreacion;
        public bool esBasura;

        // ⭐ CORREGIDO: Constructor con parámetro basura
        public Esfera(GameObject obj, float r, float m, int nv, bool basura = false)
        {
            objeto = obj;
            radio = r;
            masa = m;
            nivel = nv;
            velocidad = Vector2.zero;
            enJuego = false;
            marcadaParaFusion = false;
            tiempoCreacion = Time.time;
            esBasura = basura;
        }
    }

    void Start()
    {
        if (prefabsBolas.Count == 0)
        {
            Debug.LogError("¡Asigna al menos un prefab de bola en la lista!");
            return;
        }

        if (GameData.configuracionActual != null)
        {
            usarLimiteDinamico = GameData.configuracionActual.limiteDinamico;
            velocidadDescenso = GameData.configuracionActual.velocidadDescensoLimite;
            margenInicial = GameData.configuracionActual.margenInicialLimite;
            margenMinimo = GameData.configuracionActual.margenMinimoLimite;
            margenDinamicoActual = margenInicial;
            margenSuperior = margenInicial;

            usarBasura = GameData.configuracionActual.tieneBasura;
            probabilidadBasura = GameData.configuracionActual.probabilidadBasura;
            rangoEliminacionBasura = GameData.configuracionActual.rangoEliminacionBasura;

            Debug.Log($"Límite dinámico: {usarLimiteDinamico}, Basura: {usarBasura}");
        }

        if (usarLimiteDinamico)
        {
            CrearLineaLimiteVisual();
        }

        CrearEsferaPreview();
    }

    void Update()
    {
        if (esferaPreview != null && puedeSpawnear)
        {
            Vector3 posMouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            posMouseMundo.z = 0f;

            float limiteIzq = contenedorPosicion.x - contenedorAncho / 2f + esferaPreview.radio;
            float limiteDer = contenedorPosicion.x + contenedorAncho / 2f - esferaPreview.radio;
            float xPos = Mathf.Clamp(posMouseMundo.x, limiteIzq, limiteDer);

            esferaPreview.objeto.transform.position = new Vector3(xPos, alturaSpawn, 0f);

            if (Input.GetKeyDown(teclaSpawn))
            {
                SoltarEsfera();
            }
        }

        if (usarLimiteDinamico && GameManager.instance != null && !GameManager.instance.EstaJuegoTerminado())
        {
            margenDinamicoActual -= velocidadDescenso * Time.deltaTime;
            margenDinamicoActual = Mathf.Max(margenDinamicoActual, margenMinimo);
            margenSuperior = margenDinamicoActual;
            ActualizarPosicionLineaLimite();
        }

        VerificarLimiteSuperior();
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        foreach (var esfera in esferas)
        {
            if (!esfera.enJuego) continue;
            esfera.velocidad.y -= gravedad * dt;
            Vector3 newPos = esfera.objeto.transform.position + (Vector3)(esfera.velocidad * dt);
            newPos.z = 0f;
            esfera.objeto.transform.position = newPos;
        }

        foreach (var basura in esferasBasura)
        {
            if (!basura.enJuego) continue;
            basura.velocidad.y -= gravedad * dt;
            Vector3 newPos = basura.objeto.transform.position + (Vector3)(basura.velocidad * dt);
            newPos.z = 0f;
            basura.objeto.transform.position = newPos;
        }

        for (int i = 0; i < esferas.Count; i++)
        {
            if (!esferas[i].enJuego || esferas[i].marcadaParaFusion) continue;

            for (int j = i + 1; j < esferas.Count; j++)
            {
                if (!esferas[j].enJuego || esferas[j].marcadaParaFusion) continue;
                DetectarYResolverColision(esferas[i], esferas[j]);
            }
            ColisionConContenedor(esferas[i]);
        }

        foreach (var basura in esferasBasura)
        {
            if (!basura.enJuego) continue;

            foreach (var esfera in esferas)
            {
                if (!esfera.enJuego || esfera.marcadaParaFusion) continue;
                DetectarYResolverColision(basura, esfera);
            }

            foreach (var otraBasura in esferasBasura)
            {
                if (basura == otraBasura || !otraBasura.enJuego) continue;
                DetectarYResolverColision(basura, otraBasura);
            }

            ColisionConContenedor(basura);
        }

        if (fusionesACrear.Count > 0)
        {
            foreach (var fusion in fusionesACrear)
            {
                CrearEsferaFusionada(fusion.posicion, fusion.nivel);
            }
            fusionesACrear.Clear();
        }

        if (esferasAEliminar.Count > 0)
        {
            foreach (var esfera in esferasAEliminar)
            {
                if (esfera.esBasura)
                {
                    esferasBasura.Remove(esfera);
                }
                else
                {
                    esferas.Remove(esfera);
                }

                if (esfera.objeto != null)
                {
                    Destroy(esfera.objeto);
                }
            }
            esferasAEliminar.Clear();
        }
    }

    void VerificarLimiteSuperior()
    {
        float limiteSuperior = contenedorPosicion.y + contenedorAlto / 2f + margenDinamicoActual;
        bool sobrepasaAhora = false;

        foreach (var esfera in esferas)
        {
            if (!esfera.enJuego) continue;
            float posicionSuperior = esfera.objeto.transform.position.y + esfera.radio;
            if (posicionSuperior > limiteSuperior)
            {
                sobrepasaAhora = true;
                break;
            }
        }

        if (!sobrepasaAhora)
        {
            foreach (var basura in esferasBasura)
            {
                if (!basura.enJuego) continue;
                float posicionSuperior = basura.objeto.transform.position.y + basura.radio;
                if (posicionSuperior > limiteSuperior)
                {
                    sobrepasaAhora = true;
                    break;
                }
            }
        }

        if (sobrepasaAhora)
        {
            if (!haySobrepaso)
            {
                haySobrepaso = true;
                tiempoSobrepasando = 0f;
                Debug.Log("⚠️ ¡Las bolas están sobrepasando el límite!");
            }
            else
            {
                tiempoSobrepasando += Time.deltaTime;
                if (tiempoSobrepasando >= tiempoGracia)
                {
                    TriggerGameOver();
                }
            }
        }
        else
        {
            if (haySobrepaso)
            {
                Debug.Log("✅ Las bolas volvieron al límite");
            }
            haySobrepaso = false;
            tiempoSobrepasando = 0f;
        }
    }

    void TriggerGameOver()
    {
        Debug.Log("💀 GAME OVER - Sobrepasó el límite del contenedor");

        if (GameManager.instance != null)
        {
            GameManager.instance.GameOverPorSobrepaso();
        }

        puedeSpawnear = false;

        if (esferaPreview != null && esferaPreview.objeto != null)
        {
            Destroy(esferaPreview.objeto);
            esferaPreview = null;
        }
    }

    GameObject SeleccionarPrefabAleatorio()
    {
        List<GameObject> prefabsDisponibles = new List<GameObject>();

        foreach (GameObject prefab in prefabsBolas)
        {
            if (prefab == null) continue;
            Propiedades datos = prefab.GetComponent<Propiedades>();
            if (datos != null && datos.nivel <= maxNivelSpawn)
            {
                prefabsDisponibles.Add(prefab);
            }
        }

        if (prefabsDisponibles.Count == 0)
        {
            Debug.LogWarning("No hay prefabs disponibles para el nivel especificado");
            return prefabsBolas[0];
        }

        return prefabsDisponibles[Random.Range(0, prefabsDisponibles.Count)];
    }

    GameObject ObtenerPrefabPorNivel(int nivel)
    {
        foreach (GameObject prefab in prefabsBolas)
        {
            if (prefab == null) continue;
            Propiedades datos = prefab.GetComponent<Propiedades>();
            if (datos != null && datos.nivel == nivel)
            {
                return prefab;
            }
        }

        Debug.LogWarning($"No se encontró prefab para nivel {nivel}");
        return null;
    }

    void CrearEsferaPreview()
    {
        GameObject preview;
        bool esBasura = false;

        if (usarBasura && Random.value < probabilidadBasura && prefabBasura != null)
        {
            preview = Instantiate(prefabBasura, new Vector3(0, alturaSpawn, 0), Quaternion.identity);
            preview.name = "Preview_Basura";
            esBasura = true;
            Debug.Log("🗑️ Siguiente: BASURA");
        }
        else
        {
            prefabActual = SeleccionarPrefabAleatorio();
            preview = Instantiate(prefabActual, new Vector3(0, alturaSpawn, 0), Quaternion.identity);
            preview.name = "Preview";
        }

        float radio = 0.5f;
        float masa = 1f;
        int nivel = 1;

        if (esBasura)
        {
            PropiedadesBasura datosBasura = preview.GetComponent<PropiedadesBasura>();
            if (datosBasura != null)
            {
                radio = datosBasura.radio;
                masa = datosBasura.masa;
                nivel = -1;
                preview.transform.localScale = Vector3.one * radio * 2f;
            }
        }
        else
        {
            Propiedades datos = preview.GetComponent<Propiedades>();
            if (datos != null)
            {
                radio = datos.radio;
                masa = datos.masa;
                nivel = datos.nivel;
                preview.transform.localScale = Vector3.one * radio * 2f;
            }
            else
            {
                Debug.LogWarning($"El prefab {prefabActual.name} no tiene el componente Propiedades");
            }
        }

        SpriteRenderer spriteRend = preview.GetComponent<SpriteRenderer>();
        if (spriteRend != null)
        {
            spriteRend.sortingOrder = orderInLayer;
            Color color = spriteRend.color;
            color.a = 0.5f;
            spriteRend.color = color;
        }

        esferaPreview = new Esfera(preview, radio, masa, nivel, esBasura);
    }

    void SoltarEsfera()
    {
        esferaPreview.enJuego = true;

        SpriteRenderer spriteRend = esferaPreview.objeto.GetComponent<SpriteRenderer>();
        if (spriteRend != null)
        {
            Color color = spriteRend.color;
            color.a = 1f;
            spriteRend.color = color;
        }

        if (esferaPreview.esBasura)
        {
            esferasBasura.Add(esferaPreview);
            Debug.Log("🗑️ Basura soltada");
        }
        else
        {
            esferas.Add(esferaPreview);
        }

        esferaPreview = null;

        Invoke(nameof(CrearEsferaPreview), 0.5f);
        puedeSpawnear = false;
        Invoke(nameof(HabilitarSpawn), 0.5f);
    }

    void HabilitarSpawn()
    {
        puedeSpawnear = true;
    }

    void DetectarYResolverColision(Esfera e1, Esfera e2)
    {
        Vector2 pos1 = e1.objeto.transform.position;
        Vector2 pos2 = e2.objeto.transform.position;

        float distancia = Vector2.Distance(pos1, pos2);
        float distanciaMin = e1.radio + e2.radio;

        if (distancia < distanciaMin && distancia > 0.001f)
        {
            if (!e1.esBasura && !e2.esBasura &&
                e1.nivel == e2.nivel && !e1.marcadaParaFusion && !e2.marcadaParaFusion)
            {
                float tiempoActual = Time.time;
                if (tiempoActual - e1.tiempoCreacion > tiempoEsperaFusion &&
                    tiempoActual - e2.tiempoCreacion > tiempoEsperaFusion)
                {
                    if (e1.nivel < prefabsBolas.Count)
                    {
                        Debug.Log($"¡FUSIÓN detectada! Nivel {e1.nivel} -> {e1.nivel + 1}");

                        e1.marcadaParaFusion = true;
                        e2.marcadaParaFusion = true;

                        Vector3 posFusion = (pos1 + pos2) / 2f;
                        posFusion.z = 0f;

                        fusionesACrear.Add(new FusionPendiente(posFusion, e1.nivel + 1));

                        esferasAEliminar.Add(e1);
                        esferasAEliminar.Add(e2);

                        return;
                    }
                }
            }

            Vector2 normal = (pos2 - pos1).normalized;
            float superposicion = distanciaMin - distancia;
            Vector3 correccion = normal * (superposicion / 2f + 0.001f);

            Vector3 newPos1 = e1.objeto.transform.position - (Vector3)correccion;
            Vector3 newPos2 = e2.objeto.transform.position + (Vector3)correccion;
            newPos1.z = 0f;
            newPos2.z = 0f;
            e1.objeto.transform.position = newPos1;
            e2.objeto.transform.position = newPos2;

            Vector2 tangente = new Vector2(-normal.y, normal.x);

            float v1n = Vector2.Dot(e1.velocidad, normal);
            float v1t = Vector2.Dot(e1.velocidad, tangente);
            float v2n = Vector2.Dot(e2.velocidad, normal);
            float v2t = Vector2.Dot(e2.velocidad, tangente);

            float m1 = e1.masa;
            float m2 = e2.masa;
            float v1nNew = ((m1 - m2) * v1n + 2f * m2 * v2n) / (m1 + m2);
            float v2nNew = ((m2 - m1) * v2n + 2f * m1 * v1n) / (m1 + m2);

            v1nNew *= coefRestitucion;
            v2nNew *= coefRestitucion;

            e1.velocidad = v1nNew * normal + v1t * tangente;
            e2.velocidad = v2nNew * normal + v2t * tangente;
        }
    }

    void CrearEsferaFusionada(Vector3 posicion, int nivel)
    {
        GameObject prefab = ObtenerPrefabPorNivel(nivel);
        if (prefab == null) return;

        GameObject nuevaEsfera = Instantiate(prefab, posicion, Quaternion.identity);
        nuevaEsfera.name = $"Esfera_Nivel{nivel}";

        Propiedades datos = nuevaEsfera.GetComponent<Propiedades>();
        float radio = 0.5f;
        float masa = 1f;

        if (datos != null)
        {
            radio = datos.radio;
            masa = datos.masa;
            nuevaEsfera.transform.localScale = Vector3.one * radio * 2f;
        }

        SpriteRenderer spriteRend = nuevaEsfera.GetComponent<SpriteRenderer>();
        if (spriteRend != null)
        {
            spriteRend.sortingOrder = orderInLayer;
        }

        Esfera nuevaEsferaObj = new Esfera(nuevaEsfera, radio, masa, nivel);
        nuevaEsferaObj.enJuego = true;
        nuevaEsferaObj.velocidad = Vector2.zero;
        esferas.Add(nuevaEsferaObj);

        Debug.Log($"Nueva esfera creada - Nivel: {nivel}, Radio: {radio}");

        if (usarLimiteDinamico)
        {
            margenDinamicoActual = margenInicial;
            margenSuperior = margenInicial;
            ActualizarPosicionLineaLimite();
            Debug.Log("✨ ¡Fusión! Límite reiniciado a posición inicial");
        }

        if (usarBasura)
        {
            EliminarBasuraCercana(posicion);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.RegistrarFusion(nivel);
        }
    }

    void ColisionConContenedor(Esfera esfera)
    {
        Vector3 pos = esfera.objeto.transform.position;
        float r = esfera.radio;

        float limiteIzq = contenedorPosicion.x - contenedorAncho / 2f;
        float limiteDer = contenedorPosicion.x + contenedorAncho / 2f;
        float limiteAbajo = contenedorPosicion.y - contenedorAlto / 2f;

        if (pos.x - r < limiteIzq)
        {
            pos.x = limiteIzq + r;
            esfera.velocidad.x = -esfera.velocidad.x * coefRestitucion;
        }

        if (pos.x + r > limiteDer)
        {
            pos.x = limiteDer - r;
            esfera.velocidad.x = -esfera.velocidad.x * coefRestitucion;
        }

        if (pos.y - r < limiteAbajo)
        {
            pos.y = limiteAbajo + r;
            esfera.velocidad.y = -esfera.velocidad.y * coefRestitucion;
            esfera.velocidad.x *= 0.95f;
        }

        pos.z = 0f;
        esfera.objeto.transform.position = pos;
    }

    void OnDrawGizmos()
    {
        float limiteSuperior = contenedorPosicion.y + contenedorAlto / 2f + (usarLimiteDinamico ? margenDinamicoActual : margenSuperior);
        Gizmos.color = usarLimiteDinamico ? Color.yellow : Color.red;

        float limiteIzq = contenedorPosicion.x - contenedorAncho / 2f;
        float limiteDer = contenedorPosicion.x + contenedorAncho / 2f;

        Vector3 puntoIzq = new Vector3(limiteIzq, limiteSuperior, 0f);
        Vector3 puntoDer = new Vector3(limiteDer, limiteSuperior, 0f);

        Gizmos.DrawLine(puntoIzq, puntoDer);

        for (int i = 0; i < 5; i++)
        {
            float offset = (i - 2) * 0.05f;
            Gizmos.DrawLine(
                new Vector3(limiteIzq, limiteSuperior + offset, 0f),
                new Vector3(limiteDer, limiteSuperior + offset, 0f)
            );
        }
    }

    void CrearLineaLimiteVisual()
    {
        lineaLimiteVisual = new GameObject("LineaLimite");
        LineRenderer lr = lineaLimiteVisual.AddComponent<LineRenderer>();

        lr.startWidth = 0.15f;
        lr.endWidth = 0.15f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.sortingOrder = 100;
        lr.positionCount = 2;

        ActualizarPosicionLineaLimite();
    }

    void ActualizarPosicionLineaLimite()
    {
        if (lineaLimiteVisual == null) return;

        LineRenderer lr = lineaLimiteVisual.GetComponent<LineRenderer>();
        if (lr == null) return;

        float limiteSuperior = contenedorPosicion.y + contenedorAlto / 2f + margenDinamicoActual;
        float limiteIzq = contenedorPosicion.x - contenedorAncho / 2f;
        float limiteDer = contenedorPosicion.x + contenedorAncho / 2f;

        lr.SetPosition(0, new Vector3(limiteIzq, limiteSuperior, 0f));
        lr.SetPosition(1, new Vector3(limiteDer, limiteSuperior, 0f));

        float porcentaje = (margenDinamicoActual - margenMinimo) / (margenInicial - margenMinimo);
        lr.startColor = Color.Lerp(Color.red, Color.yellow, porcentaje);
        lr.endColor = Color.Lerp(Color.red, Color.yellow, porcentaje);
    }

    void EliminarBasuraCercana(Vector3 posicionFusion)
    {
        int contadorEliminadas = 0;

        foreach (var basura in esferasBasura.ToArray())
        {
            if (!basura.enJuego) continue;

            float distancia = Vector3.Distance(basura.objeto.transform.position, posicionFusion);

            if (distancia <= rangoEliminacionBasura)
            {
                esferasAEliminar.Add(basura);
                contadorEliminadas++;
                Debug.Log($"🗑️💥 Basura eliminada por fusión cercana");
            }
        }

        if (contadorEliminadas > 0)
        {
            Debug.Log($"✨ ¡Eliminadas {contadorEliminadas} basuras con la fusión!");
        }
    }
}