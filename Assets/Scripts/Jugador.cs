using UnityEngine;
using UnityEngine.InputSystem;

public class Jugador : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D colliderJugador;
    private Camara camara;
    public Configuracion configuracion;

    public float velocidad = 10f;
    private float movimiento;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        configuracion = FindFirstObjectByType<Configuracion>();
        rb = GetComponent<Rigidbody2D>();
        camara = Camera.main.GetComponent<Camara>();
        colliderJugador = GetComponent<Collider2D>();

        rb.gravityScale = 50f;
    }

    // Update is called once per frame
    void Update()
    {
        if (configuracion.accelerometer)
        {
            // Acelerometro input
            float AcceInput = Accelerometer.current.acceleration.ReadValue().x;
            movimiento = AcceInput*2;
        }
        else
        {
            // Input en x
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            Debug.Log("Choco");
        }

        if (collision.gameObject.CompareTag("End"))
        {
            Debug.Log("Nivel de prueba COMPLETADO");
            rb.gravityScale = 0f;
        }
    }
}