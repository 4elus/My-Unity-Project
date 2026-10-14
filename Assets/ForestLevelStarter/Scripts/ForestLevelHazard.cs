using UnityEngine;

// A beginner-friendly checkpoint reset. Integrate your HP / damage system later if needed.
public class ForestLevelHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject actor = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;
        if (!actor.CompareTag("Player")) return;

        GameObject spawn = GameObject.Find("PlayerSpawn");
        if (spawn == null) { Debug.LogWarning("Не найден PlayerSpawn"); return; }

        Rigidbody2D body = actor.GetComponent<Rigidbody2D>();
        if (body != null) body.linearVelocity = Vector2.zero; // Unity 2022/2023; also works in Unity 6 with obsolete warning
        actor.transform.position = spawn.transform.position;
    }
}
