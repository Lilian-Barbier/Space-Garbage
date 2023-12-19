using UnityEngine;

public class ButtonBobbing : MonoBehaviour
{

    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, Mathf.Sin(Time.time * 4f) * 10f - 3f, 0);
    }
}
