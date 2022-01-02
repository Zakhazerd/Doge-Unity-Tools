using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.ScriptableObjects;
//using VRC.SDKBase;
//using VRC.SDK3.Editor;
//using VRC.SDKBase.Editor;
//using VRC.SDKBase.Editor.BuildPipeline;
//using VRC.SDKBase.Validation.Performance;
//using VRC.SDKBase.Validation;
//using VRC.SDKBase.Validation.Performance.Stats;
//using VRC.SDK3.Validation; //Saving for later just incase
public class DogeTools : EditorWindow
{
    AnimationClip my_variable;
    string tmp;
    DefaultAsset my_file;
    Transform Parent;
    Transform Anchor;
    bool searchAll = true;
    VRCAvatarDescriptor Avatar;
    AnimatorController fxLayer;
    string qbParameterName;
    AnimationClip qbAnimationClip;
    Vector2 scrollPos;
    VRCExpressionsMenu avatarExpressionMenu;
    VRCExpressionParameters avatarParameterMenu;
    GameObject[] toggleObjects = new GameObject[10];
    string animationName;



    [MenuItem("Tools/Doge Tools")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DogeTools));
    }
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label("Append to animation herichy", EditorStyles.boldLabel);
        tmp = EditorGUILayout.TextField("Hierchy string to append", tmp);
        my_variable = EditorGUILayout.ObjectField("Animation Clip", my_variable, typeof(AnimationClip), false) as AnimationClip;
        EditorGUI.BeginDisabledGroup(my_variable == null);
        if (GUILayout.Button("Append"))
        {
            Reheirchy();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Label("Copy folder contents to project folder", EditorStyles.boldLabel);
        my_file = (DefaultAsset)EditorGUILayout.ObjectField("Folder", my_file, typeof(DefaultAsset), false);
        EditorGUI.BeginDisabledGroup(my_file == null);
        if (GUILayout.Button("Copy"))
        {
            Copy_move();

        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Label("Normalize and set anchors", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        Parent = EditorGUILayout.ObjectField("Avatar", Parent, typeof(Transform), true) as Transform;
        if(Parent && !Parent.gameObject.GetComponent<VRCAvatarDescriptor>())
        {
            EditorGUILayout.HelpBox("Avatar missing VRC Avatar Descriptor", MessageType.Warning);
        }
        if (EditorGUI.EndChangeCheck() && Parent != null)
        {
            SetAnchor();
            Avatar = Parent.gameObject.GetComponent<VRCAvatarDescriptor>();
            avatarExpressionMenu = Avatar.expressionsMenu;
            avatarParameterMenu = Avatar.expressionParameters;
        }
        Anchor = EditorGUILayout.ObjectField("Anchor", Anchor, typeof(Transform), true) as Transform;
        searchAll = EditorGUILayout.Toggle("Search for all meshes", searchAll);
        EditorGUI.BeginDisabledGroup(Parent == null || Anchor == null);
        if (GUILayout.Button("Fix Meshes"))
        {
            if (searchAll)
            RecurisveBounding(Parent);
            else
            bounding();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(Parent == null);
        if (GUILayout.Button("Set Fx Layer Weights To 1"))
        {
            Avatar = Parent.gameObject.GetComponent<VRCAvatarDescriptor>();
            fxLayer = Avatar.baseAnimationLayers[4].animatorController as AnimatorController;
            SetLayerWeights();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Label("Quick Bool", EditorStyles.boldLabel);
        qbParameterName = EditorGUILayout.TextField("Parameter Name", qbParameterName);
        qbAnimationClip = EditorGUILayout.ObjectField("Animation Clip", qbAnimationClip, typeof(AnimationClip), false) as AnimationClip;
        avatarExpressionMenu = EditorGUILayout.ObjectField("Expresssion Menu", avatarExpressionMenu, typeof(VRCExpressionsMenu), false) as VRCExpressionsMenu;
        if (avatarExpressionMenu != null && avatarExpressionMenu.controls.Count == 8)
        {
            EditorGUILayout.HelpBox("Expression menu already has max of 8 controls", MessageType.Warning);
        }
        avatarParameterMenu = EditorGUILayout.ObjectField("Expresssion Parameters", avatarParameterMenu, typeof(VRCExpressionParameters), false) as VRCExpressionParameters;
        EditorGUI.BeginDisabledGroup(Parent == null || qbParameterName == null || qbAnimationClip == null);
        if (GUILayout.Button("Add To Fx Layer"))
        {
            Avatar = Parent.gameObject.GetComponent<VRCAvatarDescriptor>();
            fxLayer = Avatar.baseAnimationLayers[4].animatorController as AnimatorController;
            QuickBool();
            AddParameter();
            AddMenu();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Label("Quick Bool Animation", EditorStyles.boldLabel);
        animationName = EditorGUILayout.TextField("Animation Name", animationName);
        toggleObjects[0] = EditorGUILayout.ObjectField("Object " + "1",toggleObjects[0],typeof(GameObject),true) as GameObject;
        for(int i = 1; i < toggleObjects.Length;i++)
        {
            if (toggleObjects[i - 1] != null)
            {
                toggleObjects[i] = EditorGUILayout.ObjectField("Object " + (i+1), toggleObjects[i], typeof(GameObject), true) as GameObject;
            }
            else toggleObjects[i] = null;
        }
        EditorGUI.BeginDisabledGroup(Parent == null || toggleObjects[0] == null || animationName == null);
        if (GUILayout.Button("Make Animation"))
        {
            MakeAnimation();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndScrollView();

    }
    void Copy(string sourceDir, string targetDir, string folder_name)
    {
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, folder_name + Path.GetFileName(file)));

        foreach (var directory in Directory.GetDirectories(sourceDir))
            Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)), folder_name); 
    }
    private void Copy_move()
    {
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        string folder_name = Path.GetFileNameWithoutExtension(obj.ToString()) + " ";
        string pathToCurrentFolder = obj.ToString(); //I have litterally no how the above works
        string current_filePath = Path.Combine(Directory.GetCurrentDirectory(), pathToCurrentFolder);
        current_filePath = current_filePath.Replace("/", "\\");
        string selected_assetPath = AssetDatabase.GetAssetPath(my_file);
        string selected_filePath = Path.Combine(Directory.GetCurrentDirectory(), selected_assetPath);
        selected_filePath = selected_filePath.Replace("/", "\\");
        Copy(selected_filePath,current_filePath, folder_name);
        AssetDatabase.Refresh();
    }
    private void Reheirchy()
    {
        string assetPath = AssetDatabase.GetAssetPath(my_variable); // turn this into a function retard even tho i probably only need the realitive path but fuck you
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        filePath = filePath.Replace("/", "\\");
        string to_append = "    path: ";
        string line;
        int offset = 10;
        to_append = to_append + tmp;
        tmp = filePath;
        tmp = tmp.Remove(tmp.Length - 5, 5);
        tmp = tmp + " new.anim";            //clean this up its stupid
        string out_path = tmp;
        StreamReader infile = new StreamReader(filePath);
        StreamWriter outfile = new StreamWriter(out_path);
        line = infile.ReadLine();
        while (line != null)
        {
            if (line[4] == 'p' && line[5] == 'a')
            {
             
                line = line.Remove(0,offset);
                line = to_append + line;
                outfile.WriteLine(line);

            }

            else
            {
             
                outfile.WriteLine(line);


            }
            line = infile.ReadLine();
        }
        tmp = line;
        outfile.Close();
        infile.Close();
        AssetDatabase.Refresh();
    }
    void bounding()
    {
        float tempXcenter; // Lot of this is dumb and i should being using vectors or soemthing
        float tempYcenter;
        float tempZcenter;
        float tempXextent;
        float tempYextent;
        float tempZextent;
        float greatestX = 0;
        float greatestY = 0;
        float greatestZ = 0;

        foreach (Transform Child in Parent.transform)
        {
            if (Child.gameObject.GetComponent<SkinnedMeshRenderer>() != null) //first pass to center boxes and get the greatest size in x y z
            {
                tempXcenter = Math.Abs(Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.center.x);
                tempYcenter = Math.Abs(Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.center.y);
                tempZcenter = Math.Abs(Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.center.z);
                tempXextent = Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.extents.x;
                tempYextent = Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.extents.y;
                tempZextent = Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.extents.z;

                tempXextent = (tempXcenter + tempXextent) * 2;
                tempYextent = (tempYcenter + tempYextent) * 2;
                tempZextent = (tempZcenter + tempZextent) * 2;

                if (tempXextent > greatestX) greatestX = tempXextent;
                if (tempYextent > greatestY) greatestY = tempYextent;
                if (tempZextent > greatestZ) greatestZ = tempZextent;

            }
        }

        foreach (Transform Child in Parent.transform)
        {
            if (Child.gameObject.GetComponent<SkinnedMeshRenderer>() != null) //second pass to set all to greatest size in x y z
            {
                Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(greatestX, greatestY, greatestZ));
                Debug.Log(Child.name);
            }
        }
        if (Anchor != null)
        {
            foreach (Transform Child in Parent.transform)
            {
                if (Child.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    Child.gameObject.GetComponent<SkinnedMeshRenderer>().probeAnchor = Anchor;
                }
            }

        }
    }
    void SetAnchor()
    {
        foreach (Transform Child in Parent.transform)
        {
            if (Child.name.Contains("Armature"))
            {
                foreach (Transform subChild in Child.transform)
                {
                    if (subChild.name == "Hips")
                    {
                        Anchor = subChild;
                        break;
                    }

                }

            }
        }
    }
    void RecurisveBounding(Transform myParent)
    {
        Vector3 greatestSize = new Vector3(0,0,0);
        RecurisveBiggestBox(myParent, ref greatestSize);
        RecursiveSetMeshes(myParent, greatestSize);
    }
    void RecurisveBiggestBox(Transform myParent, ref Vector3 greatestSize)
    {
        Vector3 tempCenter;
        Vector3 tempExtent;
        foreach (Transform Child in myParent.transform)
        {
            if (Child.gameObject.GetComponent<SkinnedMeshRenderer>() != null) //first pass to center boxes and get the greatest size in x y z
            {
                tempCenter.x = Math.Abs(Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.center.x);
                tempCenter.y = Math.Abs(Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.center.y);
                tempCenter.z = Math.Abs(Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.center.z);
                tempExtent.x = Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.extents.x;
                tempExtent.y = Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.extents.y;
                tempExtent.z = Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds.extents.z;

                tempExtent = (tempExtent + tempCenter) * 2;

                if (tempExtent.x > greatestSize.x) greatestSize.x = tempExtent.x;
                if (tempExtent.y > greatestSize.y) greatestSize.y = tempExtent.y;
                if (tempExtent.z > greatestSize.z) greatestSize.z = tempExtent.z;
            }
            RecurisveBiggestBox(Child, ref greatestSize);
        }
    }
    void RecursiveSetMeshes(Transform myParent, Vector3 greatestSize)
    {
        foreach (Transform Child in myParent.transform)
        {
            if (Child.gameObject.GetComponent<SkinnedMeshRenderer>() != null) //second pass to set all to greatest size in x y z
            {
                Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds = new Bounds(new Vector3(0, 0, 0), greatestSize);
                Child.gameObject.GetComponent<SkinnedMeshRenderer>().probeAnchor = Anchor;
            }
            else if (Child.gameObject.GetComponent<MeshRenderer>() != null)
            {
                Child.gameObject.GetComponent<MeshRenderer>().probeAnchor = Anchor;
            }
            RecursiveSetMeshes(Child, greatestSize);
        }
    }
    void SetLayerWeights()
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
    }
    void QuickBool()
    {
        //Add parameter and create layer
        fxLayer.AddParameter(qbParameterName, AnimatorControllerParameterType.Bool);
        int layerCount = fxLayer.layers.Length;
        fxLayer.AddLayer(qbParameterName);
        AnimatorControllerLayer[] tempLayers = fxLayer.layers;
        tempLayers[layerCount].defaultWeight = 1;
        fxLayer.layers = tempLayers;
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
    }
    void AddParameter()
    {
        //Add parameter to expression parameteres
        int newParameterCount = avatarParameterMenu.parameters.Length + 1;
        var tempAvatarParameterMenu = new VRCExpressionParameters();
        tempAvatarParameterMenu.parameters = new VRCExpressionParameters.Parameter[newParameterCount];
        int count = 0;
        foreach (var temp in avatarParameterMenu.parameters)
        {
            tempAvatarParameterMenu.parameters[count] = temp;
            count++;
        }
        tempAvatarParameterMenu.parameters[count] = new VRCExpressionParameters.Parameter();
        tempAvatarParameterMenu.parameters[count].name = qbParameterName;
        tempAvatarParameterMenu.parameters[count].valueType = VRCExpressionParameters.ValueType.Bool;
        tempAvatarParameterMenu.parameters[count].saved = true;
        tempAvatarParameterMenu.parameters[count].defaultValue = 0;
        avatarParameterMenu.parameters = tempAvatarParameterMenu.parameters;
    }
    void AddMenu()
    {
        avatarExpressionMenu.controls.Add(new VRCExpressionsMenu.Control());
        int newExpressionSlot = avatarExpressionMenu.controls.Count - 1;
        avatarExpressionMenu.controls[newExpressionSlot].type = VRCExpressionsMenu.Control.ControlType.Toggle;
        avatarExpressionMenu.controls[newExpressionSlot].name = qbParameterName;
        avatarExpressionMenu.controls[newExpressionSlot].parameter = new VRCExpressionsMenu.Control.Parameter();
        avatarExpressionMenu.controls[newExpressionSlot].parameter.name = qbParameterName;
    }
    void MakeAnimation()
    {
        int i = 0;
        string[] tempHolder = AssetDatabase.FindAssets("Doge Empty Animation");
        string emptyAssetPath = AssetDatabase.GUIDToAssetPath(tempHolder[0]);
        AnimationClip tempClip = new AnimationClip();
        tempClip = Instantiate((AnimationClip)AssetDatabase.LoadAssetAtPath(emptyAssetPath, typeof(AnimationClip)));
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        string pathToCurrentFolder = obj.ToString();
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
        qbAnimationClip = tempClip;
    }
}
  
