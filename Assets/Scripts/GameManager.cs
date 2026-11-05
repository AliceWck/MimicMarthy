using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerCharacter[] players;
    public Transform frontPoint;
    public AudioClip imposedSound;
    public AudioSource audioSource;
    public MicRecorder mic;
    public float recordSeconds = 4f;

    AudioClip[] recordings;
    int[] scores;

    IEnumerator Start()
    {
        recordings = new AudioClip[players.Length];
        scores = new int[players.Length];
        yield return new WaitForSeconds(1f);

        // D'aboord : chaque joueur s'enregistre, todo check pour après le multi
        for (int i = 0; i < players.Length; i++)
        {
            int idx = i;
            Debug.Log($"Joueur {i + 1} : écoute le son...");
            audioSource.PlayOneShot(imposedSound);
            yield return new WaitForSeconds(imposedSound.length + 0.5f);

            Debug.Log("3... 2... 1... GO !");
            yield return new WaitForSeconds(2f);

            Debug.Log("ENREGISTREMENT");
            yield return mic.Record(recordSeconds, clip => recordings[idx] = clip);
            Debug.Log("Terminé");
            yield return new WaitForSeconds(1f);
        }

        // 2 : passage devant la scène
        for (int i = 0; i < players.Length; i++)
        {
            yield return players[i].GoToFront(frontPoint);

            if (recordings[i] != null)
            {
                audioSource.clip = recordings[i];
                audioSource.Play();
                yield return new WaitForSeconds(recordings[i].length);
            }

            yield return players[i].GoHome();
            yield return CollectRatings(i);
        }

        // 3 : podium de fin, à check aussi
        int best = System.Array.IndexOf(scores, scores.Max());
        int pire = System.Array.IndexOf(scores, scores.Min());
        players[best].PlayWin();
        players[pire].PlayLose();
        Debug.Log($"Gagnant : Joueur {best + 1} / Dernier : Joueur {pire + 1}");
    }

    // Pour l'instant, notes aléatoires. A remplacer par l'UI et le multi
    IEnumerator CollectRatings(int performer)
    {
        for (int voter = 0; voter < players.Length; voter++)
        {
            if (voter == performer) continue;
            scores[performer] += Random.Range(0, 11);
        }
        yield return new WaitForSeconds(0.5f);
    }
}