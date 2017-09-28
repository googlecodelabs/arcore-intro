using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalScalable : MonoBehaviour
{
    private Vector3 originalScale;
    static private float scaleFactor = 1.0f;
    public bool handleScaleInput = false;
    public bool adjustScale = true;
    private float speed = 1f;

    // Use this for initialization
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (handleScaleInput)
        {
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos =
                    touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos =
                    touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance)
                // between the touches in each frame.
                float prevTouchDeltaMag =
                    (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag =
                    (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

	
                // range is 1 to 1000.  
                // The scale factor is v/100f meaning scale from 0.01 to 10.
                float v = scaleFactor * 100f;

                // negative delta is stretching, positive is pinching
                v -= deltaMagnitudeDiff * speed;

                scaleFactor = Mathf.Clamp(v, 1f, 1000f) / 100f;
            }
        }

        if (adjustScale)
        {
            transform.localScale = originalScale * scaleFactor;
        }
    }
}
