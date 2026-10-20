using UnityEngine;

// Vibración del dispositivo. En Android permite elegir duración; en iOS usa la vibración estándar del sistema
public static class Vibracion
{
    const string ClaveActiva = "VibracionActiva";

    // Para un interruptor en Configuración; se guarda entre sesiones
    public static bool Activa
    {
        get => PlayerPrefs.GetInt(ClaveActiva, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(ClaveActiva, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    static AndroidJavaObject vibrador;
    static int versionSdk;

    static AndroidJavaObject Vibrador()
    {
        if (vibrador == null)
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var actividad = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                vibrador = actividad.Call<AndroidJavaObject>("getSystemService", "vibrator");

            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                versionSdk = version.GetStatic<int>("SDK_INT");
        }
        return vibrador;
    }

    // VibrationEffect.createOneShot(ms, DEFAULT_AMPLITUDE = -1)
    static AndroidJavaObject CrearEfecto(long milisegundos)
    {
        using (var efecto = new AndroidJavaClass("android.os.VibrationEffect"))
            return efecto.CallStatic<AndroidJavaObject>("createOneShot", milisegundos, -1);
    }
#endif

    public static void Vibrar(long milisegundos = 80)
    {
        if (!Activa) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaObject v = Vibrador();
            if (versionSdk >= 33)
            {
                // Se marca como respuesta táctil (USAGE_TOUCH) para que siga el ajuste de
                // "vibración táctil" y no el de multimedia, que suele estar apagado
                using (var efecto = CrearEfecto(milisegundos))
                using (var claseAtributos = new AndroidJavaClass("android.os.VibrationAttributes"))
                using (var atributos = claseAtributos.CallStatic<AndroidJavaObject>("createForUsage",
                           claseAtributos.GetStatic<int>("USAGE_TOUCH")))
                    v.Call("vibrate", efecto, atributos);
            }
            else if (versionSdk >= 26)
            {
                // Antes de Android 13 el equivalente es AudioAttributes.USAGE_ASSISTANCE_SONIFICATION
                using (var efecto = CrearEfecto(milisegundos))
                using (var claseAudio = new AndroidJavaClass("android.media.AudioAttributes"))
                using (var builder = new AndroidJavaObject("android.media.AudioAttributes$Builder"))
                {
                    builder.Call<AndroidJavaObject>("setUsage",
                        claseAudio.GetStatic<int>("USAGE_ASSISTANCE_SONIFICATION")).Dispose();
                    using (var atributos = builder.Call<AndroidJavaObject>("build"))
                        v.Call("vibrate", efecto, atributos);
                }
            }
            else
            {
                v.Call("vibrate", milisegundos);
            }
        }
        catch (System.Exception e)
        {
            // Si falla la API nativa se usa la vibración estándar
            // (esta llamada también hace que Unity agregue el permiso VIBRATE al manifest)
            Debug.LogWarning("Vibración nativa no disponible: " + e.Message);
            Handheld.Vibrate();
        }
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }
}
