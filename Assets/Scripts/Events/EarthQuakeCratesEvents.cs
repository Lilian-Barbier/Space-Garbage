using UnityEngine;

public class EarthQuakeCratesEvents : MonoBehaviour, IEvent
{
    double eventDuration;
    double spawnBlocksByHalfSecond;
    GameObject[] crates;

    public void StartEvent()
    {
        eventDuration = 3;
        spawnBlocksByHalfSecond = 1;

        crates = GameObject.FindGameObjectsWithTag("Crate");

        InvokeRepeating(nameof(InstantiateBlocks), 0.5f, 0.5f);
    }


    void InstantiateBlocks()
    {
        CameraShake.Instance.Shake(0.025f, 0.4f);

        eventDuration--;

        for (int i = 0; i < spawnBlocksByHalfSecond; i++)
        {
            var crate = crates[Random.Range(0, crates.Length)].GetComponent<CrateBehaviour>();
            var block = crate.GetObject(instantiateWithColliderTrigger: false);

            block.transform.position = crate.transform.position;
        }

        if (eventDuration < 1)

        {
            CancelInvoke(nameof(InstantiateBlocks));
        }
    }

}
