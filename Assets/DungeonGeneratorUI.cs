using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static DungeonGenerator;

public class DungeonGeneratorUI : EditorWindow
{
    public static int TotalRoomNumber;
    public static int ShopRoomNumber;
    public static int MinibossRoomNumber;
    public static int SecretRoomNumber;
    public static int MapLength;
    public static int MapWidth;
    public static DungeonInterpreter dungeonInterpreter;

    [MenuItem("Dungeon/Dungeon Generator")]

    public static void ShowWindow()
    {
        GetWindow(typeof(DungeonGeneratorUI));
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Dungeon", EditorStyles.boldLabel);

        GUILayout.Label("Map Size", EditorStyles.boldLabel);
        MapLength = EditorGUILayout.IntField("Length: ", MapLength);
        MapWidth = EditorGUILayout.IntField("Width: ", MapWidth);

        GUILayout.Label("Rooms", EditorStyles.boldLabel);
        TotalRoomNumber = EditorGUILayout.IntField("Total Room Number: ", TotalRoomNumber);
        ShopRoomNumber = EditorGUILayout.IntField("Shop Room Number: ", ShopRoomNumber);
        MinibossRoomNumber = EditorGUILayout.IntField("Miniboss Room Number: ", MinibossRoomNumber);
        SecretRoomNumber = EditorGUILayout.IntField("Secret Room Number: ", SecretRoomNumber);

        dungeonInterpreter = EditorGUILayout.ObjectField(dungeonInterpreter, typeof(DungeonInterpreter), true) as DungeonInterpreter;

        EditorGUI.BeginDisabledGroup(TotalRoomNumber < ShopRoomNumber + MinibossRoomNumber + SecretRoomNumber || dungeonInterpreter == null || MapLength * MapWidth < TotalRoomNumber);
        if (GUILayout.Button("Generate Dungeon"))
        {
            InitializeMap();
            GenerateMap();
        }
        EditorGUI.EndDisabledGroup();

        if (TotalRoomNumber < ShopRoomNumber + MinibossRoomNumber + SecretRoomNumber)
        {
            EditorGUILayout.HelpBox("The Total Room Number must be higher than the addition of the special rooms.", MessageType.Warning);
        }
    }
}