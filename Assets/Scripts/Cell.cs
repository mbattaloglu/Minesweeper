using UnityEngine;

public struct Cell
{
    public Vector3Int position;
    public CellType type;
    public int number;
    public bool revealed;
    public bool flagged;
    public bool exploded;
}
