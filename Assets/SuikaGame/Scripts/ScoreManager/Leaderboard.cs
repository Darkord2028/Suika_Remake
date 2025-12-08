using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Highscore[] highScoresList = new Highscore[0];

    [Header("UI Elements (3 slots expected)")]
    [SerializeField] private TextMeshProUGUI[] usernameText;
    [SerializeField] private TextMeshProUGUI[] highScoreText;

    void Start()
    {
        GetHighScore();
    }

    public void UpdateLeaderboard(string username, int score)
    {
        if (!CanUpdateHigscore(username, score))
            return;

        StartCoroutine(UploadHighScore(username, score));
    }

    private IEnumerator UploadHighScore(string username, int score)
    {
        string url = webURL + privateCode + "/add/" + UnityWebRequest.EscapeURL(username) + "/" + score;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                GetHighScore();
            }
            else
            {
                Debug.Log("Error uploading: " + www.error);
            }
        }
    }

    private void GetHighScore()
    {
        StartCoroutine(DownloadHighScore());
    }

    private IEnumerator DownloadHighScore()
    {
        string url = webURL + publicCode + "/pipe/";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string text = www.downloadHandler.text;
                FormatHighscores(text);
            }
            else
            {
                Debug.Log("Error downloading: " + www.error);
            }
        }
    }

    private void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(
            new char[] { '\n' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        List<Highscore> tempList = new List<Highscore>();

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split('|');
            if (entryInfo.Length < 2) continue;

            string username = entryInfo[0];
            if (!int.TryParse(entryInfo[1], out int score)) continue;

            tempList.Add(new Highscore(username, score));
        }

        tempList = tempList
            .OrderByDescending(h => h.score)
            .Take(3)
            .ToList();

        highScoresList = tempList.ToArray();

        int slots = Mathf.Min(3, usernameText.Length, highScoreText.Length);

        for (int i = 0; i < slots; i++)
        {
            if (i < highScoresList.Length)
            {
                usernameText[i].text = highScoresList[i].username;
                highScoreText[i].text = highScoresList[i].score.ToString();
            }
            else
            {
                usernameText[i].text = "---";
                highScoreText[i].text = "0";
            }
        }
    }

    private bool CanUpdateHigscore(string username, int score)
    {
        List<Highscore> tempList = highScoresList.ToList();
        tempList.Add(new Highscore(username, score));
        Highscore[] newTop = tempList
            .OrderByDescending(h => h.score)
            .Take(3)
            .ToArray();

        if (highScoresList == null || highScoresList.Length == 0)
            return true;

        if (newTop.Length != highScoresList.Length) return true;

        for (int i = 0; i < newTop.Length; i++)
        {
            if (newTop[i].username != highScoresList[i].username ||
                newTop[i].score != highScoresList[i].score)
            {
                return true;
            }
        }

        return false;
    }

    public void UploadTopThree()
    {
        StartCoroutine(UploadTopThreeCoroutine());
    }

    private IEnumerator UploadTopThreeCoroutine()
    {
        using (UnityWebRequest clearReq = UnityWebRequest.Get(webURL + privateCode + "/clear"))
        {
            yield return clearReq.SendWebRequest();

            if (clearReq.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Failed to clear existing highscores: " + clearReq.error);
                yield break;
            }
        }

        for (int i = 0; i < highScoresList.Length && i < 3; i++)
        {
            string url = webURL + privateCode + "/add/" +
                         UnityWebRequest.EscapeURL(highScoresList[i].username) + "/" +
                         highScoresList[i].score;

            using (UnityWebRequest uploadReq = UnityWebRequest.Get(url))
            {
                yield return uploadReq.SendWebRequest();

                if (uploadReq.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Failed to upload highscore: " + uploadReq.error);
                }
            }
        }

        Debug.Log("Uploaded only top three highscores.");
        GetHighScore();
    }
}
