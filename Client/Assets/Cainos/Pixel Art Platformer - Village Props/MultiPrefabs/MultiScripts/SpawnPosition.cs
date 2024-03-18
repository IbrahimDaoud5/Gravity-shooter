using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Position of targets", menuName = "ScriptableObjects/TargetsPositionScriptableObject", order = 1)]
public class SpawnPosition : ScriptableObject
{
    public List<Vector3> savedPositions;
}
