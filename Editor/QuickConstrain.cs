using UnityEngine;
using DogeHelper;
using UnityEditor;
using UnityEngine.Animations;

public class QuickConstrain : DogeHelpers
{
   public static void AddConstraint(GameObject constraintTarget, GameObject toConstrain, bool constrainAtLocation)
    {
        toConstrain.AddComponent(typeof(ParentConstraint));
        GameObject newObject = new GameObject(string.Format("{0} {1} Constraint", toConstrain.name, constraintTarget.name));
        newObject.transform.SetParent(constraintTarget.transform);
        newObject.transform.localPosition = new Vector3(0, 0, 0);
        ConstraintSource tempSource = new ConstraintSource
        {
            sourceTransform = newObject.transform,
            weight = 1F,
        };


        toConstrain.GetComponent<ParentConstraint>().AddSource((tempSource));
        if (!constrainAtLocation)
        {
            toConstrain.GetComponent<ParentConstraint>().SetTranslationOffset(0, newObject.transform.InverseTransformPoint(toConstrain.transform.position));
            Quaternion rotationOffset = Quaternion.Inverse(newObject.transform.rotation) * toConstrain.transform.rotation;
            toConstrain.GetComponent<ParentConstraint>().SetRotationOffset(0, rotationOffset.eulerAngles);
        }
        else
        {
            toConstrain.GetComponent<ParentConstraint>().SetTranslationOffset(0, new Vector3(0, 0, 0));
            toConstrain.GetComponent<ParentConstraint>().SetRotationOffset(0, new Vector3(0, 0, 0));
        }

    }
    public static void AddToConstraint(GameObject constraintTarget, GameObject toConstrain)
    {
        GameObject newObject = new GameObject(string.Format("{0} {1} Constraint", toConstrain.name, constraintTarget.name));
        newObject.transform.SetParent(constraintTarget.transform);
        newObject.transform.localPosition = new Vector3(0, 0, 0);
        ConstraintSource tempSource = new ConstraintSource
        {
            sourceTransform = newObject.transform,
            weight = 0F,
        };

        toConstrain.GetComponent<ParentConstraint>().AddSource((tempSource));
        toConstrain.GetComponent<ParentConstraint>().SetTranslationOffset(0, new Vector3(0, 0, 0));
        toConstrain.GetComponent<ParentConstraint>().SetRotationOffset(0, new Vector3(0, 0, 0));

    }
}

