using UnityEngine;
using System.Collections;

public class StarParticle : MonoBehaviour
{

    RectTransform rtransform;
    void Start()
    {
        rtransform = GetComponent<RectTransform>();
        ResetPosition();
    }

    void Update()
    {
        if (rtransform.anchoredPosition.y < -800)
        {
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        float rate = Random.Range(0, 1f);
        rtransform.rigidbody2D.mass = (1 - rate) * 0.5f;

        rtransform.anchoredPosition = new Vector2(0, Random.Range(200, 300));
        rtransform.rigidbody2D.AddForce(new Vector2(Random.Range(-80, 80), 0));

        int newSize = (int)(rate * 50f);
        rtransform.sizeDelta = new Vector2(newSize, newSize);
    }
}
