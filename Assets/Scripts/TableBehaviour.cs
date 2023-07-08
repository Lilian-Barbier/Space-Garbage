using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBehaviour : MonoBehaviour
{
    private Transform objectCarried;


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Blocks") && objectCarried == null)
        {
            SetObjectCarried(collision.transform);
        }
    }

    public void SetObjectCarried(Transform newObjectCarried)
    {
        objectCarried = newObjectCarried;
        objectCarried.GetComponent<Collider2D>().isTrigger = true;
        objectCarried.position = transform.position + new Vector3(0, 0.25f, 0);
    }

    public Transform GetObjectCarried()
    {
        var tmpObjectCarried = objectCarried;
        objectCarried = null;
        return tmpObjectCarried;
    }

    public bool CanAcceptObject()
    {
        return objectCarried == null;
    }
}
