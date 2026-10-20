using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    private GeneradorObstaculos generador;
    private GameObject prefab;

    public void Inicializar(GeneradorObstaculos generador, GameObject prefab)
    {
        this.generador = generador;
        this.prefab = prefab;
    }

    // Update is called once per frame
    void Update()
    {
        // El jugador cae, así que los obstáculos suben en pantalla
        transform.position += Vector3.up * generador.Velocidad * Time.deltaTime;

        // Al salir por arriba vuelve al pool
        if (transform.position.y > generador.LimiteSuperior)
            generador.Devolver(this, prefab);
    }
}
