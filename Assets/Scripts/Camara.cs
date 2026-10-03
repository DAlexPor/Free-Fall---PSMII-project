using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering;

public class Camara : MonoBehaviour
{
    public Transform jugador;
    public float areaGameplay = 10f;
    public float margenX = 0f;
    public float margenY = 0f;
    private Camera camara;

    private void Start()
    {
        camara = GetComponent<Camera>();
        AjustarCamara();
    }
    private void Update()
    {
        AjustarCamara(); // test
    }

    private void LateUpdate()
    {
        // Fijar jugador en la camara
        float posY = jugador.position.y - camara.orthographicSize + margenY;
        transform.position = new Vector3(transform.position.x, posY, transform.position.z);
    }

    void AjustarCamara()
    {
        // Crea la pantalla responsiva de manera que el tamaño de juego sea la misma para cada resolución de pantalla
        camara.orthographicSize = areaGameplay / (2f * camara.aspect);
    }
    public float BordeIzquierdo()
    {
        return transform.position.x - areaGameplay / 2f;
    }

    public float BordeDerecho()
    {
        return transform.position.x + areaGameplay / 2f;
    }
}
