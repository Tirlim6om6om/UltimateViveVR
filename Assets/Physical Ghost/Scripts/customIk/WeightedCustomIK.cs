using UnityEngine;

namespace Physical_Ghost.Scripts.customIk
{
    [ExecuteInEditMode]
    public class WeightedCustomIK : CustomIK
    {
        [Header("Weights")]
        [SerializeField] private IkWeights[] weights;

        protected override void UpdateSequence()
        {
            for (int i = 0; i < weights.Length; i++)
            {
                IkWeights wh = weights[i];
                targetIK.position = Vector3.Lerp(targetIK.position, wh.ikRef.position, wh.weight);
            }

            base.UpdateSequence();
        }
    }
}