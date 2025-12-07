using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dropper : MonoBehaviour
{
    [HideInInspector] [SerializeField] GameManager gameManager;
    [HideInInspector] [SerializeField] InputManager inputManager;

    [Header("Runtime Animal")]
    [SerializeField] GameObject currentAnimal;

    [Header("Dropper Variables")]
    [SerializeField] private float dropWait = 0.5f;
    [SerializeField] Transform rightEnd;
    [SerializeField] Transform leftEnd;
    [SerializeField] float dropperHeight;
    [SerializeField] Transform cheatAnimalDropPosition;

    [Header("UI Variables")]
    [SerializeField] Image secondAnimalImage;
    [SerializeField] Image thirdAnimalImage;
    [SerializeField] Image cheatAnimalSprite;

    private bool canDrop = true;

    private int firstRandomInt = 0;
    private int secondRandomInt = 1;
    private int thirdRandomInt = 2;

    // gets game manager
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        InitializeFirstAnimals();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDropperMovement();

        if(!canDrop && inputManager.drop)
        {
            inputManager.UseDropInput();
        }

        if (inputManager.drop && currentAnimal != null && canDrop)
        {
            canDrop = false;
            gameManager.AddAnimalToList();
            inputManager.UseDropInput();
            currentAnimal.GetComponent<Rigidbody2D>().gravityScale = 1;
            currentAnimal.GetComponent<Animal>().isCombining = false;
            StartCoroutine(WaitForDrop());
        }
    }

    private void InitializeFirstAnimals()
    {
        currentAnimal = gameManager.GetAnimal(this.transform.position, Vector3.zero, firstRandomInt);
        currentAnimal.GetComponent<Rigidbody2D>().gravityScale = 0;
        currentAnimal.GetComponent<Animal>().isCombining = true;
        secondAnimalImage.sprite = gameManager.animalPrefabs[secondRandomInt].GetComponent<SpriteRenderer>().sprite;
    }

    private IEnumerator WaitForDrop()
    {
        yield return new WaitForSeconds(dropWait);
        SpawnNewFruit();
        yield return new WaitForSeconds(0.001f);
        canDrop = true;
    }

    public void SpawnNewFruit()
    {
        firstRandomInt = secondRandomInt;
        secondRandomInt = thirdRandomInt;

        int randomFruit = Random.Range(0, 5);
        thirdRandomInt = randomFruit;

        UpdateAnimalDropSprites();

        currentAnimal = gameManager.GetAnimal(this.transform.position, Vector3.zero, firstRandomInt);
        currentAnimal.GetComponent<Rigidbody2D>().gravityScale = 0;
        currentAnimal.GetComponent<Animal>().isCombining = true;
    }   

    public void HandleDropperMovement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(inputManager.movePosition);
        mousePos.y = dropperHeight;

        if (!gameManager.gameOver && mousePos.x < leftEnd.position.x && mousePos.x > rightEnd.position.x)
        {
            transform.position = mousePos;
        }

        if (currentAnimal != null && currentAnimal.GetComponent<Rigidbody2D>().gravityScale == 0)
        {
            currentAnimal.transform.position = this.transform.position;
        }
    }

    private void UpdateAnimalDropSprites()
    {
        secondAnimalImage.sprite = gameManager.animalPrefabs[secondRandomInt].GetComponent<SpriteRenderer>().sprite;
        //thirdAnimalImage.sprite = gameManager.animalPrefabs[thirdRandomInt].GetComponent<SpriteRenderer>().sprite;
    }

    public void UpdateCheatAnimalSprite(int index)
    {
        cheatAnimalSprite.sprite = gameManager.animalPrefabs[index].GetComponent<Animal>().GetComponent<SpriteRenderer>().sprite;
    }

    public void DropCheatAnimal(int index)
    {
        GameObject newAnimal = gameManager.GetAnimal(cheatAnimalDropPosition.position, Vector3.zero, index);
        newAnimal.GetComponent<Rigidbody2D>().gravityScale = 1;
        newAnimal.GetComponent<Animal>().isCombining = false;
        gameManager.AddAnimalToList(newAnimal);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 leftPosition = rightEnd.position;
        Vector3 rightPosition = leftEnd.position;
        leftPosition.y = dropperHeight;
        rightPosition.y = dropperHeight;
        Gizmos.DrawLine(leftPosition, rightPosition);
    }

}
