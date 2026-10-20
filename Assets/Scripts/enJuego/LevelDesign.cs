using System.Collections;
using UnityEngine;
using TMPro;

public class LevelDesign : MonoBehaviour
{
    public TMP_Text alturaTxt;
    public TMP_Text tiempoTxt;
    public SistemaPuntaje puntaje;
    public Jugador jugador;
    public Camara camara;
    public EndMenu endMenu;
    public float retrasoMenuFinal = 1.5f;   // s entre aterrizar y mostrar EndMenu

    public float alturaInicial = 10000f;    // m
    public float gravedad = 9.81f;          // m/s²
    public float velocidad = 55f;           // m/s (paracaidista boca abajo)
    public float escalaTiempo = 1f;         // >1 acelera la caída para el gameplay

    private float tiempo;
    private float tiempoJuego;
    private float alturaActual;
    private float velocidadActual;

    Animator animator;
    private bool isFinished = false;
    private bool iniciado = false;

    public float AlturaActual => alturaActual;
    public float VelocidadActual => velocidadActual;
    public bool EnJuego => iniciado && alturaActual > 0f;   // false durante la intro y después de aterrizar

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        if (jugador == null) jugador = FindFirstObjectByType<Jugador>();
        if (camara == null) camara = FindFirstObjectByType<Camara>();
        // EndMenu se oculta a sí mismo en Awake, por eso se busca incluyendo inactivos
        if (endMenu == null) endMenu = FindFirstObjectByType<EndMenu>(FindObjectsInactive.Include);

        alturaActual = alturaInicial;

        if (puntaje == null)
            puntaje = FindFirstObjectByType<SistemaPuntaje>();

        tiempoTxt.text = "00:00.00";
        ActualizarTexto();

        if (animator != null)
        {
            // Intro: el Animator tiene el control hasta que el clip llame a IniciarNivel() con un Animation Event
            jugador.enabled = false;
            camara.enabled = false;
            // Al reactivarlo en el final sigue desde el estado de la intro en vez de reiniciarla
            animator.keepAnimatorStateOnDisable = true;
        }
        else
        {
            IniciarNivel();
        }
    }

    // Llamado por un Animation Event al final del clip de intro
    public void IniciarNivel()
    {
        if (iniciado) return;
        iniciado = true;

        // Apagado durante el gameplay para que no sobrescriba lo que mueven Jugador y Camara
        if (animator != null) animator.enabled = false;
        jugador.enabled = true;
        camara.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!EnJuego) return;

        tiempo += Time.deltaTime * escalaTiempo;
        tiempoJuego += Time.deltaTime;

        // Caída con resistencia del aire (solución analítica)
        //float k = gravedad / velocidad;
        //velocidadActual = velocidad * Tanh(k * tiempo);
        //float distancia = (velocidad * velocidad / gravedad) * LnCosh(k * tiempo);

        // Caída sin resistencia del aire (Menos realista)
        velocidadActual = gravedad * tiempo;
        float distancia = 0.5f * gravedad * tiempo * tiempo;

        alturaActual = Mathf.Max(0f, alturaInicial - distancia);
        ActualizarTexto();

        // Actualizar tiempo de juego
        int minutos = Mathf.FloorToInt(tiempoJuego / 60f);
        int segundos = Mathf.FloorToInt(tiempoJuego % 60f);
        int milisegundos = Mathf.FloorToInt((tiempoJuego * 100f) % 100f);
        tiempoTxt.text = string.Format("{0:00}:{1:00}.{2:00}", minutos, segundos, milisegundos);

        if (alturaActual <= 0f)
        {
            // Fin del nivel: este bloque corre una sola vez porque Update sale al inicio cuando EnJuego es false
            Debug.Log("Terminó");
            // Los scripts sueltan el control y el Animator toma el relevo
            jugador.enabled = false;
            camara.enabled = false;
            if (animator != null)
            {
                animator.enabled = true;
                animator.SetBool("finished", isFinished = true);
            }
            bool nuevoRecord = puntaje.FinalizarNivel();
            StartCoroutine(MostrarMenuFinal(nuevoRecord));
        }
    }

    // Espera a que se vea la animación de aterrizaje antes de mostrar el menú
    IEnumerator MostrarMenuFinal(bool nuevoRecord)
    {
        yield return new WaitForSeconds(retrasoMenuFinal);
        endMenu.Mostrar(puntaje, nuevoRecord);
    }

    void ActualizarTexto()
    {
        alturaTxt.text = alturaActual.ToString("N0") + " m";
    }

    static float Tanh(float x) => (float)System.Math.Tanh(x);

    // ln(cosh(x)) estable para x grandes (cosh se desborda alrededor de x > 89)
    static float LnCosh(float x)
    {
        x = Mathf.Abs(x);
        return x + Mathf.Log(1f + Mathf.Exp(-2f * x)) - 0.6931472f; // ln 2
    }
}
