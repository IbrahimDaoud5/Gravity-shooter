using UnityEngine;

using System.Collections.Generic;

[ExecuteInEditMode]
public class PositionSaver : MonoBehaviour
{
    public List<Vector3> savedPositions;

    void OnEnable()
    {
        SavePositions();
    }


    public void SavePositions()
    {
        savedPositions = new List<Vector3>();
        foreach (Transform child in transform)
        {
            savedPositions.Add(child.position);
            // Optionally, if you want to save the world position
            // savedPositions.Add(child.TransformPoint(child.position));
        }
    }
}
