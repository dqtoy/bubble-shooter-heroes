using UnityEngine;
using System.Collections;

public class SquirrelFaceController : MonoBehaviour
{

    public Vector3 targetPosition;
    void OnEnable()
    {

        float range = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(targetPosition.x, targetPosition.y));
        float trueTime = range / 70f;
        moveToLocal(targetPosition, trueTime, new Vector3(), Finish());
    }

    IEnumerator Finish()
    {
        Animator animator = this.GetComponent<Animator>();
        animator.Play("End");
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void moveToLocal(Vector3 destination, float duration, Vector3 manipulate = new Vector3(), IEnumerator nextAction = null)
    {
        StopAllCoroutines();
        StartCoroutine(tweenToLocal(destination, duration, manipulate, nextAction));
    }

    IEnumerator tweenToLocal(Vector3 destination, float speed, Vector3 manipulate = new Vector3(), IEnumerator nextAction = null)
    {
        float timeThrough = 0.0f;
        Vector3 initialPosition = transform.localPosition;
        while (Vector3.Distance(transform.localPosition, destination) >= 0.1f)
        {
            timeThrough += Time.deltaTime;
            Vector3 target = Vector3.Lerp(initialPosition, destination, timeThrough);
            transform.localPosition = target;// +manip;
            yield return null;
        }
        transform.localPosition = destination;
        if (nextAction != null)
            StartCoroutine(nextAction);
    }
}