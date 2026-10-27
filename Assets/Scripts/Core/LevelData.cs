
using UnityEngine;

public enum LevelTileType
{
    Normal,
    Occupied
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Tower Defense/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] public int levelId = 1;

    [SerializeField] public int width = 18;
    [SerializeField] public int height = 10;

    [SerializeField]
    private LevelTileType[] tiles = new LevelTileType[40];

    public int TileCount => tiles == null ? 0 : tiles.Length;

    public LevelTileType GetTile(int x, int y)
    {
        return tiles[y * width + x];
    }

    public void SetTile(int x, int y, LevelTileType type)
    {
        tiles[y * width + x] = type;
    }

    public void Resize(int newWidth, int newHeight)
    {
        newWidth = Mathf.Max(1, newWidth);
        newHeight = Mathf.Max(1, newHeight);

        LevelTileType[] newTiles =
            new LevelTileType[newWidth * newHeight];

        if (tiles != null)
        {
            int copyWidth = Mathf.Min(width, newWidth);
            int copyHeight = Mathf.Min(height, newHeight);

            for (int y = 0; y < copyHeight; y++)
            {
                for (int x = 0; x < copyWidth; x++)
                {
                    int oldIndex = y * width + x;
                    int newIndex = y * newWidth + x;

                    if (oldIndex < tiles.Length)
                        newTiles[newIndex] = tiles[oldIndex];
                }
            }
        }

        width = newWidth;
        height = newHeight;
        tiles = newTiles;
    }

    public void Fill(LevelTileType type)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = type;
        }
    }
}
