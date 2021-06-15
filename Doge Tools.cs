using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;
public class DogeTools : EditorWindow
{
    AnimationClip my_variable;
    string tmp;
    DefaultAsset my_file;
    GameObject Parent;
    Transform Anchor;

  

    [MenuItem("Tools/Doge Tools")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DogeTools));
    }
    private void OnGUI()
    {
        GUILayout.Label("Append to animation herichy", EditorStyles.boldLabel);
        tmp = EditorGUILayout.TextField("Hierchy string to append", tmp);
        my_variable = EditorGUILayout.ObjectField("Animation clip", my_variable, typeof(AnimationClip), false) as AnimationClip;

        if (GUILayout.Button("Append"))
        {
            Reheirchy();
        }
        GUILayout.Label("Copy all from folder to current open in project", EditorStyles.boldLabel);
        my_file = (DefaultAsset)EditorGUILayout.ObjectField("Folder", my_file, typeof(DefaultAsset), false);
        if (my_file != null) { 
        if (my_file.name == "Assets")
        {
            EditorGUILayout.HelpBox("Bruh just don't", MessageType.Warning, true);
        }
    }
        if (GUILayout.Button("Copy"))
        {
            Copy_move();
        }
        GUILayout.Label("Normalize and set anchors", EditorStyles.boldLabel);
        Anchor = EditorGUILayout.ObjectField("Anchor", Anchor, typeof(Transform), true) as Transform;
        Parent = EditorGUILayout.ObjectField("Avatar", Parent, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Fix skinned meshes"))
        {
            bounding();
        }
    }
    void Copy(string sourceDir, string targetDir, string folder_name)
    {
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, folder_name + Path.GetFileName(file)));

        foreach (var directory in Directory.GetDirectories(sourceDir))
            Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)), folder_name); //this is recursive but i dont know what ends it
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



                Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds = new Bounds(new Vector3(0, 0, 0), new Vector3 (tempXextent, tempYextent, tempZextent));
                Debug.Log(Child.name);
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
        Transform Hips = Parent.transform; //Set anchor override to hips this assumes armature is first child 
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

        else foreach (Transform Child in Parent.transform)
        {
           
            if (Child.name == "Armature")
            {
                foreach (Transform subChild in Child.transform)
                {
                    if (subChild.name == "Hips")
                    {
                        Hips = subChild;
                        break;
                    }
                        
                }
                
            }
            if (Child.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                Child.gameObject.GetComponent<SkinnedMeshRenderer>().probeAnchor = Hips;
            }
        }
    }
}
