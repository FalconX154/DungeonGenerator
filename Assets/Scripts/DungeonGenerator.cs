using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static DungeonGeneratorUI;

public partial class DungeonGenerator : MonoBehaviour
{
    public enum ERoomType
    {
        free,
        normal,
        shop,
        miniboss,
        boss,
    }

    private static ERoomType[,] map;
    private static List<Vector2Int> roomsList = new List<Vector2Int>();
    private static Dictionary<Vector2Int, ERoomType> specialRoomsDictionary = new Dictionary<Vector2Int, ERoomType>();
    private static ILayoutInterpreter layoutInterpreter;

    public static void InitializeMap()
    {
        layoutInterpreter = dungeonInterpreter;
        map = new ERoomType[MapLength, MapWidth];
        ResetMap();
    }

    private static void ResetMap()
    {
        for (int x = 0; x < MapLength; x++)
        {
            for (int y = 0; y < MapWidth; y++)
            {
                map[x, y] = ERoomType.free;
            }
        }

        roomsList.Clear();
    }

    public static void GenerateMap()
    {
        Vector2Int startCoordinates = GetRandomStartCoordinates();
        ;
        while (true)
        {
            roomsList = GenerateLayout(startCoordinates);

            if (ValidateLayout())
            {
                break;
            }
        }

        GenerateSpecialRooms();
        layoutInterpreter.InterpretLayout(map);
        //Debug.Log(roomsGenerated.Count);
    }

    private static Vector2Int GetRandomStartCoordinates()
    {
        int randomX = Random.Range(0, MapLength);
        int randomY = Random.Range(0, MapWidth);

        return new Vector2Int(randomX, randomY);
    }

    private static List<Vector2Int> GenerateLayout(Vector2Int _startCoordinates)
    {
        ResetMap();

        map[_startCoordinates.x, _startCoordinates.y] = ERoomType.normal;
        roomsList.Add(_startCoordinates);

        List<Vector2Int> discoverList = new List<Vector2Int>();
        discoverList.Add(_startCoordinates);

        while (roomsList.Count < TotalRoomNumber && discoverList.Count > 0)
        {
            Vector2Int currentCoordinates = discoverList[discoverList.Count - 1];
            discoverList.RemoveAt(discoverList.Count - 1);

            Vector2Int[] neighbourCoordinates = new Vector2Int[]
            {
            currentCoordinates + Vector2Int.up,
            currentCoordinates + Vector2Int.right,
            currentCoordinates + Vector2Int.down,
            currentCoordinates + Vector2Int.left,
            };

            bool hasGeneratedARoom = false;

            for (int i = 0; i < neighbourCoordinates.Length; i++)
            {
                Vector2Int currentNeighbourCoordinate = neighbourCoordinates[i];

                if (!IsCoordinateInBounds(currentNeighbourCoordinate) || roomsList.Count >= TotalRoomNumber)
                {
                    continue;
                }

                if (map[currentNeighbourCoordinate.x, currentNeighbourCoordinate.y] == ERoomType.free && CheckNearbyRooms(currentNeighbourCoordinate) <= 1)
                {
                    map[currentNeighbourCoordinate.x, currentNeighbourCoordinate.y] = ERoomType.normal;
                    roomsList.Add(currentNeighbourCoordinate);
                    discoverList.Add(currentNeighbourCoordinate);
                    hasGeneratedARoom = true;

                    if (roomsList.Count == TotalRoomNumber)
                    {
                        break;
                    }
                }
            }

            if (hasGeneratedARoom)
            {
                continue;
            }
        }

        return roomsList;
    }

    private static void GenerateSpecialRooms()
    {
        specialRoomsDictionary.Clear();

        while (specialRoomsDictionary.Count < ShopRoomNumber + MinibossRoomNumber)
        {
            AssignSpecialRooms(ShopRoomNumber, ERoomType.shop);
            AssignSpecialRooms(MinibossRoomNumber, ERoomType.miniboss);
        }

        int bossRoomIndex = roomsList.Count - 1;
        Vector2Int bossRoomCoordinates = roomsList[bossRoomIndex];
        map[bossRoomCoordinates.x, bossRoomCoordinates.y] = ERoomType.boss;
    }

    private static void AssignSpecialRooms(int _roomNumber, ERoomType _eRoomType)
    {
        for (int i = 0; i < _roomNumber; i++)
        {
            int roomIndex = Random.Range(0, roomsList.Count - 2);
            Vector2Int roomCoordinates = roomsList[roomIndex];

            if (map[roomCoordinates.x, roomCoordinates.y] == ERoomType.normal)
            {
                map[roomCoordinates.x, roomCoordinates.y] = _eRoomType;
                specialRoomsDictionary[roomCoordinates] = _eRoomType;
            }
            else
            {
                i--;
            }
        }
    }

    private static bool ValidateLayout()
    {
        if (roomsList.Count == TotalRoomNumber)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static int CheckNearbyRooms(Vector2Int _coordinates)
    {
        Vector2Int[] neighbourCoordinates = new Vector2Int[]
        {
                _coordinates + Vector2Int.up,
                _coordinates + Vector2Int.right,
                _coordinates + Vector2Int.down,
                _coordinates + Vector2Int.left,
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
        if (_coordinate.x >= 0 && _coordinate.x < MapLength && _coordinate.y >= 0 && _coordinate.y < MapWidth)
        {
            return true;
        }
        else
        {
            //Debug.LogWarning($"Coordinate {_coordinate} is out of bounds.");
            return false;
        }
    }
}