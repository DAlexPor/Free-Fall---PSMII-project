using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Va en el GameObject "EndMenu" (dentro del Canvas UI)
public class EndMenu : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text puntuacionTxt;
    public TMP_Text recordTxt;
    public TMP_Text recordLabel;        // el texto "Mejor"; cambia a "¡Nuevo récord!" cuando aplica
    public TMP_Text dineroTxt;
    public TMP_Text dineroTotalTxt;

    [Header("Aparición")]
    public float duracionFade = 0.5f;   // s

    public int escenaMenu = 0;          // índice de "Menú" en Build Settings

    private CanvasGroup grupo;
    private bool mostrando;

    void Awake()
    {
        if (!TryGetComponent(out grupo))
            grupo = gameObject.AddComponent<CanvasGroup>();

        // Oculto durante el juego (si se dejó inactivo en el editor, Awake corre hasta Mostrar)
        if (!mostrando)
            gameObject.SetActive(false);
    }

    public void Mostrar(SistemaPuntaje puntaje, bool nuevoRecord)
    {
        string nivel = SceneManager.GetActiveScene().name;

        // Se usan los valores congelados en FinalizarNivel, no los actuales,
        // para que coincidan con los que se usaron para decidir el récord
        puntuacionTxt.text = puntaje.PuntajeFinal.ToString();
        recordTxt.text = DatosJuego.ObtenerRecord(nivel).ToString();
        dineroTxt.text = "+$" + puntaje.DineroFinal;
        dineroTotalTxt.text = "Total: $" + DatosJuego.DineroTotal;
        if (recordLabel != null && nuevoRecord)
            recordLabel.text = "¡Nuevo récord!";

        mostrando = true;
        gameObject.SetActive(true);
        StartCoroutine(Aparecer());
    }

    IEnumerator Aparecer()
    {
        // Sin interacción hasta que termine el fade para evitar toques accidentales
        grupo.interactable = false;
        float tiempo = 0f;

        while (tiempo < duracionFade)
        {
            grupo.alpha = tiempo / duracionFade;
            tiempo += Time.unscaledDeltaTime;
            yield return null;
        }

        grupo.alpha = 1f;
        grupo.interactable = true;
    }

    // Botón "¡De nuevo!"
    public void Reintentar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Botón "Salir al menú"
    public void SalirAlMenu()
    {
        SceneManager.LoadScene(escenaMenu);
    }
}
