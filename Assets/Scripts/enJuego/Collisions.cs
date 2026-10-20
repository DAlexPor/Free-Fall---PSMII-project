using System.Collections.Generic;
using UnityEngine;

// Va en el GameObject "Jugador" (el que tiene Collider2D + Rigidbody2D)
public class Collisions : MonoBehaviour
{
    public SistemaPuntaje puntaje;
    public LevelDesign nivel;
    public long duracionVibracion = 80;     // ms (en iOS la duración la decide el sistema)

    // Obstáculos (raíz del prefab) que el jugador tocó mientras estaba dentro de su margen
    private readonly HashSet<Transform> golpeados = new HashSet<Transform>();

    private void Start()
    {
        // Si no se asignó en el Inspector, buscarlo en la escena
        if (puntaje == null)
            puntaje = FindFirstObjectByType<SistemaPuntaje>();
        if (nivel == null)
            nivel = FindFirstObjectByType<LevelDesign>();
    }

    // Solo cuentan los choques mientras se juega (no en la intro ni después de aterrizar)
    bool EnJuego => nivel == null || nivel.EnJuego;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!EnJuego) return;

        if (collision.CompareTag("Obstaculo"))
        {
            Debug.Log("Choco");
            puntaje.puntajeActual -= 120;
            golpeados.Add(Raiz(collision));
            Vibracion.Vibrar(duracionVibracion);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!EnJuego) return;

        bool margen1 = collision.CompareTag("margen1");
        bool margen2 = collision.CompareTag("margen2");
        if (!margen1 && !margen2) return;

        // Si chocó con el obstáculo de este margen no hay recompensa
        // (Remove también limpia el registro para cuando el obstáculo se recicle del pool)
        if (golpeados.Remove(Raiz(collision))) return;

        Debug.Log("Cerca");
        if (margen1)
        {
            puntaje.puntajeActual += 100;
            puntaje.dineroActual += 50;
        }
        else
        {
            puntaje.puntajeActual += 150;
            puntaje.dineroActual += 80;
        }
    }

    // El margen y el obstáculo son hermanos dentro del prefab, así que comparten la raíz
    static Transform Raiz(Collider2D collision)
    {
        return collision.transform.parent != null ? collision.transform.parent : collision.transform;
    }
}
