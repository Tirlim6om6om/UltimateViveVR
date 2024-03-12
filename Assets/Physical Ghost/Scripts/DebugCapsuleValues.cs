using UnityEngine;

[System.Serializable]
public struct DebugCapsuleValues
{
    public Vector3 center;
    public float radius;
    public float height;
    public CapsuleDirection direction;

    public DebugCapsuleValues(Vector3 center, float radius, float height, CapsuleDirection direction)
    {
        this.center = center;
        this.radius = radius;
        this.height = height;
        this.direction = direction;
    }
}