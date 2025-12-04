using System.Collections;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [Header("Animal Data")]
    [SerializeField] bool isLastItem;

    [Header("Animal Runtime Data")]
    public int id;
    public Dropper dropper;
    public bool isTouchingTopBar = false;
    public GameObject eyesClose;

    public bool isCombining;

    private GameManager game;

    // gets game manager
    void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();

        eyesClose = transform.GetChild(0).gameObject;
        eyesClose.SetActive(false);

        dropper = GameManager.instance.dropper;
    }

    private void OnEnable()
    {
        isCombining = false;
    }

    void OnTriggerExit2D(Collider2D collision) {
        isTouchingTopBar = false;
    }

    // checks if fruit is touching another fruit
    void OnCollisionEnter2D(Collision2D collision)
    {
        // gets fruit component on the other fruit
        Animal otherAnimal = collision.gameObject.GetComponent<Animal>();
        // checks if the other fruit exists and if the id is the same, and checks if the instance id is different (to prevent double combining)
        if (otherAnimal != null && otherAnimal.id == this.id && this.gameObject.GetInstanceID() < collision.gameObject.GetInstanceID() && !isCombining)
        {
            isCombining = true;
            game.ReturnObject(this.gameObject);
            game.ReturnObject(collision.gameObject);

            ParticleSystem combineParticle = game.GetParticle(transform.position, Vector3.zero);
            combineParticle.Play();
            game.ReturnObject(combineParticle.gameObject, combineParticle.main.duration);

            AudioSource combineAudioSource = game.GetAudioSource(transform.position, Vector3.zero);
            combineAudioSource.Play();
            game.ReturnObject(combineAudioSource.gameObject, 0.8f);

            if (!isLastItem)
            {
                GameObject animal = game.GetAnimal(this.transform.position, Vector3.zero, this.id + 1);
                game.AddAnimalToList(animal);
            }
            game.addScore(this.id);
        }
        if (collision.gameObject.CompareTag("OffCollider"))
        {
            game.ReturnObject(gameObject);
        }
    }
}
