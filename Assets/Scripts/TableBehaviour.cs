using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBehaviour : MonoBehaviour
{
    protected Transform objectCarried;
    protected float timeOnTable;

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
        timeOnTable = 0;
        objectCarried = newObjectCarried;
        objectCarried.GetComponent<Collider2D>().isTrigger = true;
        objectCarried.position = transform.position + new Vector3(0, 0.25f, 0);
    }

    public Transform GetObjectCarried()
    {
        timeOnTable = 0;
        var tmpObjectCarried = objectCarried;
        objectCarried = null;
        return tmpObjectCarried;
    }

    public bool CanAcceptObject()
    {
        return objectCarried == null;
    }
}
