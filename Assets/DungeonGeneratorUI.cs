using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DungeonGeneratorUI : EditorWindow
{
    public static int TotalRoomNumber;
    public static int ShopRoomNumber;
    public static int MinibossRoomNumber;
    public static int SecretRoomNumber;
    public static int MapLength;
    public static int MapWidth;

    [MenuItem("Dungeon/Dungeon Generator")]

    public static void ShowWindow()
    {
        GetWindow(typeof(DungeonGeneratorUI));
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Dungeon", EditorStyles.boldLabel);

        GUILayout.Label("Map Size", EditorStyles.boldLabel);
        EditorGUILayout.IntField("Length: ", MapLength);
        EditorGUILayout.IntField("Width: ", MapWidth);

        GUILayout.Label("Rooms", EditorStyles.boldLabel);
        EditorGUILayout.IntField("Total Room Number: ", TotalRoomNumber);
        EditorGUILayout.IntField("Shop Room Number: ", ShopRoomNumber);
        EditorGUILayout.IntField("Miniboss Room Number: ", MinibossRoomNumber);
        EditorGUILayout.IntField("Secret Room Number: ", SecretRoomNumber);

        EditorGUI.BeginDisabledGroup(TotalRoomNumber < ShopRoomNumber + MinibossRoomNumber + SecretRoomNumber);
        if (GUILayout.Button("Generate Dungeon"))
        {
            DungeonGenerator.InitializeMap();

        }
        EditorGUI.EndDisabledGroup();

        if (TotalRoomNumber < ShopRoomNumber + MinibossRoomNumber + SecretRoomNumber)
        {
            EditorGUILayout.HelpBox("The Total Room Number must be higher than the addition of the special rooms.", MessageType.Warning);
        }
    }
}