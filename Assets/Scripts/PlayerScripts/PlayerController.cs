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
namespace Descent
{
    public class PlayerController : MonoBehaviour
    {

        [Header("Player Objects")]
        public Rigidbody rb;
        
        [Header("Player Settings")]
        public float speed;
        public float sensitivity;
        public float maxForce;
        public float jumpForce;
        private bool grounded = true;

        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;
        public bool InvertedCamera = false;
        private float _lookRotation;

        private PlayerInput _playerInput;
        private FirstPersonInputs _input;

        private GameObject _camHolder;
        
        
    //This will get reenabled when these scripts get split out into two codes,
    //that way the input system can be checked for mouse and keyboard or controller.
        private bool IsCurrentDeviceMouse
        {
            get
            {
                return _playerInput.currentControlScheme == "KeyboardMouse";
            }
        }

        void Start()
        {
            _playerInput =  GetComponent<PlayerInput>();
            _input = GetComponent<FirstPersonInputs>();
        }

        void Awake()
        {
            if(_camHolder == null) _camHolder = GameObject.FindGameObjectWithTag("MainCamera");
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        void Update()
        {
            Jump();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            ProcessLook();
        }

        private void ProcessLook()
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            HandlePlayerRotation(deltaTimeMultiplier);
            HandleCameraRotations(deltaTimeMultiplier);
        }

        private void HandlePlayerRotation(float dt)
        {
            //Gather mouse inputs and apply sensitivity then rotate player model
            float xRotation = _input.characterRotation.x * sensitivity * dt;
            transform.Rotate(Vector3.up * xRotation);
        }

        private void HandleCameraRotations(float dt)
        {
            //Gather mouse inputs and apply sensitivity and invert to normal

            float invert = InvertedCamera ? 1f : -1f;
            _lookRotation += (_input.characterRotation.y * sensitivity * invert * dt);
            //Stop the camera rotations from going over or below 360 degrees
            if (_lookRotation > 360f) _lookRotation -= 360f;
            if (_lookRotation < -360f) _lookRotation += 360f;
            //Clamp camera rotation to prevent doing a backflip with camera
            _lookRotation = Mathf.Clamp(_lookRotation, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_lookRotation, 0.0f, 0.0f);
        }

        private void FixedUpdate()
        {
            ProcessMovement();
        }

        private void ProcessMovement()
        {
            //Find target velocity
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 targetVelocity = new Vector3(_input.movement.x, 0, _input.movement.y) * speed;

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

            if(_input.jump)
            {
                if (grounded)
                {
                    jumpForces = Vector3.up * jumpForce;
                    rb.AddForce(jumpForces, ForceMode.VelocityChange);
                    _input.jump = false;
                }
            }
        }
    } 
}
