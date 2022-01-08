using UnityEngine;
using UnityEditor;
using System.IO;

using System.Reflection;
using System;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.ScriptableObjects;

//namespace QuickBool.QuickI

    public class QuickInts : QuickBools
    {
        public static void QuickInt(ref AnimatorController fxLayer, string qiParametername, GameObject[] intObjects, Transform Parent,VRCExpressionParameters avatarParameterMenu,VRCExpressionsMenu avatarExpressionMenu)
        {
        AnimationClip[] intClips = new AnimationClip[intObjects.Length+1];
        string pathToCurrentFolder = GetCurrentFolder();
        string currentFolder = Path.Combine(Directory.GetCurrentDirectory(), pathToCurrentFolder);
        currentFolder = "Assets" + currentFolder.Substring(Application.dataPath.Length);

        AssetDatabase.CreateFolder(currentFolder, qiParametername + " Animations");
        currentFolder = currentFolder + "/" + qiParametername + " Animations";
        string newFolder = currentFolder;

        int i = 0;
        while (intObjects[i] != null)
        {
            newFolder = currentFolder;
            string[] tempHolder = AssetDatabase.FindAssets("Doge Empty Animation");
            string emptyAssetPath = AssetDatabase.GUIDToAssetPath(tempHolder[0]);
            AnimationClip tempClip = new AnimationClip();
            tempClip = Instantiate((AnimationClip)AssetDatabase.LoadAssetAtPath(emptyAssetPath, typeof(AnimationClip)));

            newFolder += "/" + intObjects[i].name + ".anim";



            EditorCurveBinding binding = MakeBinding(Parent, intObjects[i]);
            var curve = MakeCurve(intObjects[i]);
            AnimationUtility.SetEditorCurve(tempClip, binding, curve);
            if (i > 0) //I should just grab this stuff from array of clips
            {
                binding = MakeBinding(Parent, intObjects[0]);
                curve = MakeCurve(intObjects[0]);
                AnimationUtility.SetEditorCurve(tempClip, binding, curve);
            }
            AssetDatabase.CreateAsset(tempClip, newFolder);
            intClips[i + 1] = tempClip;
            i++;

        }


        int layerCount = fxLayer.layers.Length;
        fxLayer = AddAnimatorLayer(fxLayer, qiParametername, 1);
        string[] tempHolder2 = AssetDatabase.FindAssets("Default DT State");
        string emptyAssetPath2 = AssetDatabase.GUIDToAssetPath(tempHolder2[0]);
        AnimationClip tempClip2 = (AnimationClip)AssetDatabase.LoadAssetAtPath(emptyAssetPath2, typeof(AnimationClip));
        intClips[0] = tempClip2;    //ill clean this up later
        AnimatorState[] intState = new AnimatorState[intClips.Length];
        AnimatorStateMachine intStateMachine = fxLayer.layers[layerCount].stateMachine;
        AnimatorStateTransition[] intTransitions = new AnimatorStateTransition[intClips.Length];
        int j = 0;
        while (intClips[j] != null)
        {
            intState[j] = fxLayer.AddMotion(intClips[j], layerCount);
            intTransitions[j] = intStateMachine.AddAnyStateTransition(intState[j]);
            intTransitions[j].duration = 0;
            if(j == 0)
            {
                intTransitions[j].AddCondition(AnimatorConditionMode.Equals, 1, qiParametername);
            }
            else if(j == 1)
            {
                intTransitions[j].AddCondition(AnimatorConditionMode.Equals, 0, qiParametername);

            }
            else
            {
                intTransitions[j].AddCondition(AnimatorConditionMode.Equals, j, qiParametername);

            }

            j++;
        }
        for(int k = 0; intState[k] != null; k++)
        {
            VRCAnimatorLayerControl Behavior = intState[k].AddStateMachineBehaviour<VRCAnimatorLayerControl>();
            Behavior.layer = layerCount;
            Behavior.goalWeight = Math.Min(k,1);
            Behavior.blendDuration = 0;
            Behavior.playable = VRCAnimatorLayerControl.BlendableLayer.FX;
        }
        AddParameter(avatarParameterMenu, qiParametername, VRCExpressionParameters.ValueType.Int, 1);
        for(int k = 1; intClips[k] != null;k++)
        {
            AddMenu(avatarExpressionMenu, intClips[k].name, k, qiParametername);
        }
    }
}
  

