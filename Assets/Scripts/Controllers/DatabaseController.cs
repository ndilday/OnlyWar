using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
