using UnityEngine;
using UnityEngine.InputSystem;

public class Jugador : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D colliderJugador;
    private Camara camara;

    public float velocidad = 10f;
    private float movimiento;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        camara = Camera.main.GetComponent<Camara>();
        colliderJugador = GetComponent<Collider2D>();

        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
    }

    // Update is called once per frame
    void Update()
    {
        if (Accelerometer.current != null && Accelerometer.current.enabled)
        {
            // Acelerometro input
            float acceInput = Accelerometer.current.acceleration.ReadValue().x;
            if (Mathf.Abs(acceInput) < 0.05f) acceInput = 0f;   // zona muerta para evitar temblor
            movimiento = Mathf.Clamp(acceInput * 2f, -1f, 1f);
        }
        else
        {
            // Normal input (teclado en PC / Editor)
            movimiento = Input.GetAxisRaw("Horizontal");
        }
    }

    private void FixedUpdate()
    {
        // Calcular movimiento en horizontal
        Vector2 posicion = rb.position;
        posicion.x += movimiento * velocidad * Time.fixedDeltaTime;

        // Limitar movimiento hasta los bordes de pantalla
        float bordeIzquierdo = camara.BordeIzquierdo();
        float bordeDerecho = camara.BordeDerecho();
        float mitadX = colliderJugador.bounds.extents.x;
        float limiteIzquierdo = bordeIzquierdo + mitadX + camara.margenX;
        float limiteDerecho = bordeDerecho - mitadX - camara.margenX;
        posicion.x = Mathf.Clamp(posicion.x, limiteIzquierdo, limiteDerecho);

        // Aplicar posicion
        rb.MovePosition(posicion);
    }
}