using UnityEngine;

public class ObstaculoMov : MonoBehaviour
{
    public float amplitud = 1.5f;     // u a cada lado del centro
    public float frecuencia = 0.5f;   // vueltas completas por segundo

    private float fase;

    void OnEnable()
    {
        // Fase aleatoria para que no se muevan todos sincronizados al reciclarse
        fase = Random.Range(0f, 2f * Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.Sin(Time.time * frecuencia * 2f * Mathf.PI + fase) * amplitud;
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }
}