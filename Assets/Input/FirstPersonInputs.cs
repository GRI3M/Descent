using UnityEngine;
using UnityEngine.InputSystem;

namespace Descent
{
    public class FirstPersonInputs : MonoBehaviour
    {
        [Header("Player Input Values")]
        public Vector2 movement;
        public Vector2 characterRotation;
        public bool jump;
        public bool sprint;
        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool enabledInputForLook = true;
        

        public void OnMove(InputValue value)
        {
            movement = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            if(enabledInputForLook)
                characterRotation = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }

        public void OnSprint(InputValue value)
        {
            sprint = value.isPressed;
        }

        private void OnAplicationFocus(bool focus)
        {
            SetCursosrState(cursorLocked);
        }

        private void SetCursosrState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
