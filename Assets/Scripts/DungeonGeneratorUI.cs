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

        EditorGUILayout.Space();

        GUILayout.Label("Map Size", EditorStyles.boldLabel);
        MapLength = EditorGUILayout.IntField("Length: ", MapLength);
        MapWidth = EditorGUILayout.IntField("Width: ", MapWidth);

        EditorGUILayout.Space();

        GUILayout.Label("Rooms", EditorStyles.boldLabel);
        TotalRoomNumber = EditorGUILayout.IntField("Total Room Number: ", TotalRoomNumber);

        GUILayout.Label("Special Rooms", EditorStyles.miniBoldLabel);
        ShopRoomNumber = EditorGUILayout.IntField("Shop Room Number: ", ShopRoomNumber);
        MinibossRoomNumber = EditorGUILayout.IntField("Miniboss Room Number: ", MinibossRoomNumber);

        EditorGUILayout.Space();

        dungeonInterpreter = EditorGUILayout.ObjectField(dungeonInterpreter, typeof(DungeonInterpreter), true) as DungeonInterpreter;

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(TotalRoomNumber < ShopRoomNumber + MinibossRoomNumber || dungeonInterpreter == null || MapLength * MapWidth <= TotalRoomNumber || TotalRoomNumber <= 1 || ShopRoomNumber + MinibossRoomNumber == TotalRoomNumber - 1 || dungeonInterpreter.room1Door == null);
        if (GUILayout.Button("Generate Dungeon"))
        {
            InitializeMap();
            GenerateMap();
        }
        EditorGUI.EndDisabledGroup();

        #region Warnings
        if (TotalRoomNumber < ShopRoomNumber + MinibossRoomNumber)
        {
            EditorGUILayout.HelpBox("The Total Room Number must be higher than the addition of the special rooms!", MessageType.Warning);
        }

        if (dungeonInterpreter == null)
        {
            EditorGUILayout.HelpBox("A Scriptable Object must be used as an interpreter for the dungeon!", MessageType.Warning);
        }

        if (MapLength * MapWidth <= TotalRoomNumber)
        {
            EditorGUILayout.HelpBox("The Total Room Number must be equal or smaller than the size of the map!", MessageType.Warning);
        }

        if (TotalRoomNumber <= 1)
        {
            EditorGUILayout.HelpBox("The Total Room Number must be higher than 1!", MessageType.Warning);
        }

        if (ShopRoomNumber + MinibossRoomNumber == TotalRoomNumber - 1)
        {
            EditorGUILayout.HelpBox("The number of Special Rooms must be lower than the Total Room Number minus 1!", MessageType.Warning);
        }

        if (dungeonInterpreter != null)
        {
            if (dungeonInterpreter.room1Door == null || dungeonInterpreter.room2DoorC == null || dungeonInterpreter.room2DoorS == null || dungeonInterpreter.room3Door == null || dungeonInterpreter.room4Door == null)
            {
                EditorGUILayout.HelpBox("Your Scriptable Object is missing a Prefab!", MessageType.Warning);
            }
        }      
        #endregion
    }
}