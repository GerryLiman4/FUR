
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private const float TileSize = 32f;

    private LevelTileType selectedBrush = LevelTileType.Normal;
    private Vector2 scrollPosition;

    private GUIStyle tileLabelStyle;

    private void OnEnable()
    {
        tileLabelStyle = new GUIStyle(EditorStyles.boldLabel);
        tileLabelStyle.alignment = TextAnchor.MiddleCenter;
        tileLabelStyle.normal.textColor = Color.white;
    }

    public override void OnInspectorGUI()
    {
        LevelData data = (LevelData)target;

        // Level ID
        serializedObject.Update();
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("levelId")
        );
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(
            "Level Layout",
            EditorStyles.boldLabel
        );

        // Grid dimensions
        int newWidth = EditorGUILayout.IntField(
            "Columns", data.width
        );

        int newHeight = EditorGUILayout.IntField(
            "Rows", data.height
        );

        if (newWidth != data.width || newHeight != data.height
            || data.TileCount != data.width * data.height)
        {
            Undo.RecordObject(data, "Resize Level");
            data.Resize(newWidth, newHeight);
            EditorUtility.SetDirty(data);
        }

        EditorGUILayout.Space(8);

        // Brush selection
        EditorGUILayout.LabelField("Selected Brush");

        selectedBrush = (LevelTileType)GUILayout.Toolbar(
            (int)selectedBrush,
            new string[]
            {
                "Normal (Path)",
                "Occupied (Build)"
            }
        );

        EditorGUILayout.Space(10);

        DrawGrid(data);

        EditorGUILayout.Space(10);

        // Utility buttons
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Fill Normal"))
        {
            Undo.RecordObject(data, "Fill Normal");
            data.Fill(LevelTileType.Normal);
            EditorUtility.SetDirty(data);
        }

        if (GUILayout.Button("Fill Occupied"))
        {
            Undo.RecordObject(data, "Fill Occupied");
            data.Fill(LevelTileType.Occupied);
            EditorUtility.SetDirty(data);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawGrid(LevelData data)
    {
        float gridWidth = data.width * TileSize;
        float gridHeight = data.height * TileSize;

        scrollPosition = EditorGUILayout.BeginScrollView(
            scrollPosition,
            GUILayout.Height(Mathf.Min(gridHeight + 20f, 350f))
        );

        Rect gridRect = GUILayoutUtility.GetRect(
            gridWidth,
            gridHeight,
            GUILayout.ExpandWidth(false)
        );

        // Draw cells
        for (int row = 0; row < data.height; row++)
        {
            int y = data.height - row - 1;

            for (int x = 0; x < data.width; x++)
            {
                LevelTileType type = data.GetTile(x, y);

                Rect cellRect = new Rect(
                    gridRect.x + x * TileSize,
                    gridRect.y + row * TileSize,
                    TileSize - 1,
                    TileSize - 1
                );

                Color color = type == LevelTileType.Normal
                    ? new Color(0.2f, 0.4f, 0.65f)
                    : new Color(0.2f, 0.5f, 0.3f);

                EditorGUI.DrawRect(cellRect, color);

                GUI.Label(
                    cellRect,
                    type == LevelTileType.Normal ? "P" : "B",
                    tileLabelStyle
                );
            }
        }

        // Mouse painting
        Event e = Event.current;

        int controlId = GUIUtility.GetControlID(
            FocusType.Passive,
            gridRect
        );

        if (e.type == EventType.MouseDown &&
            e.button == 0 &&
            gridRect.Contains(e.mousePosition))
        {
            GUIUtility.hotControl = controlId;
            PaintAtMouse(data, gridRect, e.mousePosition);
            e.Use();
        }

        if (e.type == EventType.MouseDrag &&
            GUIUtility.hotControl == controlId)
        {
            PaintAtMouse(data, gridRect, e.mousePosition);
            e.Use();
        }

        if (e.type == EventType.MouseUp &&
            e.button == 0 &&
            GUIUtility.hotControl == controlId)
        {
            GUIUtility.hotControl = 0;
            e.Use();
        }

        EditorGUILayout.EndScrollView();
    }

    private void PaintAtMouse(
        LevelData data,
        Rect gridRect,
        Vector2 mousePosition)
    {
        if (!gridRect.Contains(mousePosition))
            return;

        int x = Mathf.FloorToInt(
            (mousePosition.x - gridRect.x) / TileSize
        );

        int row = Mathf.FloorToInt(
            (mousePosition.y - gridRect.y) / TileSize
        );

        int y = data.height - row - 1;

        if (x < 0 || x >= data.width ||
            y < 0 || y >= data.height)
            return;

        if (data.GetTile(x, y) == selectedBrush)
            return;

        Undo.RecordObject(data, "Paint Level Tile");

        data.SetTile(x, y, selectedBrush);

        EditorUtility.SetDirty(data);

        Repaint();
    }
}
