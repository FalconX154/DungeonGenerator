using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static DungeonGenerator;

public partial class DungeonGenerator : MonoBehaviour
{
    /// <summary>
    /// rooms generated are not equal to totalroomNumber
    /// room does not change to corresponding material
    /// </summary> 

    public enum ERoomType
    {
        free,
        normal,
        shop,
        miniboss,
        boss,
        secret,
    }

    private static ERoomType[,] map;
    private static List<Vector2Int> roomsGenerated;
    private static ILayoutInterpreter layoutInterpreter;
    private static DungeonInterpreter dungeonInterpreter;

    public static void InitializeMap()
    {
        if (dungeonInterpreter == null)
        {
            layoutInterpreter = dungeonInterpreter;
        }

        layoutInterpreter.InterpretLayout(map);
        map = new ERoomType[DungeonGeneratorUI.MapLength, DungeonGeneratorUI.MapWidth];
        roomsGenerated = new List<Vector2Int>();
        ResetMap();
    }

    private static void ResetMap()
    {
        for (int x = 0; x < DungeonGeneratorUI.MapLength; x++)
        {
            for (int y = 0; y < DungeonGeneratorUI.MapWidth; y++)
            {
                map[x, y] = ERoomType.free;
            }
        }

        roomsGenerated.Clear();
        GenerateMap();
    }

    private static void GenerateMap()
    {
        Vector2Int startCoordinates = GetRandomStartCoordinates();
;
        while (true)
        {
            roomsGenerated = GenerateLayout(startCoordinates);

            if (ValidateLayout())
            {
                break;
            }
        }

        GenerateSpecialRooms();
        Debug.Log(roomsGenerated.Count);
    }

    private static Vector2Int GetRandomStartCoordinates()
    {
        int randomX = Random.Range(0, DungeonGeneratorUI.MapWidth);
        int randomY = Random.Range(0, DungeonGeneratorUI.MapLength);

        return new Vector2Int(randomX, randomY);
    }

    private static List<Vector2Int> GenerateLayout(Vector2Int _startCoordinates)
    {
        ResetMap();

        map[_startCoordinates.x, _startCoordinates.y] = ERoomType.normal;

        List<Vector2Int> discoverList = new List<Vector2Int>();
        discoverList.Add(_startCoordinates);

        while (discoverList.Count > 0)
        {
            Vector2Int currentCoordinates = discoverList[discoverList.Count - 1];
            discoverList.RemoveAt(discoverList.Count - 1);

            Vector2Int[] neighbourCoordinates = new Vector2Int[]
            {
            currentCoordinates + Vector2Int.up,
            currentCoordinates + Vector2Int.down,
            currentCoordinates + Vector2Int.left,
            currentCoordinates + Vector2Int.right,
            };

            bool hasGeneratedARoom = false;

            for (int i = 0; i < neighbourCoordinates.Length; i++)
            {
                Vector2Int currentNeighbourCoordinate = neighbourCoordinates[i];

                if (!IsCoordinateInBounds(currentNeighbourCoordinate))
                {
                    continue;
                }

                if (roomsGenerated.Count >= DungeonGeneratorUI.TotalRoomNumber)
                {
                    break;
                }

                if (Random.Range(0, 2) == 0)
                {
                    continue;
                }

                if (CheckNearbyRooms(currentNeighbourCoordinate) > 1)
                {
                    continue;
                }

                map[currentNeighbourCoordinate.x, currentNeighbourCoordinate.y] = ERoomType.normal;
                roomsGenerated.Add(currentNeighbourCoordinate);
                discoverList.Add(currentNeighbourCoordinate);
                hasGeneratedARoom = true;
            }

            if (hasGeneratedARoom)
            {
                roomsGenerated.Add(currentCoordinates);
            }
        }

        return roomsGenerated;
    }

    private static void GenerateSpecialRooms()
    {
        AssignSpecialRooms(DungeonGeneratorUI.ShopRoomNumber, ERoomType.shop);
        AssignSpecialRooms(DungeonGeneratorUI.MinibossRoomNumber, ERoomType.miniboss);

        int bossRoomIndex = roomsGenerated.Count - 1;
        Vector2Int bossRoomCoordinates = roomsGenerated[bossRoomIndex];
        map[bossRoomCoordinates.x, bossRoomCoordinates.y] = ERoomType.boss;
        roomsGenerated.RemoveAt(bossRoomIndex);

        //for (int i = 0; i < secretRoomNumber; i++)
        //{
        //    int secretRoomIndex = Random.Range(0, roomsGenerated.Count);
        //    Vector2Int secretRoomCoordinates = roomsGenerated[secretRoomIndex];
        //    map[secretRoomCoordinates.x, secretRoomCoordinates.y] = ERoomType.secret;
        //    roomsGenerated.RemoveAt(secretRoomIndex);
        //}
    }

    private static void AssignSpecialRooms(int _roomNumber, ERoomType _eRoomType)
    {
        for (int i = 0; i < _roomNumber; i++)
        {
            int roomIndex = Random.Range(0, roomsGenerated.Count - 2);
            Vector2Int roomCoordinates = roomsGenerated[roomIndex];
            map[roomCoordinates.x, roomCoordinates.y] = _eRoomType;
            roomsGenerated.RemoveAt(roomIndex);
        }
    }

    private static bool ValidateLayout()
    {
        return roomsGenerated.Count >= DungeonGeneratorUI.TotalRoomNumber;
    }

    private static int CheckNearbyRooms(Vector2Int _coordinates)
    {
        Vector2Int[] neighbourCoordinates = new Vector2Int[]
        {
                _coordinates + Vector2Int.up,
                _coordinates + Vector2Int.down,
                _coordinates + Vector2Int.left,
                _coordinates + Vector2Int.right,
        };

        int neighbourCount = 0;

        for (int i = 0; i < neighbourCoordinates.Length; i++)
        {
            Vector2Int currentNeighbourCoordinate = neighbourCoordinates[i];

            if (!IsCoordinateInBounds(currentNeighbourCoordinate))
            {
                continue;
            }

            if (map[currentNeighbourCoordinate.x, currentNeighbourCoordinate.y] != ERoomType.free)
            {
                ++neighbourCount;
            }
        }

        return neighbourCount;
    }

    private static bool IsCoordinateInBounds(Vector2Int _coordinate)
    {
        if (_coordinate.x >= 0 && _coordinate.x < DungeonGeneratorUI.MapLength && _coordinate.y >= 0 && _coordinate.y < DungeonGeneratorUI.MapWidth)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}