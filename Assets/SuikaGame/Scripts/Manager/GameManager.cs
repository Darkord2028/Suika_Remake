using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [SerializeField] ObjectPoolManager poolManager;

    private GameObject prevAnimal;
    public List<GameObject> animalTable = new List<GameObject>();

    [Header("Player Data")]
    [SerializeField] InputManager inputManager;
    public Dropper dropper;
    [SerializeField] float inputDelay;

    [Header("Animal Data")]
    public GameObject[] animalPrefabs;

    [Header("Particle Data")]
    public GameObject combineParticleSystem;
    [SerializeField] ParticleSystem allClearParticleSystem;

    [Header("Audio Data")]
    public GameObject combineAudioSource;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Button playAgainButton;
    [SerializeField] GameObject gameOverBackground;
    public static int highScore = 0;
    public bool gameOver = false;

    [Header("Game Over Data")]
    [SerializeField] Transform rightDeathCheckPosition;
    [SerializeField] Transform leftDeathCheckPosition;
    [SerializeField] int numberOfRaycasts;
    [SerializeField] float rayDistance;
    [SerializeField] float deathCheckTime;
    [SerializeField] LayerMask hitLayerMask;
    private float currentDeathTimer;

    private Leaderboard leaderboard;

    #region Unity Callback Functions

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        playAgainButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }

    private void Start()
    {
        leaderboard = GameObject.FindFirstObjectByType<Leaderboard>();
        poolManager = GameObject.FindFirstObjectByType<ObjectPoolManager>();
        InitializePoolObjects();
    }

    private void Update()
    {
        CheckForGameEnd();
    }

    #endregion

    #region Object Pool Funtions

    private void InitializePoolObjects()
    {
        poolManager.InitializeNewPool(combineParticleSystem);
        poolManager.InitializeNewPool(combineAudioSource);

        for (int i = 0; i < animalPrefabs.Length; i++)
        {
            poolManager.InitializeNewPool(animalPrefabs[i]);
        }
    }

    /// <summary>
    /// Instantiates the fruit prefab with the given parameters
    /// </summary>
    public GameObject GetAnimal(Vector3 position, Vector3 rotation, int id)
    {
        GameObject newFruit = poolManager.GetObject(animalPrefabs[id], position, Vector3.zero);
        newFruit.GetComponent<Animal>().id = id;
        prevAnimal = newFruit;
        return newFruit;
    }

    public ParticleSystem GetParticle(Vector3 position, Vector3 rotation)
    {
        return poolManager.GetObject(combineParticleSystem, position, rotation).GetComponent<ParticleSystem>();
    }

    public AudioSource GetAudioSource(Vector3 position, Vector3 rotation)
    {
        return poolManager.GetObject(combineAudioSource, position, rotation).GetComponent<AudioSource>();
    }

    public void ReturnObject(GameObject gameObject, float delay = 0)
    {
        if (delay == 0)
        {
            poolManager.ReturnToPool(gameObject);
            animalTable.Remove(gameObject);
        }
        else
        {
            poolManager.ReturnObject(gameObject, delay);
        }
    }

    #endregion

    #region Other Functions

    /// <summary>
    /// Adds score based on the id of the fruit
    /// </summary>
    public void addScore(int id) {
        switch (id) {
            case 0:
                highScore += 1;
                break;
            case 1:
                highScore += 3;
                break;
            case 2:
                highScore += 6;
                break;
            case 3:
                highScore += 10;
                break;
            case 4:
                highScore += 15;
                break;
            case 5:
                highScore += 21;
                break;
            case 6:
                highScore += 28;
                break;
            case 7:
                highScore += 36;
                break;
            case 8:
                highScore += 45;
                break;
            case 9:
                highScore += 55;
                break;
            case 10:
                highScore += 66;
                break;
        }
        scoreText.text = highScore.ToString();
    }

    public void AddAnimalToList(GameObject gameObject = null)
    {
        if(gameObject == null)
        {
            animalTable.Add(prevAnimal);
        }
        else
        {
            animalTable.Add(gameObject);
        }
    }

    #endregion

    #region Check Functions

    private bool CheckIfDead()
    {
        if (gameOver) return false;

        float rayWidth = rightDeathCheckPosition.position.x - leftDeathCheckPosition.position.x;
        float raySpacing = rayWidth/ (numberOfRaycasts + 1);

        for (int i = numberOfRaycasts; i > 0; i--)
        {
            float xPosition = leftDeathCheckPosition.position.x + (raySpacing * i);
            Vector2 rayOrigin = new Vector2(xPosition, leftDeathCheckPosition.position.y);
            if(Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance, hitLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    private void CheckForGameEnd()
    {
        if (CheckIfDead())
        {
            currentDeathTimer -= Time.deltaTime;
            if(currentDeathTimer <= 0)
            {
                endGame();
            }
        }
        else
        {
            currentDeathTimer = deathCheckTime;
        }
    }

    public void endGame() 
    {
        gameOver = true;
        allClearParticleSystem.Play();
        inputManager.SetPlayerInput(false);
        gameOverBackground.SetActive(true);
        leaderboard.UpdateLeaderboard(PlayerPrefs.GetString("playerID", "Player"), highScore);
        //Debug.Log("Trigger Game Over");
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator PlayerInputDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (CheckIfDead())
        {
            gameOver = true;
            inputManager.SetPlayerInput(false);
            HandleDeath();
            yield break;
        }
        else
        {
            inputManager.SetPlayerInput(true);
        }
    }

    private void HandleDeath()
    {        
        gameOver = false;
        int length = animalTable.Count;
        for (int i = 0; i < length; i++)
        {
            GameObject instance = animalTable[0];
            animalTable.Remove(instance);
            ReturnObject(instance);
        }

        GameObject whale = GetAnimal(Vector3.zero, Vector3.zero, 10);
        animalTable.Add(whale);
        inputManager.SetPlayerInput(true);
        allClearParticleSystem.Play();
    }

    #endregion

    #region Gizmos Function

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rightDeathCheckPosition.position, leftDeathCheckPosition.position);

        float rayWidth = rightDeathCheckPosition.position.x - leftDeathCheckPosition.position.x;
        float raySpacing = rayWidth / (numberOfRaycasts + 1);

        for (int i = numberOfRaycasts; i > 0; i--)
        {
            float xPosition = leftDeathCheckPosition.position.x + (raySpacing * i);
            Vector2 rayOrigin = new Vector2(xPosition, leftDeathCheckPosition.position.y);

            if (CheckIfDead())
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawRay(rayOrigin, Vector2.down * rayDistance);
        }
    }

    #endregion

}
