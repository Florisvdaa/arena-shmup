using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXV.ShieldDemo
{
    public class fxvFPSController : MonoBehaviour
    {
        public Camera playerCamera;

        float lookSpeed = 2.0f;
        float lookXLimit = 45.0f;
        float walkingSpeed = 7.5f;
        float runningSpeed = 11.5f;

       // [SerializeField]
       // float gravity = 20.0f;

        CharacterController characterController;
        Vector3 moveDirection = Vector3.zero;
        float rotationX = 0;

        [HideInInspector]
        bool canMove = true;

        void Start()
        {
            characterController = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetKey(KeyCode.E) && canMove)
            {
                moveDirection.y = walkingSpeed;
            }
            else if (Input.GetKey(KeyCode.Q) && canMove)
            {
                moveDirection.y = -walkingSpeed;
            }
            else
            {
                moveDirection.y = 0.0f;
            }

           /* if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }*/

            characterController.Move(moveDirection * Time.deltaTime);

            if (canMove)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }
    }
}
