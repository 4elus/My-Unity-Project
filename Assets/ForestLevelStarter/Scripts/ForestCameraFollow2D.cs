using UnityEngine;

// Optional: attach to Main Camera when you do not already have a camera-follow script.
[RequireComponent(typeof(Camera))]
public class ForestCameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.18f;
    public float levelStartX = 0f;
    public float levelEndX = 121f;
    public float fixedY = 0f;

    private Vector3 smoothVelocity;
    private Camera cameraComponent;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        float halfWidth = cameraComponent.orthographicSize * cameraComponent.aspect;
        float left = levelStartX + halfWidth;
        float right = levelEndX - halfWidth;
        float x = left < right ? Mathf.Clamp(target.position.x, left, right)
                               : (levelStartX + levelEndX) * 0.5f;
        Vector3 desired = new Vector3(x, fixedY, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref smoothVelocity, smoothTime);
    }
}
