using UnityEngine;

// Datos globales del proyecto, guardados en PlayerPrefs para que sobrevivan al cerrar el juego
public static class DatosJuego
{
    const string ClaveDinero = "DineroTotal";
    const string ClaveRecord = "Record_";   // + nombre de la escena (un récord por nivel)

    public static int DineroTotal
    {
        get => PlayerPrefs.GetInt(ClaveDinero, 0);
        set
        {
            PlayerPrefs.SetInt(ClaveDinero, value);
            PlayerPrefs.Save();
        }
    }

    public static int ObtenerRecord(string nivel)
    {
        return PlayerPrefs.GetInt(ClaveRecord + nivel, 0);
    }

    // Guarda el puntaje solo si supera el récord anterior; devuelve true si fue nuevo récord
    public static bool RegistrarPuntaje(string nivel, int puntaje)
    {
        if (puntaje <= ObtenerRecord(nivel)) return false;

        PlayerPrefs.SetInt(ClaveRecord + nivel, puntaje);
        PlayerPrefs.Save();
        return true;
    }
}
