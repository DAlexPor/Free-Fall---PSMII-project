using System.Collections.Generic;
using UnityEngine;

public class GeneradorObstaculos : MonoBehaviour
{
    public GameObject[] prefabs;
    public LevelDesign nivel;

    [Header("Velocidad de u/s de inicio a fin")]
    public float velocidadMinima = 3f;          // u/s al inicio de la caída
    public float velocidadMaxima = 15f;         // u/s
    [Header("Unidades en pantalla")]
    public float unidadesPorMetro = 0.03f;      // convierte los m/s de la caída a u/s en pantalla
    public float distanciaEntreObstaculos = 4f; // u entre un obstáculo y el siguiente
    public float variacionDistancia = 0.5f;     // ±50% aleatorio sobre la distancia
    [Header("Margenes de pantalla")]
    public float margenX = 0.5f;                // unidades despegados del borde
    public float margenY = 1f;                  // unidades fuera de pantalla para aparecer/desaparecer

    private Camera cam;
    private Camara camara;
    private float distanciaRecorrida;
    private float siguienteDistancia;
    private readonly Dictionary<GameObject, Queue<Obstaculo>> pool = new Dictionary<GameObject, Queue<Obstaculo>>();

    public float Velocidad { get; private set; }
    public float LimiteSuperior => cam.transform.position.y + cam.orthographicSize + margenY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        camara = cam.GetComponent<Camara>();
        siguienteDistancia = CalcularDistancia();
    }

    // Update is called once per frame
    void Update()
    {
        // Durante la intro y al llegar al suelo no se mueven ni se generan obstáculos
        if (!nivel.EnJuego)
        {
            Velocidad = 0f;
            return;
        }

        // Velocidad en pantalla ligada a la velocidad de caída
        Velocidad = Mathf.Clamp(velocidadMinima + nivel.VelocidadActual * unidadesPorMetro, velocidadMinima, velocidadMaxima);

        // Generar por distancia recorrida: la separación se mantiene aunque la caída se acelere
        distanciaRecorrida += Velocidad * Time.deltaTime;
        if (distanciaRecorrida >= siguienteDistancia)
        {
            distanciaRecorrida -= siguienteDistancia;
            siguienteDistancia = CalcularDistancia();
            Generar();
        }
    }

    float CalcularDistancia()
    {
        return distanciaEntreObstaculos * Random.Range(1f - variacionDistancia, 1f + variacionDistancia);
    }

    void Generar()
    {
        if (prefabs.Length == 0) return;

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        float x = Random.Range(camara.BordeIzquierdo() + margenX, camara.BordeDerecho() - margenX);
        float y = cam.transform.position.y - cam.orthographicSize - margenY;

        Obstaculo obstaculo = Obtener(prefab);
        obstaculo.transform.position = new Vector3(x, y, 0f);
    }

    // Reutiliza un obstáculo inactivo del pool o crea uno nuevo
    Obstaculo Obtener(GameObject prefab)
    {
        if (pool.TryGetValue(prefab, out Queue<Obstaculo> cola) && cola.Count > 0)
        {
            Obstaculo reciclado = cola.Dequeue();
            reciclado.gameObject.SetActive(true);
            return reciclado;
        }

        GameObject nuevo = Instantiate(prefab, transform);
        if (!nuevo.TryGetComponent(out Obstaculo obstaculo))
            obstaculo = nuevo.AddComponent<Obstaculo>();
        obstaculo.Inicializar(this, prefab);
        return obstaculo;
    }

    public void Devolver(Obstaculo obstaculo, GameObject prefab)
    {
        obstaculo.gameObject.SetActive(false);

        if (!pool.TryGetValue(prefab, out Queue<Obstaculo> cola))
        {
            cola = new Queue<Obstaculo>();
            pool[prefab] = cola;
        }
        cola.Enqueue(obstaculo);
    }
}
