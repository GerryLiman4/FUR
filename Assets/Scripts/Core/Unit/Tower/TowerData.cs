using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Tower Data/Tower Configuration")]
public class TowerData : ScriptableObject
{
    public UnitId id;
    public string towerName;

    public int cost;
}
