using Models;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class SeparatorBehaviour : TableBehaviour
{

    [SerializeField] private float timeForSeparate = 0.5f;
    [SerializeField] private GameObject trashPrefab;

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
            slider.value = timeOnTable / timeForSeparate;

            if (timeOnTable > timeForSeparate)
            {
                var trash = GetObjectCarried();
                var trashSeparate = TrashUtils.SeparateTrash(trash.GetComponent<TrashBehaviour>().trash);
                Destroy(trash.gameObject);

                GameObject organicTrash = Instantiate(trashPrefab, transform.position + Vector3.up, Quaternion.identity);
                organicTrash.GetComponent<TrashBehaviour>().SetTrash(trashSeparate[0]);

                GameObject metalTrash = Instantiate(trashPrefab, transform.position + Vector3.down, Quaternion.identity);
                metalTrash.GetComponent<TrashBehaviour>().SetTrash(trashSeparate[1]);
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
