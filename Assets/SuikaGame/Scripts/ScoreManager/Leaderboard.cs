using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public struct Highscore
{
    public string username;
    public int score;

    public Highscore(string _username, int _score)
    {
        this.username = _username;
        this.score = _score;
    }
}

public class Leaderboard : MonoBehaviour
{
    const string privateCode = "U8XTCocb50afGaaKDmlENAk1HBfvs4R0eSTwuQaN5cPg";
    const string publicCode = "693535d08f40bb18643bbbbc";
    const string webURL = "http://dreamlo.com/lb/";

    private Highscore[] highScoresList;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI[] usernameText;
    [SerializeField] private TextMeshProUGUI[] highScoreText;

    void Start()
    {
        GetHighScore();
    }

    void Update()
    {
        
    }

    private void AddNewHighScore(string username, int score)
    {
        StartCoroutine(UploadHighScore(username, score));
    }

    private void GetHighScore()
    {
        StartCoroutine(DownloadHighScore());
    }

    private IEnumerator UploadHighScore(string username, int score)
    {
        UnityWebRequest www = new UnityWebRequest(webURL + privateCode + "/add/" + UnityWebRequest.EscapeURL(username) + "/" + score);
        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Upload Successful");
        }
        else
        {
            Debug.Log("Error uploading: " + www.error);
        }
    }

    private IEnumerator DownloadHighScore()
    {
        string url = webURL + publicCode + "/pipe/";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string text = www.downloadHandler.text;
            Debug.Log("Downloaded:\n" + text);
            FormatHighscores(text);
        }
        else
        {
            Debug.Log("Error downloading: " + www.error);
        }
    }

    private void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highScoresList = new Highscore[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highScoresList[i] = new Highscore(username, score);
            usernameText[i].text = highScoresList[i].username;
            highScoreText[i].text = highScoresList[i].score.ToString();
            Debug.Log(highScoresList[i].username + ": " + highScoresList[i].score);
        }
    }

}
