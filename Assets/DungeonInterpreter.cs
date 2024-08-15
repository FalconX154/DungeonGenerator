using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DungeonGenerator;

public sealed class DungeonInterpreter : MonoBehaviour, ILayoutInterpreter
{
    [SerializeField] private DungeonRoom room1Door;
    [SerializeField] private DungeonRoom room2DoorC;
    [SerializeField] private DungeonRoom room2DoorS;
    [SerializeField] private DungeonRoom room3Door;
    [SerializeField] private DungeonRoom room4Door;

    private ERoomType[,] currentLayout;
    private int currentLayoutLength;
    private int currentLayoutWidth;
    private const float roomSize = 10f;

    public void InterpretLayout(ERoomType[,] _layout)
    {
        currentLayout = _layout;
        currentLayoutLength = currentLayout.GetLength(0);
        currentLayoutWidth = currentLayout.GetLength(1);

        for (int x = 0; x < currentLayoutLength; x++)
        {
            for (int y = 0; y < currentLayoutWidth; y++)
            {
                if (currentLayout[x, y] == ERoomType.free)
                {
                    continue;
                }

                bool[] hasRoomInDirection = new bool[4];
                int neighbourCount = GetNeighbourCountAndDirections(ref hasRoomInDirection, new Vector2Int(x, y));

                DungeonRoom prefabToSpawn = SelectPrefab(neighbourCount, hasRoomInDirection);
                float rotation = DetermineRotation(neighbourCount, hasRoomInDirection);

                DungeonRoom newRoom = Instantiate(prefabToSpawn, this.transform);
                newRoom.transform.localRotation = Quaternion.Euler(Vector3.up * rotation);
                newRoom.transform.localPosition = new Vector3(x, 0f, y) * roomSize;
                newRoom.SetRoomType(currentLayout[x, y]);
            }
        }
    }

    private DungeonRoom SelectPrefab(int _neighbourCount, bool[] _hasRoomInDirection)
    {
        switch (_neighbourCount)
        {
            case 1: return room1Door;

            case 2:
                return (_hasRoomInDirection[0] && _hasRoomInDirection[2]) || (_hasRoomInDirection[1] && _hasRoomInDirection[3])
                    ? room2DoorS : room2DoorC;

            case 3: return room3Door;

            default: return room4Door;
        }
    }

    private float DetermineRotation(int _neighbourCount, bool[] _hasRoomInDirection)
    {
        switch (_neighbourCount)
        {
            case 1:
                if (_hasRoomInDirection[0]) return 0f;
                if (_hasRoomInDirection[1]) return 90f;
                if (_hasRoomInDirection[2]) return 180f;
                return -90f;

            case 2:
                if (_hasRoomInDirection[0] && _hasRoomInDirection[2]) return 0f;
                if (_hasRoomInDirection[1] && _hasRoomInDirection[3]) return 90f;
                if (_hasRoomInDirection[0] && _hasRoomInDirection[1]) return 0f;
                if (_hasRoomInDirection[0] && _hasRoomInDirection[3]) return -90f;
                if (_hasRoomInDirection[1] && _hasRoomInDirection[2]) return 90f;
                return 180f;

            case 3:
                if (_hasRoomInDirection[0]) return 180f;
                if (_hasRoomInDirection[1]) return -90f;
                if (_hasRoomInDirection[2]) return 0f;
                return 90f;

            default: return 0f;
        }
    }

    private int GetNeighbourCountAndDirections(ref bool[] _hasRoomInDirection, Vector2Int _coordinates)
    {
        if (_hasRoomInDirection == null || _hasRoomInDirection.Length != 4)
        {
            return 0;
        }

        Vector2Int[] neighbourCurrentCoordinates = new Vector2Int[]
        {
            _coordinates + Vector2Int.up,
            _coordinates + Vector2Int.down,
            _coordinates + Vector2Int.left,
            _coordinates + Vector2Int.right
        };

        int neighbourCount = 0;

        for (int i = 0; i < neighbourCurrentCoordinates.Length; i++)
        {
            Vector2Int neighbourCoordinates = neighbourCurrentCoordinates[i];
            _hasRoomInDirection[i] = false;

            if (IsCoordinateInBounds(neighbourCoordinates) && currentLayout[neighbourCoordinates.x, neighbourCoordinates.y] != ERoomType.free)
            {
                _hasRoomInDirection[i] = true;
                neighbourCount++;
            }
        }

        return neighbourCount;
    }

    private bool IsCoordinateInBounds(Vector2Int _coordinate)
    {
        if (_coordinate.x >= 0 && _coordinate.x < currentLayoutLength && _coordinate.y >= 0 && _coordinate.y < currentLayoutWidth)
        {
            return true;
        }
        else
        {
            return false; 
        }
    }
}