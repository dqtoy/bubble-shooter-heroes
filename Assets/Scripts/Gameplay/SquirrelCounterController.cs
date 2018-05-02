using UnityEngine;
using System.Collections;

public class SquirrelCounterController : MonoBehaviour
{

    public GameObject facePrefab;
    public static SquirrelCounterController instance = null;

    void Start()
    {
        instance = this;
    }

    public static void CreateSquirrelFace(Vector3 pos)
    {
        if (instance != null)
        {
            instance.CreateSquirrelFaceAt(pos);
        }
    }

    void CreateSquirrelFaceAt(Vector3 pos)
    {
        GameObject squirrel_face = Instantiate(facePrefab) as GameObject;
        squirrel_face.transform.SetParent(transform, false);
        squirrel_face.transform.position = pos;

        squirrel_face.gameObject.SetActive(true);
    }
}
