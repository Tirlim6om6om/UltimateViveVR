using UnityEngine;

namespace Physical_Ghost.Scripts.customIk
{
    [System.Serializable]
    public struct IkWeights
    {
        [Range(0,1)]
        public float weight;
        public Transform ikRef;
        public IkWeights(Transform ikRef ,float w = 0.5f)
        {
            weight = w;
            this.ikRef = ikRef;
        }
    }
}