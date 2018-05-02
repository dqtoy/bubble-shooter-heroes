using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LittleSquirrelController : MonoBehaviour
{

    public static bool noSquirrelFlying = true;

    public static List<LittleSquirrelController> publicList = new List<LittleSquirrelController>();

    public Animator animator;
    void Start()
    {

        animation.Play("Dropping");

        if (GlobalData.gameMode == GlobalData.GameMode.PUZZLE_MODE)
            StartCoroutine(moveTo(new Vector3(Random.Range(-2.0f, 2.0f), -2.9f, transform.position.z), 3));
        else if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
        {
            int index = publicList.Count;

            if (index >= 6)
            {
                LittleSquirrelController needDead = publicList[publicList.Count - 6];
                needDead.animation.Play("FadeoutAndDie");

                index = index % 6;
            }

            StartCoroutine(moveTo(new Vector3(-1.74f + index * 0.7f, -2.9f, transform.position.z), 3));

            publicList.Add(this);
        }

    }

    public IEnumerator moveTo(Vector3 destination, float duration)
    {
        noSquirrelFlying = false;
        float timeThrough = 0.0f;
        Vector3 initialPosition = transform.position;
        while (Vector3.Distance(transform.position, destination) >= 0.05)
        {

            timeThrough += Time.deltaTime;
            Vector3 target = Vector3.Lerp(initialPosition, destination, timeThrough / duration);
            transform.position = target;
            yield return null;
        }
        transform.position = destination;
        animation.Play("Landing");
        noSquirrelFlying = true;
    }

    public IEnumerator moveToAndDie(Vector3 destination, float duration)
    {
        noSquirrelFlying = false;
        float timeThrough = 0.0f;
        Vector3 initialPosition = transform.position;
        while (Vector3.Distance(transform.position, destination) >= 0.05)
        {
            timeThrough += Time.deltaTime;
            Vector3 target = Vector3.Lerp(initialPosition, destination, timeThrough / duration);
            transform.position = target;
            yield return null;
        }
        transform.position = destination;
        animation.Play("Landing");
        noSquirrelFlying = true;

        Destroy(gameObject, 0.5f);
    }


}
