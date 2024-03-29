using UnityEngine;
using UnityEngine.UI;

public class ScrollingMosaic : MonoBehaviour
{

    [SerializeField] private RawImage image;
    [SerializeField] private float xSpeed = 1f;
    [SerializeField] private float ySpeed = 1f;


    // Update is called once per frame
    void Update()
    {
        image.uvRect = new Rect(image.uvRect.position + new Vector2(xSpeed, ySpeed) * Time.deltaTime, image.uvRect.size);
    }
}
