using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Vector3 dispenserPosition;
    public SpacePieceSpawn spacePieceSpawn;
    public float speed;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position - transform.right, speed * Time.deltaTime);
        if (Vector2.Distance(dispenserPosition, transform.position) > 15)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Blocks"))
        {
            if (!TutorielManager.Instance.tutorialHookPassed)
            {
                TutorielManager.Instance.tutorialHookPassed = true;
                TutorielManager.Instance.NextTutorial();
            }

            other.gameObject.transform.position = dispenserPosition;
            other.gameObject.GetComponent<Collider2D>().isTrigger = false;
            spacePieceSpawn.RemovePiece(other.gameObject);
            audioSource.Play();
        }
    }
}
