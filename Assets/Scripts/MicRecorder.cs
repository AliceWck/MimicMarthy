using System;
using System.Collections;
using UnityEngine;

public class MicRecorder : MonoBehaviour
{
    // Fréquence d'échantillonnage du micro (44100 Hz)
    public int sampleRate = 44100;

    // Lance un enregistrement pendant un temps et renvoie le résultat une fois qu'il est terminé
    public IEnumerator Record(float seconds, Action<AudioClip> onDone)
    {
        // Check si micro dispo
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("Aucun micro détecté");
            // Renvoie null si aucun micro n'est trouvé
            onDone?.Invoke(null);
            yield break;
        }

        // Démarre l'enregistrement avec durée et freq définies
        AudioClip clip = Microphone.Start(null, false, Mathf.CeilToInt(seconds), sampleRate);

        // Attend fin de l'enregistrement
        yield return new WaitForSeconds(seconds);

        // Stop micro
        Microphone.End(null);

        // Renvoie le son enregistré
        onDone?.Invoke(clip);
    }
}