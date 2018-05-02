using UnityEngine;
using System.Collections;

public class MultiResolution : MonoBehaviour
{


    private Transform m_tranform;
    private static float BASE_WIDTH = 480f;
    private static float BASE_HEIGHT = 800f;
    private float baseRatio;
    private float percentScale;
    void Start()
    {
        m_tranform = transform;
        setScale();
    }
    void setScale()
    {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8
        baseRatio = (float)BASE_WIDTH / BASE_HEIGHT * Screen.height;
        percentScale = Screen.width / baseRatio;
        m_tranform.localScale = new Vector3(m_tranform.localScale.x * percentScale, m_tranform.localScale.y, m_tranform.localScale.z);
#endif
    }


}
