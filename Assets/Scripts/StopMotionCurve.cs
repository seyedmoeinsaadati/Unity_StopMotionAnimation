using UnityEditor;
using UnityEngine;

namespace Moein.Animation
{
    [ExecuteInEditMode]
    public class StopMotionCurve : MonoBehaviour
    {
        public bool autoUpdate = false;
        [SerializeField] private FrameRate frameRate;
        [SerializeField] AnimationUtility.TangentMode tangentMode;
        [SerializeField] WeightedMode weightedMode;

        [SerializeField] private ParticleSystem.MinMaxCurve curve;

        [SerializeField] private float previewOffset = -1;

        private AnimationCurve inCurve => curve.curveMin;
        private AnimationCurve outCurve => curve.curveMax;

        private void Update()
        {
            if (autoUpdate) RefreshOutCurve();
        }

        private void RefreshOutCurve()
        {
            var curve = AnimationClipUtility.ReCurve(inCurve, tangentMode, tangentMode, weightedMode, (int) frameRate,
                previewOffset);
            outCurve.keys = curve.keys;
            outCurve.postWrapMode = curve.postWrapMode;
            outCurve.preWrapMode = curve.preWrapMode;
        }
    }
}