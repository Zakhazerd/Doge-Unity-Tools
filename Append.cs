using UnityEngine;
using UnityEditor;
using System.IO;
/*using System.Collections;
using System;
using System.Text;
*/

public class Append : EditorWindow
{
    AnimationClip my_variable;
    string tmp;
  

    [MenuItem("Tools/Doge Tools")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Append));
    }
    private void OnGUI()
    {
        GUILayout.Label("Append time :)", EditorStyles.boldLabel);
        tmp = EditorGUILayout.TextField("New Hierchy", tmp);
        my_variable = EditorGUILayout.ObjectField("Animation",my_variable, typeof(AnimationClip), false) as AnimationClip;

        if (GUILayout.Button("Do the thing"))
        {
            Reheirchy();
        }
    }
    private void Reheirchy()
    {
        string assetPath = AssetDatabase.GetAssetPath(my_variable);
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        filePath = filePath.Replace("/", "\\");
        string to_append = "    path: ";
        string line;
        int offset = 10;
        to_append = to_append + tmp;
        tmp = filePath;
        tmp = tmp.Remove(tmp.Length - 5, 5);
        tmp = tmp + " new.anim";
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
}
