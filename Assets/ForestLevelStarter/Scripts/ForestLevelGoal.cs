using UnityEngine;

public class ForestLevelGoal : MonoBehaviour
{
    private bool completed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject actor = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;
        if (completed || !actor.CompareTag("Player")) return;
        completed = true;
        Debug.Log("УРОВЕНЬ 1 ПРОЙДЕН! Лис достиг финишной двери.");
        // Later, you can show a victory panel or load Level 02 here.
    }
}
