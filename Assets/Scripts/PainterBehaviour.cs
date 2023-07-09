using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PainterBehaviour : TableBehaviour
{

    [SerializeField] private bool isPainterRed;
    [SerializeField] private bool isPainterBlue;

    [SerializeField] private float timeForPaint;

    private Slider slider;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPainterRed|| isPainterBlue)
        {
            if (objectCarried != null)
            {
                var dominoBehaviour = objectCarried.GetComponent<DominoBehavior>();

                if(dominoBehaviour.domino.GetColor() != Enums.BlockColor.Failed)
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

}
