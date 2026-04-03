using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

//Going to split this script into 2 scripts, one for the input system
//and the other for the player controller (Not using the character controller class)

/*
Looking through documentation and studying the different types of movement systems,
this game will require a customized input scheme, as there will be cliffs that the 
player will have to switch input systems to to climb, as well as tunnels that the player
will crouch into and crawl through. The built in character controller will not be enough.

List of character movements to implement:
- looking around (mouse/R joystick movement)
- walking (WASD/L joystick movement)
- crouching (L Crtl/ B button)
- crawling (Mixture of WASD & mouse movement/ R & L joystick movement)
- climbing (Mixture of WASD & mouse movement/ R & L joystick movement)
- jumping (Spacebar/A button)

Mechanics to implement:
- stamina
- player fear recognition (Having the game detect when the character is starting to panic
  and having the character breath heavier, and movement more frantic)
- Health Mechanic (The player will get scraped and banged up from crawling around through
tight tunnels, so it would make sense to have be bandaged every once and a while)
- Hunger and Thirst mechanic (I think these should go down slowly, but they will have to 
be prioritizd so that the player is forced to return to supply areas)

*/

public class PlayerController : MonoBehaviour
{

    [Header("Player Objects")]
    public Rigidbody rb;
    public GameObject camHolder;
    [Header("Player Settings")]
    public float speed;
    public float sensitivity;
    public float maxForce;
    public float jumpForce;
    private Vector2 move, look;
    private bool grounded = true;


    private float lookRotation;
    
   //This will get reenabled when these scripts get split out into two codes,
   //that way the input system can be checked for mouse and keyboard or controller.
    /*private bool IsCurrentDeviceMouse
    {
        get
        {
            #if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
            #else
            return false;
            #endif
        }
    }*/

    GameObject _mainCamera;
    void Awake()
    {
    
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ProcessLook();

    }

    private void ProcessLook()
    {
        //Turn Model
        transform.Rotate(Vector3.up * look.x * sensitivity);

        //Turn Camera
        lookRotation += (-look.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90f, 90f);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        Jump();
    }


    private void FixedUpdate()
    {
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        //Find target velocity
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y) * speed;

        //Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Calculate forces
        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        //Limit forces
        Vector3.ClampMagnitude(velocityChange, maxForce);

        //Apply forces
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }


    private void Jump()
    {
        Vector3 jumpForces = Vector3.zero;
        if (grounded)
        {
            jumpForces = Vector3.up * jumpForce;
            rb.AddForce(jumpForces, ForceMode.VelocityChange);
        }
    }
} 

