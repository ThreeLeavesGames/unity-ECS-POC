using UnityEngine;

public class TargetPositionProvider : MonoBehaviour
{
    public GameObject targetObject; // Assign this in the Inspector

    public Vector3 GetTargetPosition()
    {
        return targetObject.transform.position;
    }
}
