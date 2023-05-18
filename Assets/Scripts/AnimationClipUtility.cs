using UnityEditor;
using UnityEngine;

namespace Moein.Animation
{
    public enum FrameRate
    {
        FR_4 = 4,
        FR_6 = 6,
        FR_8 = 8,
        FR_12 = 12,
        FR_24 = 24,
        FR_30 = 30
    }

    public static class AnimationClipUtility
    {
        /// <summary>
        /// Recreate an animation clip with new frameRate and other options
        /// </summary>
        public static AnimationClip ReClip(AnimationClip inClip,
            AnimationUtility.TangentMode rightTangent = AnimationUtility.TangentMode.Auto,
            AnimationUtility.TangentMode leftTangent = AnimationUtility.TangentMode.Auto,
            WeightedMode weightedMode = WeightedMode.Both,
            int frameRate = 24)
        {
            AnimationClip clip = new AnimationClip
            {
                legacy = inClip.legacy,
                events = inClip.events,
                frameRate = inClip.frameRate,
                wrapMode = inClip.wrapMode
            };

            var curves = GetAllCurves(inClip);

            clip.ClearCurves();

            for (int i = 0; i < curves.Length; i++)
            {
                curves[i].curve = ReCurve(curves[i].curve, rightTangent, leftTangent, weightedMode, frameRate);
                SetCurveData(clip, curves[i]);
            }

            return clip;
        }

        public static void SetCurveData(AnimationClip clip, AnimationClipCurveData data)
        {
            clip.SetCurve(data.path, data.type, data.propertyName, data.curve);
        }

        /// <summary>
        /// Recreate an animation curve with new frameRate and other options
        /// </summary>
        public static AnimationCurve ReCurve(AnimationCurve inCurve,
            AnimationUtility.TangentMode rightTangent = AnimationUtility.TangentMode.Auto,
            AnimationUtility.TangentMode leftTangent = AnimationUtility.TangentMode.Auto,
            WeightedMode weightedMode = WeightedMode.Both,
            int frameRate = 24, float valueOffset = 0)
        {
            float maxTime = inCurve.keys[inCurve.length - 1].time;
            float dt = 1f / frameRate;

            AnimationCurve result = new AnimationCurve();
            for (float t = 0; t <= maxTime; t += dt)
            {
                float v = inCurve.Evaluate(t) + valueOffset;
                result.AddKey(t, v);
            }

            // time fitting
            Keyframe[] keys = result.keys;

            float resultMaxTime = result.keys[result.length - 1].time;
            for (int i = 0; i < result.length; i++)
            {
                keys[i].time = keys[i].time.Remap(0, resultMaxTime, 0, maxTime);
            }

            result.keys = keys;

            for (int i = 0; i < result.keys.Length; i++)
            {
                result.keys[i].weightedMode = weightedMode;
                AnimationUtility.SetKeyLeftTangentMode(result, i, leftTangent);
                AnimationUtility.SetKeyRightTangentMode(result, i, rightTangent);
            }

            result.postWrapMode = inCurve.postWrapMode;
            result.preWrapMode = inCurve.preWrapMode;
            return result;
        }

        public static AnimationClipCurveData[] GetAllCurves(AnimationClip clip)
        {
            EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
            AnimationClipCurveData[] data = new AnimationClipCurveData[curveBindings.Length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new AnimationClipCurveData(curveBindings[i])
                {
                    curve = AnimationUtility.GetEditorCurve(clip, curveBindings[i])
                };
            }

            return data;
        }

        public static float Remap(this float value, float firstMin, float firstMax,
            float secondMin, float secondMax)
        {
            float t = Mathf.InverseLerp(firstMin, firstMax, value);
            return Mathf.Lerp(secondMin, secondMax, t);
        }
    }
}