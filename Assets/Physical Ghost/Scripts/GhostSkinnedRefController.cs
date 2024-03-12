using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Physical_Ghost.Scripts
{
    [System.Serializable]
    public struct GhostVertexReference
    {
        public Transform targetLimb;
        public Transform ghostLimb;
        public Color refColor;
    }

    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class GhostSkinnedRefController : MonoBehaviour
    {
        [SerializeField] private GhostVertexReference[] targetRefs;

        [Range(0, 2)]
        [SerializeField] private float minDistance = 0.1f;

        [Range(0.0001f, 2)]
        [SerializeField] private float maxDistance = 1.8f;

        [Header("Debug")]
        [SerializeField] private bool displayAlways;

        private SkinnedMeshRenderer _skin;
        private SkinnedMeshRenderer Skin => _skin ??= GetComponent<SkinnedMeshRenderer>();

        private Dictionary<GhostVertexReference, float> _refPower = new();


        private float _sqrMin;
        private float _sqrMax;


        private static readonly int Strength = Shader.PropertyToID("_Strength");
        private static readonly int RefVertexColor = Shader.PropertyToID("_RefColor");
        private static readonly int TargetHue = Shader.PropertyToID("_TargetHue");

        private MaterialPropertyBlock _mpb;
        private float _maxPower;
        private GhostVertexReference _maxPowerRef;
        private int _refCount;

        void Start()
        {
            _mpb = new MaterialPropertyBlock();

            float strValue = 1;
            if (displayAlways)
            {
                enabled = false;
            }
            else
            {
                strValue = 0;
                _sqrMin = minDistance * minDistance;
                _sqrMax = maxDistance * maxDistance;
            }

            _mpb.SetFloat(Strength, strValue);

            _refCount = targetRefs.Length;
        }

        private void FixedUpdate()
        {
            if (displayAlways) //todo убрать
                return;


            _maxPower = 0;

            for (int i = 0; i < _refCount; i++)
            {
                ApplyMaterialValues(targetRefs[i]);
            }

            _mpb.SetFloat(Strength, _maxPower);

            Color targetVertexColor = _maxPowerRef.refColor;
            if (_refCount > 1)
            {
                float saturation = 0;
                foreach (GhostVertexReference key in _refPower.Keys)
                {
                    if (!key.Equals(_maxPowerRef))
                    {
                        saturation += _refPower[key];
                      
                    }
                }

                saturation /= (_refCount - 1);


                Color.RGBToHSV(targetVertexColor, out float h, out float s, out float v);
                targetVertexColor = Color.HSVToRGB(h, 1 - saturation, 1);
               
            }

            _mpb.SetColor(RefVertexColor, targetVertexColor);
            Skin.SetPropertyBlock(_mpb);
        }

        private void ApplyMaterialValues(GhostVertexReference tgRef)
        {
            if (tgRef.targetLimb && tgRef.ghostLimb)
            {
                float dist = (tgRef.targetLimb.position - tgRef.ghostLimb.position).sqrMagnitude;
                float value = Mathf.Clamp01((dist - _sqrMin) / _sqrMax);


                _refPower[tgRef] = value;
                if (_maxPower < value)
                {
                    _maxPowerRef = tgRef;
                    _maxPower = value;
                }
            }
        }
    }
}