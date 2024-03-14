using System.Collections;
using Aptos.Unity.Rest;
using Aptos.Unity.Rest.Model;
using SimpleJSON;
using UnityEngine;

public class RaceObjectManager : MonoBehaviour
{
    public Race raceGo;
    public Transform raceSpawn;
    public IEnumerator GetRaceDataAsync()
    {
        ResponseInfo responseInfo = new();
        string data = "";

        ViewRequest viewRequest = new()
        {
            Function = "0xa94a9da70feb4596757bce720b8b612c9ef54783f84316f7cb5523b5eb4e47d7::aptos_horses_game::get_all_races_metadata",
            TypeArguments = new string[] { },
            Arguments = new string[] { }
        };

        Coroutine getUser = StartCoroutine(RestClient.Instance.View((_data, _responseInfo) =>
        {
            if (_data != null) data = _data;
            responseInfo = _responseInfo;

        }, viewRequest));

        yield return getUser;

        if (responseInfo.status == ResponseInfo.Status.Failed) Debug.LogError("Error: Fetching data failed!");
        else AssignData(data);
    }

    private void AssignData(string data)
    {
        JSONNode node = JSON.Parse(data);

        foreach (Transform t in raceSpawn)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < node[0].Count; i++)
        {
            Instantiate(raceGo, raceSpawn).SetupRace(
                node[0][i]["race_id"].AsULong,
                node[0][i]["name"].Value,
                node[0][i]["price"].AsInt,
                node[0][i]["laps"].AsInt,
                node[0][i]["started"].AsBool,
                node[0][i]["players"]
            );
        }
    }
}