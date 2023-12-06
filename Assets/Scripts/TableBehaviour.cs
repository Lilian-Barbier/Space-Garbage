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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Blocks") && objectCarried == null)
        {
            SetObjectCarried(collision.transform);
        }
    }

    public virtual void SetObjectCarried(Transform newObjectCarried)
    {
        timeOnTable = 0;
        objectCarried = newObjectCarried;
        objectCarried.GetComponent<Collider2D>().isTrigger = true;
        objectCarried.GetComponent<Rigidbody2D>().isKinematic = true;
        objectCarried.position = transform.position + new Vector3(0, 0.25f, 0);
    }

    public virtual Transform GetObjectCarried()
    {
        timeOnTable = 0;
        objectCarried.GetComponent<Collider2D>().isTrigger = false;
        objectCarried.GetComponent<Rigidbody2D>().isKinematic = false;
        var tmpObjectCarried = objectCarried;
        objectCarried = null;
        return tmpObjectCarried;
    }

    public virtual Transform GetReferenceObjectCarried()
    {
        return objectCarried;
    }

    public bool CanAcceptObject()
    {
        return objectCarried == null;
    }
}
