using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    public void NewGameButton_OnClick()
    {
        string connection = $"URI=file:{Application.streamingAssetsPath}/GameData/OnlyWar.s3db";
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand command = dbcon.CreateCommand();
        command.CommandText = "SELECT * FROM Faction";
        var reader = command.ExecuteReader();
        while(reader.Read())
        {
            Debug.Log($"{reader[0]}, {reader[1]}, {reader[2]}");
        }
    }
}
