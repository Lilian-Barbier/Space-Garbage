using UnityEngine;
using UnityEngine.UI;

public class PainterBehaviour : TableBehaviour
{

    [SerializeField] private bool isPainterRed;
    [SerializeField] private bool isPainterBlue;

    [SerializeField] private float timeForPaint;

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
        if (isPainterRed || isPainterBlue)
        {
            if (objectCarried != null)
            {
                var dominoBehaviour = objectCarried.GetComponent<TrashBehaviour>();

                if (dominoBehaviour.trash.GetColor() != Enums.BlockColor.Failed)
                {
                    timeOnTable += Time.deltaTime;
                    slider.value = timeOnTable / timeForPaint;

                    if (timeOnTable > timeForPaint)
                    {
                        dominoBehaviour.AddColor(isPainterBlue, isPainterRed);
                        timeOnTable = 0;
                    }
                }
                else
                {
                    slider.value = 0;
                }

            }
            else
            {
                slider.value = 0;
            }
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
