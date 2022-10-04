using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class TestScript : MonoBehaviour
{
    public string sheetLink;
    public int column;
    public int firstRow = 1;
    public int rowCount = 1;
    public float lessValue;
    public float greaterValue;
    public AudioClip lessSpeak;
    public AudioClip normalSpeak;
    public AudioClip greaterSpeak;
    private AudioSource selectAudio;
    private Dictionary<string,float> dataSet = new Dictionary<string, float>();
    private bool statusStart = false;
    private int i = 1;


    // Start is called before the first frame update
    void Start()
    {
        i = firstRow;
        StartCoroutine(GoogleSheets());
    }

    // Update is called once per frame
    void Update()
    {
        if (i != (dataSet.Count + firstRow) &&dataSet["Mon_" + i.ToString()] <= lessValue && statusStart == false)
        {
            StartCoroutine(PlaySelectAudioGood());
            Debug.Log(dataSet["Mon_" + i.ToString()]);
        }

        if (i != (dataSet.Count + firstRow) && dataSet["Mon_" + i.ToString()] > lessValue && dataSet["Mon_" + i.ToString()]
            < greaterValue && statusStart == false)
        {
            StartCoroutine(PlaySelectAudioNormal());
            Debug.Log(dataSet["Mon_" + i.ToString()]);
        }

        if (i != (dataSet.Count + firstRow) &&dataSet["Mon_" + i.ToString()] >= greaterValue && statusStart == false)
        {
            StartCoroutine(PlaySelectAudioBad());
            Debug.Log(dataSet["Mon_" + i.ToString()]);
        }
    }

    IEnumerator GoogleSheets()
    {
        UnityWebRequest curentResp = UnityWebRequest.Get(sheetLink);
        yield return curentResp.SendWebRequest();
        string rawResp = curentResp.downloadHandler.text;
        var rawJson = JSON.Parse(rawResp);


        for (int j = firstRow; j < rawJson["values"].Count && ((j <= rowCount + firstRow) || rowCount == 0); j++)
        {
            var parseJson = JSON.Parse(rawJson["values"][j].ToString());
            var selectRow = parseJson;
            dataSet.Add("Mon_" + j.ToString(), float.Parse(selectRow[column - 1]));
        }
    }

    IEnumerator PlaySelectAudioGood()
    {
        statusStart = true;
        selectAudio = GetComponent<AudioSource>();
        selectAudio.clip = lessSpeak;
        selectAudio.Play();
        yield return new WaitForSeconds(3);
        statusStart = false;
        i++;
    }
    IEnumerator PlaySelectAudioNormal()
    {
        statusStart = true;
        selectAudio = GetComponent<AudioSource>();
        selectAudio.clip = normalSpeak;
        selectAudio.Play();
        yield return new WaitForSeconds(3);
        statusStart = false;
        i++;
    }
    IEnumerator PlaySelectAudioBad()
    {
        statusStart = true;
        selectAudio = GetComponent<AudioSource>();
        selectAudio.clip = greaterSpeak;
        selectAudio.Play();
        yield return new WaitForSeconds(4);
        statusStart = false;
        i++;
    }
}