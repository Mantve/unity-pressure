using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class LeaderboardManager : MonoBehaviour
{
    private Dictionary<int, LeaderboardDataObject> leaderboardDataObjects = new Dictionary<int, LeaderboardDataObject>();
    [SerializeField]
    private Transform content;

    [SerializeField]
    private string name;
    [SerializeField]
    private string surname;
    [SerializeField]
    private float score;

    private const string LeaderboardSave = "LeaderboardSave_";
    private const string LeaderboardSaveCount = "LeaderboardSaveCount_";

    private void Start()
    {
        LoadLeaderboardData();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddScore(name, surname, score);
        }
    }
    public void AddScore(string name, string surname, float score)
    {
        GameObject element = LoadElement();
        if (element.TryGetComponent(out LeaderboardDataObject leaderboardDataObject)) {
            int id = CalculatePosition(score);
            LeaderboardData newData = new LeaderboardData(id, name, surname, score);
            leaderboardDataObject.LeaderboardData = newData;
            PlaceScoreInPosition(leaderboardDataObject);
            SaveLeaderboardData();
        }
    }
    private void PlaceScoreInPosition(LeaderboardDataObject leaderboardDataObject)
    {
        LeaderboardData leaderboardData = leaderboardDataObject.LeaderboardData;
        if (leaderboardDataObjects.ContainsKey(leaderboardData.Place))
        {
            LeaderboardDataObject oldData = SwapData(leaderboardDataObject.LeaderboardData.Place, leaderboardDataObject);
            for (int i = leaderboardData.Place + 1; i < leaderboardDataObjects.Count + 1; i++)
            {
                if (leaderboardDataObjects.ContainsKey(i))
                {
                    oldData = SwapData(i, oldData);
                }
                else
                {
                    leaderboardDataObjects.Add(i, oldData);
                    oldData.LeaderboardData = new LeaderboardData(i, oldData.LeaderboardData);
                    oldData.gameObject.transform.SetSiblingIndex(i);
                    break;
                }
            }
        }
        else
        {
            leaderboardDataObjects.Add(leaderboardData.Place, leaderboardDataObject);
        }
    }
    private LeaderboardDataObject SwapData(int place, LeaderboardDataObject data)
    {
        LeaderboardDataObject oldData = leaderboardDataObjects[place];
        leaderboardDataObjects[place] = data;
        leaderboardDataObjects[place].LeaderboardData = new LeaderboardData(place, data.LeaderboardData);
        data.gameObject.transform.SetSiblingIndex(place);
        return oldData;
    }
    private void SaveLeaderboardData()
    {
        foreach(int key in leaderboardDataObjects.Keys)
        {
            LeaderboardData leaderboardData = leaderboardDataObjects[key].LeaderboardData;
            PlayerPrefs.SetString(LeaderboardSave + key, JsonUtility.ToJson(leaderboardData));
        }
        PlayerPrefs.SetInt(LeaderboardSaveCount, leaderboardDataObjects.Count);
        PlayerPrefs.Save();
    }
    private void LoadLeaderboardData()
    {
        int count = PlayerPrefs.GetInt(LeaderboardSaveCount);
        leaderboardDataObjects = new Dictionary<int, LeaderboardDataObject>();
        for(int i = 0; i < count; i++)
        {
            string dataLine = PlayerPrefs.GetString(LeaderboardSave + i);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(dataLine);
            if (!leaderboardDataObjects.ContainsKey(i))
            {
                GameObject element = LoadElement();
                if (element.TryGetComponent(out LeaderboardDataObject leaderboardDataObject))
                {
                    leaderboardDataObject.LeaderboardData = data;
                    leaderboardDataObjects.Add(i, leaderboardDataObject);
                }
            }
        }
    }
    private int CalculatePosition(float score)
    {
        int id = leaderboardDataObjects.Count;
        foreach (LeaderboardDataObject leaderboardDataObject in leaderboardDataObjects.Values)
        {
            if(leaderboardDataObject.LeaderboardData.Score < score && leaderboardDataObject.LeaderboardData.Place < id)
            {
                id = leaderboardDataObject.LeaderboardData.Place;
            }
        }
        return id;
    }
    private GameObject LoadElement()
    {
        GameObject element = (GameObject)Resources.Load("UILeaderboard/Element", typeof(GameObject));
        GameObject newElement = Instantiate(element, content);
        return newElement;
    }
}

[Serializable]
public class LeaderboardData
{
    [SerializeField]
    private int place;
    [SerializeField]
    private string name;
    [SerializeField]
    private string surname;
    [SerializeField]
    private float score;

    public int Place { get => place; set => place = value; }
    public string Name { get => name; set => name = value; }
    public string Surname { get => surname; set => surname = value; }
    public float Score { get => score; set => score = value; }

    public LeaderboardData(int place, string name, string surname, float score)
    {
        Place = place;
        Name = name;
        Surname = surname;
        Score = score;
    }
    public LeaderboardData(int place, LeaderboardData data)
    {
        Place = place;
        Name = data.Name;
        Surname = data.Surname;
        Score = data.Score;
    }
}
