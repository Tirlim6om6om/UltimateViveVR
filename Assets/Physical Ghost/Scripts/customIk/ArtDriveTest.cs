using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtDriveTest : MonoBehaviour
{
    [SerializeField] private List<float> values= new (new float[6]);
    private ArticulationBody _artBody;

    void Start()
    {
        _artBody = GetComponent<ArticulationBody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        _artBody.SetDriveTargets(values.Select(v=>v*Mathf.Deg2Rad).ToList());
    }
}
