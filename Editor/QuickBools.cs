using UnityEngine;
using UnityEditor;
using System.IO;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.ScriptableObjects;
using DogeHelper;


public class QuickBools : DogeHelpers
    {
        public static EditorCurveBinding MakeBinding(Transform Parent, GameObject toggleObject)
    {
        string transformPath = AnimationUtility.CalculateTransformPath(toggleObject.transform, Parent);
        transformPath.Replace(Parent.name + "/", "");
        EditorCurveBinding binding = new EditorCurveBinding
        {
            path = transformPath,
            propertyName = "m_IsActive",
            type = typeof(GameObject)
        };
        return binding;
    }
    public static AnimationCurve MakeCurve(GameObject toggleObject)
    {
        var curve = new AnimationCurve
        {
            keys = new Keyframe[2],
            postWrapMode = WrapMode.Loop,
            preWrapMode = WrapMode.Loop

        };
        Keyframe tempKey = curve.keys[0];
        Keyframe tempKey2 = curve.keys[1];
        float valueFloat = 1F;
        if (toggleObject.activeSelf)
        {
            valueFloat = 0F;
        }

        tempKey.time = 0;
        tempKey.value = valueFloat;
        tempKey2.time = 0.016666668F;
        tempKey2.value = valueFloat;

        curve.keys[0] = tempKey;
        curve.keys[1] = tempKey2;
        var ks = new Keyframe[2];
        ks[0] = tempKey;
        ks[1] = tempKey2;
        curve.keys = ks;
        return curve;
    }
        public static AnimationClip MakeAnimation(string animationName, GameObject[] toggleObjects, Transform Parent)
        {
            int i = 0;
            string[] tempHolder = AssetDatabase.FindAssets("Doge Empty Animation");
            string emptyAssetPath = AssetDatabase.GUIDToAssetPath(tempHolder[0]);
            AnimationClip tempClip = new AnimationClip();
            tempClip = Instantiate((AnimationClip)AssetDatabase.LoadAssetAtPath(emptyAssetPath, typeof(AnimationClip)));
            string pathToCurrentFolder = GetCurrentFolder();
            string currentFolder = Path.Combine(Directory.GetCurrentDirectory(), pathToCurrentFolder);
            currentFolder = "Assets" + currentFolder.Substring(Application.dataPath.Length) + "/" + animationName + ".anim";
            while (toggleObjects[i] != null)
            {
                string transformPath = AnimationUtility.CalculateTransformPath(toggleObjects[i].transform, Parent);
                transformPath.Replace(Parent.name + "/", "");
                EditorCurveBinding binding = new EditorCurveBinding
                {
                    path = transformPath,
                    propertyName = "m_IsActive",
                    type = typeof(GameObject)
                };
                //binding = EditorCurveBinding.DiscreteCurve(binding.path, binding.type, binding.propertyName);
                var curve = new AnimationCurve
                {
                    keys = new Keyframe[2],
                    postWrapMode = WrapMode.Loop,
                    preWrapMode = WrapMode.Loop

                };
                Keyframe tempKey = curve.keys[0];
                Keyframe tempKey2 = curve.keys[1];
                float valueFloat = 1F;
                if (toggleObjects[i].activeSelf)
                {
                    valueFloat = 0F;
                }

                tempKey.time = 0;
                tempKey.value = valueFloat;
                tempKey2.time = 0.016666668F;
                tempKey2.value = valueFloat;

                curve.keys[0] = tempKey;
                curve.keys[1] = tempKey2;
                var ks = new Keyframe[2];
                ks[0] = tempKey;
                ks[1] = tempKey2;
                curve.keys = ks;

                AnimationUtility.SetEditorCurve(tempClip, binding, curve);
                i++;
            }

            AssetDatabase.CreateAsset(tempClip, currentFolder);
            return tempClip;
        }

        public static VRCExpressionsMenu AddMenu(VRCExpressionsMenu avatarExpressionMenu, string qbParameterName)
        {
            avatarExpressionMenu.controls.Add(new VRCExpressionsMenu.Control());
            int newExpressionSlot = avatarExpressionMenu.controls.Count - 1;
            avatarExpressionMenu.controls[newExpressionSlot].type = VRCExpressionsMenu.Control.ControlType.Toggle;
            avatarExpressionMenu.controls[newExpressionSlot].name = qbParameterName;
            avatarExpressionMenu.controls[newExpressionSlot].parameter = new VRCExpressionsMenu.Control.Parameter();
            avatarExpressionMenu.controls[newExpressionSlot].parameter.name = qbParameterName;

            return avatarExpressionMenu;
        }
        public static VRCExpressionsMenu AddMenu(VRCExpressionsMenu avatarExpressionMenu, string qbParameterName, int parameterValue)
        {
            avatarExpressionMenu.controls.Add(new VRCExpressionsMenu.Control());
            int newExpressionSlot = avatarExpressionMenu.controls.Count - 1;
            avatarExpressionMenu.controls[newExpressionSlot].type = VRCExpressionsMenu.Control.ControlType.Toggle;
            avatarExpressionMenu.controls[newExpressionSlot].name = qbParameterName;
            avatarExpressionMenu.controls[newExpressionSlot].parameter = new VRCExpressionsMenu.Control.Parameter();
            avatarExpressionMenu.controls[newExpressionSlot].parameter.name = qbParameterName;
            avatarExpressionMenu.controls[newExpressionSlot].value = parameterValue;

            return avatarExpressionMenu;
        }
    public static VRCExpressionsMenu AddMenu(VRCExpressionsMenu avatarExpressionMenu, string toggleName, int parameterValue,string parameterName)
    {
        avatarExpressionMenu.controls.Add(new VRCExpressionsMenu.Control());
        int newExpressionSlot = avatarExpressionMenu.controls.Count - 1;
        avatarExpressionMenu.controls[newExpressionSlot].type = VRCExpressionsMenu.Control.ControlType.Toggle;
        avatarExpressionMenu.controls[newExpressionSlot].name = toggleName;
        avatarExpressionMenu.controls[newExpressionSlot].parameter = new VRCExpressionsMenu.Control.Parameter();
        avatarExpressionMenu.controls[newExpressionSlot].parameter.name = parameterName;
        avatarExpressionMenu.controls[newExpressionSlot].value = parameterValue;

        return avatarExpressionMenu;
    }
    public static VRCExpressionParameters AddParameter(VRCExpressionParameters avatarParameterMenu, string qbParameterName, VRCExpressionParameters.ValueType myType)
        {
            //Add parameter to expression parameteres
            int newParameterCount = avatarParameterMenu.parameters.Length + 1;
            VRCExpressionParameters tempAvatarParameterMenu = CreateInstance("VRCExpressionParameters") as VRCExpressionParameters;
            tempAvatarParameterMenu.parameters = new VRCExpressionParameters.Parameter[newParameterCount];
            int count = 0;
            foreach (var temp in avatarParameterMenu.parameters)
            {
                tempAvatarParameterMenu.parameters[count] = temp;
                count++;
            }
            tempAvatarParameterMenu.parameters[count] = new VRCExpressionParameters.Parameter();
            tempAvatarParameterMenu.parameters[count].name = qbParameterName;
            tempAvatarParameterMenu.parameters[count].valueType = myType;
            tempAvatarParameterMenu.parameters[count].saved = true;
            tempAvatarParameterMenu.parameters[count].defaultValue = 0;
            avatarParameterMenu.parameters = tempAvatarParameterMenu.parameters;
            return avatarParameterMenu;
        }
    public static VRCExpressionParameters AddParameter(VRCExpressionParameters avatarParameterMenu, string qbParameterName, VRCExpressionParameters.ValueType myType, int defaultValue)
    {
        //Add parameter to expression parameteres
        int newParameterCount = avatarParameterMenu.parameters.Length + 1;
        VRCExpressionParameters tempAvatarParameterMenu = CreateInstance("VRCExpressionParameters") as VRCExpressionParameters;
        tempAvatarParameterMenu.parameters = new VRCExpressionParameters.Parameter[newParameterCount];
        int count = 0;
        foreach (var temp in avatarParameterMenu.parameters)
        {
            tempAvatarParameterMenu.parameters[count] = temp;
            count++;
        }
        tempAvatarParameterMenu.parameters[count] = new VRCExpressionParameters.Parameter();
        tempAvatarParameterMenu.parameters[count].name = qbParameterName;
        tempAvatarParameterMenu.parameters[count].valueType = myType;
        tempAvatarParameterMenu.parameters[count].saved = true;
        tempAvatarParameterMenu.parameters[count].defaultValue = defaultValue;
        avatarParameterMenu.parameters = tempAvatarParameterMenu.parameters;
        return avatarParameterMenu;
    }

    public static AnimatorController AddAnimatorLayer(AnimatorController myController, string myParameterName, bool Bool)
    {
        myController.AddParameter(myParameterName, AnimatorControllerParameterType.Bool);
        int layerCount = myController.layers.Length;
        myController.AddLayer(myParameterName);
        AnimatorControllerLayer[] tempLayers = myController.layers;
        tempLayers[layerCount].defaultWeight = 1;
        myController.layers = tempLayers;
        return myController;
    }
    public static AnimatorController AddAnimatorLayer(AnimatorController myController, string myParameterName, int Int)
    {
        myController.AddParameter(myParameterName, AnimatorControllerParameterType.Int);
        int layerCount = myController.layers.Length;
        myController.AddLayer(myParameterName);
        AnimatorControllerLayer[] tempLayers = myController.layers;
        tempLayers[layerCount].defaultWeight = 1;
        myController.layers = tempLayers;
        return myController;
    }
    public static AnimatorController QuickBool(AnimatorController fxLayer, string qbParameterName, AnimationClip qbAnimationClip)
        {
        //Add parameter and create layer
        int layerCount = fxLayer.layers.Length;
        fxLayer = AddAnimatorLayer(fxLayer, qbParameterName,true);
          


            //Create states and transistions
            string[] tempHolder = AssetDatabase.FindAssets("Default DT State");
            string emptyAssetPath = AssetDatabase.GUIDToAssetPath(tempHolder[0]);
            AnimationClip tempClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(emptyAssetPath, typeof(AnimationClip));
            AnimatorState startingState = fxLayer.AddMotion(tempClip, layerCount);
            AnimatorState endingState = fxLayer.AddMotion(qbAnimationClip, layerCount);
            AnimatorStateTransition toEndTransition = startingState.AddTransition(endingState, false);
            toEndTransition.duration = 0;
            AnimatorStateTransition toStartTransition = endingState.AddTransition(startingState, false);
            toStartTransition.duration = 0;
            toStartTransition.AddCondition(AnimatorConditionMode.IfNot, 0, qbParameterName);
            toEndTransition.AddCondition(AnimatorConditionMode.If, 1, qbParameterName);


            //Position and add layer control to states
            ChildAnimatorState[] states = fxLayer.layers[layerCount].stateMachine.states;
            states[0].position = new Vector3(300, 100, 0);
            states[1].position = new Vector3(600, 100, 0);
                VRCAnimatorLayerControl entryBehavior = states[0].state.AddStateMachineBehaviour<VRCAnimatorLayerControl>();
                entryBehavior.layer = layerCount;
                entryBehavior.goalWeight = 0;
                entryBehavior.blendDuration = 0;
                entryBehavior.playable = VRCAnimatorLayerControl.BlendableLayer.FX;
            VRCAnimatorLayerControl endBehavior = states[1].state.AddStateMachineBehaviour<VRCAnimatorLayerControl>();
            endBehavior.layer = layerCount;
            endBehavior.goalWeight = 1;
            endBehavior.blendDuration = 0;
            endBehavior.playable = VRCAnimatorLayerControl.BlendableLayer.FX;
            fxLayer.layers[layerCount].stateMachine.states = states;
            return fxLayer;
        }
    }


