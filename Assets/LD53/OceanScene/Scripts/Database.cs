using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "LD53/Create database")]
public class Database : ScriptableObject
{
    public float StartMoney;
    public float FuelPrice;
    public int MaxCargoCount;
    public float MaxCargoSlotSize;
    public int WinMoney;

    public List<Story> storyPoints;

    public string GetStoryPoint(StoryKey key)
    {
        return storyPoints.First(f => f.Key == key).Message;
    }
}

public static class DatabaseProvider
{
    static Database database;

    public static Database Get()
    {
        if (database == null)
        {
            database = Resources.Load<Database>("Database");
        }

        return database;
    }
}

[System.Serializable]
public class Story
{
    public string Name;
    public StoryKey Key;
    [TextArea]
    public string Message;
}

[System.Serializable]
public enum StoryKey
{
    StoryStart,
    StoryEndFail,
    StoryEndSuccess,
    FallOffTheWorld
}