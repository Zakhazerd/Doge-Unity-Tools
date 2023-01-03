using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.ScriptableObjects;
using DogeHelper;
using System.Linq;
using UnityEngine.Animations;

public class DogeTools : EditorWindow
{
    //AnimationClip my_variable;
    //string tmp;
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
    GameObject[] intObjects = new GameObject[8];
    string qiParameterName;
    GameObject[] constraintTargets = new GameObject[6];
    GameObject toConstrain;

    [MenuItem("Tools/Doge Tools")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DogeTools));
    }
    private void OnGUI()
    {
        GUILayout.Label("Doge Unity Tools", EditorStyles.boldLabel);
        GUILayout.Label("Version 1.52 \n",EditorStyles.miniLabel);
        EditorGUI.BeginChangeCheck();
        Parent = EditorGUILayout.ObjectField("Avatar", Parent, typeof(Transform), true) as Transform;
        if (EditorGUI.EndChangeCheck() && Parent != null)
        {
            Anchor = Bounding.SetAnchor(ref Parent);
            Avatar = Parent.gameObject.GetComponent<VRCAvatarDescriptor>();
            avatarExpressionMenu = Avatar.expressionsMenu;
            avatarParameterMenu = Avatar.expressionParameters;
        }
        if (Parent && !Parent.gameObject.GetComponent<VRCAvatarDescriptor>())
        {
            EditorGUILayout.HelpBox("Avatar missing VRC Avatar Descriptor", MessageType.Warning);
        }
        avatarExpressionMenu = EditorGUILayout.ObjectField("Expresssion Menu", avatarExpressionMenu, typeof(VRCExpressionsMenu), false) as VRCExpressionsMenu;
        avatarParameterMenu = EditorGUILayout.ObjectField("Expresssion Parameters", avatarParameterMenu, typeof(VRCExpressionParameters), false) as VRCExpressionParameters;
        DogeHelpers.DrawLine();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
       


        GUILayout.Label("Normalize and set anchors", EditorStyles.boldLabel);
        Anchor = EditorGUILayout.ObjectField("Anchor", Anchor, typeof(Transform), true) as Transform;
        searchAll = EditorGUILayout.Toggle("Search for all meshes", searchAll);
        EditorGUI.BeginDisabledGroup(Parent == null || Anchor == null);
        if (GUILayout.Button("Fix Meshes"))
        {
            if (searchAll)
                Bounding.RecurisveBounding(ref Parent, Anchor);
            else
                Bounding.NonRecBounding(ref Parent, Anchor);
        }
        EditorGUI.EndDisabledGroup();
     

        EditorGUI.BeginDisabledGroup(Parent == null);
        if (GUILayout.Button("Set Fx Layer Weights To 1"))
        {
           DogeHelpers.GetFxLayer(Parent, ref fxLayer);
           fxLayer = DogeHelpers.SetLayerWeights(fxLayer);
        }
        EditorGUI.EndDisabledGroup();
        
        EditorGUI.BeginDisabledGroup(Parent == null);

        if (GUILayout.Button("Fix Layer Continuity"))
        {
            DogeHelpers.GetFxLayer(Parent, ref fxLayer);
            fxLayer = DogeHelpers.FixLayerCont(fxLayer);

        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Label("Quick Bool", EditorStyles.boldLabel);
        qbParameterName = EditorGUILayout.TextField("Parameter Name", qbParameterName);
        qbAnimationClip = EditorGUILayout.ObjectField("Animation Clip", qbAnimationClip, typeof(AnimationClip), false) as AnimationClip;
     //   avatarExpressionMenu = EditorGUILayout.ObjectField("Expresssion Menu", avatarExpressionMenu, typeof(VRCExpressionsMenu), false) as VRCExpressionsMenu;
        if (avatarExpressionMenu != null && avatarExpressionMenu.controls.Count == 8)
        {
            EditorGUILayout.HelpBox("Expression menu already has max of 8 controls", MessageType.Warning);
        }
    //    avatarParameterMenu = EditorGUILayout.ObjectField("Expresssion Parameters", avatarParameterMenu, typeof(VRCExpressionParameters), false) as VRCExpressionParameters;
        EditorGUI.BeginDisabledGroup(Parent == null || qbParameterName == null || qbAnimationClip == null);
        if (GUILayout.Button("Add To Fx Layer"))
        {
            DogeHelpers.GetFxLayer(Parent, ref fxLayer);
            fxLayer = QuickBools.QuickBool(fxLayer, qbParameterName, qbAnimationClip);
            avatarParameterMenu = QuickBools.AddParameter(avatarParameterMenu, qbParameterName, VRCExpressionParameters.ValueType.Bool);
            avatarExpressionMenu =  QuickBools.AddMenu(avatarExpressionMenu, qbParameterName);
        }
        EditorGUI.EndDisabledGroup();


        GUILayout.Label("Quick Bool Animation", EditorStyles.boldLabel);
        animationName = EditorGUILayout.TextField("Animation Name", animationName);
        toggleObjects[0] = EditorGUILayout.ObjectField("Object 1",toggleObjects[0],typeof(GameObject),true) as GameObject;
        for(int i = 1; i < toggleObjects.Length;i++)
        {
            if (toggleObjects[i - 1] != null)
            {
                toggleObjects[i] = EditorGUILayout.ObjectField("Object " + (i+1), toggleObjects[i], typeof(GameObject), true) as GameObject;
            }
            else toggleObjects[i] = null;
        }
        EditorGUI.BeginDisabledGroup(Parent == null || toggleObjects[0] == null || animationName == null || animationName == "");
        if (GUILayout.Button("Make Animation"))
        {
            qbAnimationClip = QuickBools.MakeAnimation(animationName, toggleObjects, Parent);
            
        }
       
        EditorGUI.EndDisabledGroup();
        if (toggleObjects[0] != null || animationName != null && animationName != "")
        {
            if (Parent == null)
                EditorGUILayout.HelpBox("Avatar needs to be set", MessageType.Warning);
        }

        GUILayout.Label("Quick Int", EditorStyles.boldLabel);
        qiParameterName = EditorGUILayout.TextField("Parameter Name", qiParameterName);
        intObjects[0] = EditorGUILayout.ObjectField("Default Object", intObjects[0], typeof(GameObject), true) as GameObject;
        for (int i = 1; i < intObjects.Length; i++)
        {
            if (intObjects[i - 1] != null)
            {
                intObjects[i] = EditorGUILayout.ObjectField("Object " + (i + 1), intObjects[i], typeof(GameObject), true) as GameObject;
            }
            else intObjects[i] = null;
        }
        EditorGUI.BeginDisabledGroup(Parent == null || intObjects[0] == null || qiParameterName == null && qiParameterName != "");
        if (GUILayout.Button("Quick Int"))
        {
            DogeHelpers.GetFxLayer(Parent, ref fxLayer);
            QuickInts.QuickInt(ref fxLayer, qiParameterName, intObjects, Parent,avatarParameterMenu,avatarExpressionMenu);

        }
        EditorGUI.EndDisabledGroup();
        if (intObjects[0] != null || qiParameterName != null && qiParameterName != "")//It didnt like me doing an &&
        {
            if(Parent == null)
            EditorGUILayout.HelpBox("Avatar needs to be set", MessageType.Warning);
        }
        if(avatarExpressionMenu != null && intObjects.Count(s => s != null) + avatarExpressionMenu.controls.Count > 8)
        {
            EditorGUILayout.HelpBox("Expression Menu needs " + (intObjects.Count(s => s != null) + avatarExpressionMenu.controls.Count - 8) + " more control(s)", MessageType.Warning);
            

        }
        GUILayout.Label("Quick Constrain", EditorStyles.boldLabel);
        constraintTargets[0] = EditorGUILayout.ObjectField("Constraint Targets", constraintTargets[0], typeof(GameObject), true) as GameObject;
        for (int i = 1; i < constraintTargets.Length; i++)
        {
            if (constraintTargets[i - 1] != null)
            {
                constraintTargets[i] = EditorGUILayout.ObjectField(" ", constraintTargets[i], typeof(GameObject), true) as GameObject;
            }
            else constraintTargets[i] = null;
        }
        toConstrain = EditorGUILayout.ObjectField("Object To Constrain", toConstrain, typeof(GameObject), true) as GameObject;
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(constraintTargets[0] == null || toConstrain == null);
        if (GUILayout.Button("       Constrain      ")) //I hope this is temporary
        {
            if(toConstrain.GetComponent<ParentConstraint>() != null)
            QuickConstrain.AddConstraint(constraintTargets[0], toConstrain, false);
            else QuickConstrain.AddToConstraint(constraintTargets[0], toConstrain);

            for (int i = 1; constraintTargets[i] == null; i++)
            {
                QuickConstrain.AddToConstraint(constraintTargets[i], toConstrain);
                
            }
            toConstrain.GetComponent<ParentConstraint>().constraintActive = true;
            toConstrain.GetComponent<ParentConstraint>().locked = true;
        }
        if (GUILayout.Button("Constrain and Zero"))
        {
            if (toConstrain.GetComponent<ParentConstraint>() == null)
                QuickConstrain.AddConstraint(constraintTargets[0], toConstrain, true);
            else QuickConstrain.AddToConstraint(constraintTargets[0], toConstrain);
            for (int i = 1; constraintTargets[i] != null; i++)
            {
                QuickConstrain.AddToConstraint(constraintTargets[i], toConstrain);

            }
            toConstrain.GetComponent<ParentConstraint>().constraintActive = true;
            toConstrain.GetComponent<ParentConstraint>().locked = true;
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
        //GUILayout.Label("Append to animation herichy", EditorStyles.boldLabel); Depricated Function to be redone later
        //tmp = EditorGUILayout.TextField("Hierchy string to append", tmp);
        //my_variable = EditorGUILayout.ObjectField("Animation Clip", my_variable, typeof(AnimationClip), false) as AnimationClip;
        //EditorGUI.BeginDisabledGroup(my_variable == null);
        //if (GUILayout.Button("Append"))
        //{
        //    Reheirchy();
        //}
        //EditorGUI.EndDisabledGroup();


        GUILayout.Label("Copy folder contents to project folder", EditorStyles.boldLabel);
        my_file = (DefaultAsset)EditorGUILayout.ObjectField("Folder", my_file, typeof(DefaultAsset), false);
        EditorGUI.BeginDisabledGroup(my_file == null);
        if (GUILayout.Button("Copy"))
        {
            CopyMove();

        }
        EditorGUI.EndDisabledGroup();
        DogeHelpers.DrawLine();
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
    private void CopyMove()
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
    //private void Reheirchy()
    //{
    //    string assetPath = AssetDatabase.GetAssetPath(my_variable); // turn this into a function retard even tho i probably only need the realitive path but fuck you
    //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
    //    filePath = filePath.Replace("/", "\\");
    //    string to_append = "    path: ";
    //    string line;
    //    int offset = 10;
    //    to_append = to_append + tmp;
    //    tmp = filePath;
    //    tmp = tmp.Remove(tmp.Length - 5, 5);
    //    tmp = tmp + " new.anim";            //clean this up its stupid
    //    string out_path = tmp;
    //    StreamReader infile = new StreamReader(filePath);
    //    StreamWriter outfile = new StreamWriter(out_path);
    //    line = infile.ReadLine();
    //    while (line != null)
    //    {
    //        if (line[4] == 'p' && line[5] == 'a')
    //        {
             
    //            line = line.Remove(0,offset);
    //            line = to_append + line;
    //            outfile.WriteLine(line);

    //        }

    //        else
    //        {
             
    //            outfile.WriteLine(line);


    //        }
    //        line = infile.ReadLine();
    //    }
    //    tmp = line;
    //    outfile.Close();
    //    infile.Close();
    //    AssetDatabase.Refresh();
    //}

}
  
