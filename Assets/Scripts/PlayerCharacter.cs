using System.Collections;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {
    // Vitesse de déplacement et de rotation personnage
    public float moveSpeed = 2.5f;
    public float turnSpeed = 360f;

    [Header("Valeurs du paramètre 'blend feeling'")] // Ajuster en testant, todo check ça sur l'UI Unity
    public float winBlend = 0f;

    Animator animator;
    Vector3 homePos;
    Quaternion homeRot;

 
    void Awake() {
        // Récupère l'animator et garde la position de départ du personnage
        animator = GetComponent<Animator>();
        homePos = transform.position;
        homeRot = transform.rotation;
    }

    // Déplace le perso vers l'avant puis le fait regarder la caméra
    public IEnumerator GoToFront(Transform front) {
        yield return WalkTo(front.position);
        yield return TurnTo(FacingCamera());
    }

    // Ramène le perso à sa positionn et son orientation du début
    public IEnumerator GoHome() {
        yield return WalkTo(homePos);
        yield return TurnTo(homeRot);
    }

    // Gère le déplacement du personnage jusqu'à une position donnée, mettre UI Unity
    IEnumerator WalkTo(Vector3 target) {
        animator.ResetTrigger("idle");
        animator.SetTrigger("run");

        while (true) {
            // Ignore la différence de hauteur pour garder le personnage au même niveau
            Vector3 flatTarget = new Vector3(target.x, transform.position.y, target.z);
            if (Vector3.Distance(transform.position, flatTarget) < 0.05f) break;

            // Oriente le personnage dans la direction de mvt
            Vector3 dir = flatTarget - transform.position;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

            // Fait avancer le personnage
            transform.position = Vector3.MoveTowards(
                transform.position, flatTarget, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Animation d'attente quand arrivé
        animator.ResetTrigger("run");
        animator.SetTrigger("idle");
    }


    // Fait tourner le perso vers une orientation particulière
    IEnumerator TurnTo(Quaternion rot) {
        while (Quaternion.Angle(transform.rotation, rot) > 1f) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, rot, turnSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Calcule l'orientation qu'il faut pour regarder la caméra
    Quaternion FacingCamera() {
        Vector3 dir = Camera.main.transform.position - transform.position;
        dir.y = 0;
        return Quaternion.LookRotation(dir);
    }

    // Lance l'animation de victoire avec la valeur de blend définie, check le schéma mvt
    public void PlayWin() {
        animator.SetFloat("blend feeling", winBlend);
        animator.SetTrigger("feel");
    }

    // Lance l'animation de défaite
    public void PlayLose() {
        animator.SetTrigger("fall");
    }
}