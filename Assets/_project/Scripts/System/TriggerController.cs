using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TriggerController : MonoBehaviour
{
    public string targetTag = "Player";
    public float delay = 0f;
    public UnityEvent onTrigger;
    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(targetTag)) return;

        triggered = true;
        StartCoroutine(InvokeDelayed());
    }

    IEnumerator InvokeDelayed()
    {
        yield return new WaitForSeconds(delay);
        onTrigger.Invoke();
    }
}
