using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardDataObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI idField;
    [SerializeField]
    private TextMeshProUGUI nameField;
    [SerializeField]
    private TextMeshProUGUI surnameField;
    [SerializeField]
    private TextMeshProUGUI scoreField;
    private LeaderboardData leaderboardData;
    public LeaderboardData LeaderboardData
    {
        get
        {
            return leaderboardData;
        }
        set
        {
            leaderboardData = value;
            if(value != null)
            {
                idField.text = value.Place.ToString();
                nameField.text = value.Name;
                surnameField.text = value.Surname;
                scoreField.text = value.Score.ToString();
            }
        }
    }
}
