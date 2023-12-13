using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FilterBehaviour : TableBehaviour
{

    [SerializeField] private int dominoFilterSize = 1;
    [SerializeField] private float timeForFilter = 0.5f;

    private Animator animator;

    private Slider slider;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        animator = GetComponent<Animator>();

        slider.gameObject.SetActive(false);
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
                var domino = GetObjectCarried();
                if (dominoBehaviour.trash.Blocks.Length == dominoFilterSize)
                {
                    domino.transform.position = transform.position + Vector3.up;
                }
                else
                {
                    domino.transform.position = transform.position + Vector3.down;
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
        slider.gameObject.SetActive(true);

        base.SetObjectCarried(newObjectCarried);
        animator.SetBool("IsPainting", true);
    }

    public override Transform GetObjectCarried()
    {
        slider.gameObject.SetActive(false);

        var obj = base.GetObjectCarried();
        animator.SetBool("IsPainting", false);
        return obj;
    }
}
