using UnityEngine;
using System;
public class Bounding : DogeTools
{
    public static void NonRecBounding(ref Transform Parent, Transform Anchor)
    {
        Vector3 tempCenter;
        Vector3 tempExtent;
        Vector3 greatestSize = new Vector3(0, 0, 0);


        foreach (Transform Child in Parent.transform)
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
        }

        foreach (Transform Child in Parent.transform)
        {
            if (Child.gameObject.GetComponent<SkinnedMeshRenderer>() != null) //second pass to set all to greatest size in x y z
            {
                Child.gameObject.GetComponent<SkinnedMeshRenderer>().localBounds = new Bounds(new Vector3(0, 0, 0), greatestSize);
                Debug.Log(Child.name);
            }
        }
        
        foreach (Transform Child in Parent.transform)
         {
           if (Child.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
              {
                  Child.gameObject.GetComponent<SkinnedMeshRenderer>().probeAnchor = Anchor;
              }
         }

        
    }
    public static Transform SetAnchor(ref Transform Parent)
    {
        Transform Anchor = Parent;
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
        return Anchor;
    }
    public static void RecurisveBounding(ref Transform myParent, Transform Anchor)
    {
        Vector3 greatestSize = new Vector3(0, 0, 0);
        RecurisveBiggestBox(myParent, ref greatestSize);
        RecursiveSetMeshes(myParent, greatestSize, Anchor);
    }   
    static void RecurisveBiggestBox(Transform myParent, ref Vector3 greatestSize)
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
    static void RecursiveSetMeshes(Transform myParent, Vector3 greatestSize, Transform Anchor)
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
            RecursiveSetMeshes(Child, greatestSize, Anchor);
        }
    }
}
