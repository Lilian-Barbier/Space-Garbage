using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class SeparatorBehaviour : TableBehaviour
{

    [SerializeField] private float timeForSeparate = 0.5f;
    [SerializeField] private GameObject trashPrefab;

    private Animator animator;

    private Slider slider;
    AudioSource audioSource;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        slider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (objectCarried != null)
        {
            var dominoBehaviour = objectCarried.GetComponent<TrashBehaviour>();

            timeOnTable += Time.deltaTime;
            slider.value = timeOnTable / timeForSeparate;

            if (timeOnTable > timeForSeparate)
            {
                var trash = base.GetObjectCarried();

                animator.SetBool("IsPainting", false);
                audioSource.Stop();

                slider.gameObject.SetActive(false);

                var trashSeparate = TrashUtils.SeparateTrash(trash.GetComponent<TrashBehaviour>().trash);
                Destroy(trash.gameObject);

                if (trashSeparate[0].GetTrashSize() != 0)
                {
                    GameObject organicTrash = Instantiate(trashPrefab, transform.position + Vector3.up, Quaternion.identity);
                    organicTrash.GetComponent<TrashBehaviour>().SetTrash(trashSeparate[0]);
                }

                if (trashSeparate[1].GetTrashSize() != 0)
                {
                    GameObject metalTrash = Instantiate(trashPrefab, transform.position + Vector3.down, Quaternion.identity);
                    metalTrash.GetComponent<TrashBehaviour>().SetTrash(trashSeparate[1]);
                }

                if (!TutorielManager.Instance.tutorialDividerPassed)
                {
                    TutorielManager.Instance.tutorialDividerPassed = true;
                    TutorielManager.Instance.NextTutorial();
                }
            }
        }
        else
        {
            slider.value = 0;
        }
    }

    public override void SetObjectCarried(Transform newObjectCarried)
    {
        TrashBehaviour trashBehaviour = newObjectCarried.GetComponent<TrashBehaviour>();

        if (trashBehaviour.IsOnlyOneMaterialTrash())
        {
            newObjectCarried.GetComponent<Collider2D>().isTrigger = false;
            newObjectCarried.GetComponent<Rigidbody2D>().isKinematic = false;
            return;
        }

        slider.gameObject.SetActive(true);

        base.SetObjectCarried(newObjectCarried);
        animator.SetBool("IsPainting", true);
        audioSource.Play();

    }

    public override Transform GetObjectCarried()
    {
        return null;
    }
}
