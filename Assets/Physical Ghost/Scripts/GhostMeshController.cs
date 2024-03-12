using System;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class GhostMeshController : MonoBehaviour
{
    [SerializeField] private Transform parentTf;

    [Range(0, 2)]
    [SerializeField] private float minDistance = 0.1f;

    [Range(0, 2)]
    [SerializeField] private float maxDistance = 1.8f;

    private Transform _tf;
    private MeshRenderer _meshRender;
    private MeshRenderer MeshRender => _meshRender ??= GetComponent<MeshRenderer>(); //<-- Потому что создание обрабатывается в Awake, и там же вызывается SetMaterial, а если определять _meshRender в Start(), то он не будет готов, и в итоге NPE
    private MaterialPropertyBlock _mpb;
    private float _sqrMin;
    private float _sqrMax;
    private static readonly int Strength = Shader.PropertyToID("_Strength");


    [SerializeField] private bool displayAlways;
   

    void Start()
    {
        _mpb = new MaterialPropertyBlock();

        float strValue = 1;
        if (!parentTf || displayAlways)
        {
            enabled = false;
        }
        else
        {
            _tf = transform;
            strValue = 0;
            _sqrMin = minDistance * minDistance;
            _sqrMax = maxDistance * maxDistance;
        }
        
        _mpb.SetFloat(Strength,strValue);
        MeshRender.SetPropertyBlock(_mpb);
       
    }

    void FixedUpdate()
    {
        if (!displayAlways && parentTf)
        {
            float dist = (_tf.position - parentTf.position).sqrMagnitude;
            float value = Mathf.Clamp01((dist - _sqrMin) / _sqrMax);
            _mpb.SetFloat(Strength, value);
            MeshRender.SetPropertyBlock(_mpb);
        }
    }

    public void SetParent(Transform parentTf1) { parentTf = parentTf1; }

    public void SetMaterial(Material mat) { MeshRender.material = mat; }
}