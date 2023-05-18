#if UNITY_EDITOR
using UnityEngine;
using System;
using UnityEditor;

namespace Moein.Animation
{
    [ExecuteInEditMode]
    public class StopMotionAnimationClip : MonoBehaviour
    {
        [SerializeField] private FrameRate frameRate;
        [SerializeField] AnimationUtility.TangentMode tangentMode;
        [SerializeField] WeightedMode weightedMode;

        [SerializeField] private AnimationClip[] clips;
        [SerializeField] private bool save;

        private void Update()
        {
            if (save)
            {
                if (clips.Length > 0)
                {
                    for (int i = 0; i < clips.Length; i++)
                    {
                        ReClip(clips[i]);
                    }
                }

                save = false;
            }
        }

        private void ReClip(AnimationClip clip)
        {
            var newClip = AnimationClipUtility.ReClip(clip, tangentMode, tangentMode, weightedMode, (int) frameRate);

            string path = AssetDatabase.GetAssetPath(clip);
            path = path.Replace($"{clip.name}.anim", "");
            SaveAnimationClip(path, $"{clip.name}_ST_{frameRate}", newClip);
        }

        public static void SaveAnimationClip(string path, string filename, AnimationClip clip)
        {
            path = path + filename + ".anim";
            try
            {
                AssetDatabase.CreateAsset(clip, path);
                AssetDatabase.SaveAssets();

                Debug.Log($"AnimationClip Saved Successfully. {path}");
            }
            catch (Exception)
            {
                Debug.LogError($"AnimationClip Saved Failed. {path}");
            }
        }
    }
}

#endif