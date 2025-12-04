using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Dropper dropper;
    private PlayerInput playerInput;

    #region Public Get Variables

    public Vector2 movePosition { get; private set; }
    public bool drop { get; private set; }
    public bool cheatDrop { get; private set; }

    [SerializeField] int cheatDropInt;

    #endregion

    #region Unity Callback Functions

    private void Start()
    {
        dropper = GetComponent<Dropper>();
        playerInput = GetComponent<PlayerInput>();
        dropper.UpdateCheatAnimalSprite(cheatDropInt);
    }

    #endregion

    #region Input Actions

    public void OnMovePositionInput(InputAction.CallbackContext context)
    {
        movePosition = context.ReadValue<Vector2>();
    }

    public void OnDropInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            drop = true;
        }
    }

    public void OnCheatDropInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dropper.DropCheatAnimal(cheatDropInt);
        }
    }

    public void OnAnimalCheatCycleForwardInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if(cheatDropInt < 10)
            {
                cheatDropInt += 1;
            }
            else
            {
                cheatDropInt = 0;
            }

            dropper.UpdateCheatAnimalSprite(cheatDropInt);
        }
    }

    public void OnAnimalCheatCycleBackwardInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (cheatDropInt > 0)
            {
                cheatDropInt -= 1;
            }
            else
            {
                cheatDropInt = 10;
            }

            dropper.UpdateCheatAnimalSprite(cheatDropInt);
        }
    }

    #endregion

    #region Set Functions

    public void SetPlayerInput(bool enable)
    {
        playerInput.enabled = enable;
    }

    #endregion

    #region Use Actions

    public void UseDropInput() => drop = false;
    public void UseCheatDropInput() => cheatDrop = false;

    #endregion

}
