using UnityEngine;
using GoShared;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CoordinateData", order = 3)]
public class CoordinateData : ScriptableObject
{
    public Coordinates Coordinates;
}