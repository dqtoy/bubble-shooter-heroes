using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MeshAnimationAdapter : MonoBehaviour
{

    public Color color = Color.white;
    TextMesh textMesh = null;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        if (textMesh != null)
            color = textMesh.color;
    }

    void Update()
    {
        DoUpdate();
    }

    void DoUpdate()
    {
        if (textMesh != null && (textMesh.color != color))
        {
            if (textMesh.color != color) textMesh.color = color;
        }
    }
}