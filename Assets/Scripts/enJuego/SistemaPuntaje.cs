using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SistemaPuntaje : MonoBehaviour
{
    public TMP_Text puntosTxt;
    public TMP_Text dineroTxt;

    public int puntajeActual;
    public int dineroActual;

    // Valores congelados al terminar el nivel; son los que muestra el EndMenu
    public int PuntajeFinal { get; private set; }
    public int DineroFinal { get; private set; }

    [Header("Pulso al cambiar")]
    public Color colorAumento = new Color(0.3f, 1f, 0.3f);
    public Color colorDisminucion = new Color(1f, 0.3f, 0.3f);
    public float duracionPulso = 0.4f;      // s que tarda en volver al color original
    public float escalaPulso = 1.25f;       // tamaño máximo del texto durante el pulso

    private int puntajeAnterior;
    private int dineroAnterior;
    private Color colorPuntos;
    private Color colorDinero;
    private Coroutine pulsoPuntos;
    private Coroutine pulsoDinero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        puntajeActual = 0;
        dineroActual = 0;
        puntajeAnterior = 0;
        dineroAnterior = 0;
        colorPuntos = puntosTxt.color;
        colorDinero = dineroTxt.color;
        ActualizarPuntos();
    }

    private void Update()
    {
        // Detectar cambios hechos desde otros scripts (Collisions)
        if (puntajeActual != puntajeAnterior)
        {
            Pulso(puntosTxt, colorPuntos, puntajeActual > puntajeAnterior, ref pulsoPuntos);
            puntajeAnterior = puntajeActual;
        }

        if (dineroActual != dineroAnterior)
        {
            Pulso(dineroTxt, colorDinero, dineroActual > dineroAnterior, ref pulsoDinero);
            dineroAnterior = dineroActual;
        }

        ActualizarPuntos();
    }

    // Se llama una sola vez al terminar el nivel; devuelve true si fue nuevo récord
    public bool FinalizarNivel()
    {
        PuntajeFinal = puntajeActual;
        DineroFinal = dineroActual;
        DatosJuego.DineroTotal += DineroFinal;

        string nivel = SceneManager.GetActiveScene().name;
        bool nuevoRecord = DatosJuego.RegistrarPuntaje(nivel, PuntajeFinal);
        if (nuevoRecord)
            Debug.Log("¡Nuevo récord en " + nivel + ": " + PuntajeFinal + "!");

        Debug.Log("Dinero total: $" + DatosJuego.DineroTotal);
        return nuevoRecord;
    }

    private void ActualizarPuntos()
    {
        puntosTxt.text = "Puntos: " + puntajeActual;
        dineroTxt.text = "$" + dineroActual;
    }

    void Pulso(TMP_Text texto, Color colorOriginal, bool aumento, ref Coroutine pulso)
    {
        // Reinicia el pulso si llega otro cambio antes de que termine
        if (pulso != null) StopCoroutine(pulso);
        pulso = StartCoroutine(AnimarPulso(texto, colorOriginal, aumento ? colorAumento : colorDisminucion));
    }

    IEnumerator AnimarPulso(TMP_Text texto, Color colorOriginal, Color colorPulso)
    {
        Transform t = texto.transform;
        float tiempo = 0f;

        while (tiempo < duracionPulso)
        {
            float p = tiempo / duracionPulso;
            texto.color = Color.Lerp(colorPulso, colorOriginal, p);
            t.localScale = Vector3.one * Mathf.Lerp(escalaPulso, 1f, p);
            tiempo += Time.deltaTime;
            yield return null;
        }

        texto.color = colorOriginal;
        t.localScale = Vector3.one;
    }
}
