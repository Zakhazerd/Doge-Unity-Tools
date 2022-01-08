using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;
using System.Reflection;
using System;
namespace DogeHelper
{
    public class DogeHelpers : DogeTools
    {
        public static AnimatorController SetLayerWeights(AnimatorController fxLayer)
        {
            int layerCount = 0;
            AnimatorControllerLayer[] layers = fxLayer.layers;
            foreach (AnimatorControllerLayer temp in fxLayer.layers)
            {
                if (!fxLayer.layers[layerCount].name.Contains("Hai"))
                {
                    layers[layerCount].defaultWeight = 1;
                    layerCount++;
                }
                else layerCount++;
            }
            fxLayer.layers = layers;
            return fxLayer;
        }
        public static void GetFxLayer(Transform Parent, ref AnimatorController fxLayer)
        {
            fxLayer = Parent.gameObject.GetComponent<VRCAvatarDescriptor>().baseAnimationLayers[4].animatorController as AnimatorController;
        }
        public static void DrawLine()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 2);
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1));
        }
        public static string GetCurrentFolder()
        {
            
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            return obj.ToString();
        }
    }

    
}