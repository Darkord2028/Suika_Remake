using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrefManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName_InputField;
    [SerializeField] private Button submitButton;

    private void Start()
    {
        if (PlayerPrefs.HasKey("playerID"))
        {
            string playerID = PlayerPrefs.GetString("playerID");
            DebugPlayerName(playerID);
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            playerName_InputField.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(true);
        }

        submitButton.onClick.AddListener(() =>
        {
            string playerName = playerName_InputField.text;

            if (string.IsNullOrEmpty(playerName))
            {
                playerName_InputField.placeholder.GetComponent<TMP_Text>().text = "Name cannot be empty!";
                playerName_InputField.placeholder.GetComponent<TMP_Text>().color = Color.red;
            }
            else
            {
                SavePlayerName(playerName);
            }

        });
    }

    private void DebugPlayerName(string name)
    {
        string id = System.Guid.NewGuid().ToString();
        Debug.Log(name + id);
    }

    private void SavePlayerName(string playerName)
    {
        if (!PlayerPrefs.HasKey("playerID"))
        {
            PlayerPrefs.SetString("playerID", playerName);
        }
        string playerID = PlayerPrefs.GetString("playerID");
        SceneManager.LoadScene("MainScene");
    }
}
