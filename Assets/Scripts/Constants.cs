using UnityEngine;
using System.Collections;

public static class Constants
{
    public const int MaxRows = 4;
    public const int MaxColumns = 4;
    public const int MaxSize = MaxRows * MaxColumns;
}

public enum GameState
{
    Start,
    Playing,
    Animating,
    End
}