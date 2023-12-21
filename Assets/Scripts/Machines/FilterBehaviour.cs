using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterBehaviour : TableBehaviour
{

    [SerializeField] private int dominoFilterSize = 1;
    [SerializeField] private float timeForFilter = 0.5f;
    private int maxSizeFilter = 4;

    private Animator animator;

    private Slider slider;
    [SerializeField] List<SpriteRenderer> hologramBlock;

    AudioSource audioSource;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        slider.gameObject.SetActive(false);

        for (int i = 0; i < dominoFilterSize; i++)
        {
            hologramBlock[i].color = new Color(hologramBlock[1].color.r, hologramBlock[1].color.g, hologramBlock[1].color.b, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (objectCarried != null)
        {
            var dominoBehaviour = objectCarried.GetComponent<TrashBehaviour>();

            timeOnTable += Time.deltaTime;
            slider.value = timeOnTable / timeForFilter;

            if (timeOnTable > timeForFilter)
            {
                var domino = base.GetObjectCarried();

                animator.SetBool("IsPainting", false);
                slider.gameObject.SetActive(false);

                if (dominoBehaviour.GetTrashSize() == dominoFilterSize)
                {
                    domino.transform.position = transform.position + Vector3.up;
                }
                else
                {
                    domino.transform.position = transform.position + Vector3.down;
                }

                audioSource.Stop();

            }
        }
        else
        {
            slider.value = 0;
        }
    }

    public void ChangeFilterSize()
    {
        dominoFilterSize++;

        if (dominoFilterSize > maxSizeFilter)
        {
            dominoFilterSize = 1;
        }

        for (int i = 0; i < maxSizeFilter; i++)
        {
            if (i < dominoFilterSize)
            {
                hologramBlock[i].color = new Color(hologramBlock[1].color.r, hologramBlock[1].color.g, hologramBlock[1].color.b, 1f);
            }
            else
            {
                hologramBlock[i].color = new Color(hologramBlock[1].color.r, hologramBlock[1].color.g, hologramBlock[1].color.b, 0.2f);
            }
        }
    }

    public override void SetObjectCarried(Transform newObjectCarried)
    {
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
