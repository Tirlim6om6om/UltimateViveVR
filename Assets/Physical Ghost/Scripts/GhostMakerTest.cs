using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(Rigidbody))]
public class GhostMakerTest : MonoBehaviour
{
    [SerializeField] private Transform customAnchor;

    [Space(20)]
    [Header("Debug")]
    [SerializeField] private GameObject ghostPrefab;

    [SerializeField] private DebugCapsuleValues capsuleValues = new(Vector3.zero, 0.5f, 2, CapsuleDirection.YAxis);

    [Header("Materials")]
    [SerializeField] private Material solidGhostMat;

    [SerializeField] private Material distanceGhostMat;
    [SerializeField] private bool considerDistanceFromPlayer;


    private void Start()
    {
        GameObject parent = gameObject;

        GameObject ghostObject = Instantiate(ghostPrefab, parent.transform.position, parent.transform.rotation,
            customAnchor);

        ghostObject.GetComponent<MeshFilter>().mesh = parent.GetComponent<MeshFilter>().mesh;

        GhostMeshController meshController = ghostObject.GetComponent<GhostMeshController>();
        meshController.SetParent(parent.transform);
        meshController.SetMaterial(considerDistanceFromPlayer ? distanceGhostMat : solidGhostMat);

        Rigidbody rBody = ghostObject.GetComponent<Rigidbody>();
        rBody.Sleep();

        Joint joint = ghostObject.GetComponent<Joint>();
        joint.connectedBody = parent.GetComponent<Rigidbody>();

        Vector3 posDiff = customAnchor.position - ghostObject.transform.position;
        Vector3 anchorPos = posDiff.magnitude * Vector3.down;
        joint.anchor = anchorPos;
        joint.connectedAnchor = anchorPos;

        SetUpCapsule(ghostObject);

        ghostObject.SetActive(true);
        rBody.WakeUp();
    }

    private void SetUpCapsule(GameObject ghostObject)
    {
        CapsuleCollider caspColdr = ghostObject.GetComponent<CapsuleCollider>();

        caspColdr.center = capsuleValues.center;
        caspColdr.radius = capsuleValues.radius;
        caspColdr.height = capsuleValues.height;
        caspColdr.direction = (int) capsuleValues.direction;
        Debug.Log($"Capsule Direction : {caspColdr.direction}");
    }
}