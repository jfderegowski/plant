using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class CubeMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private InputAction movementAction;

        private void Start() => movementAction.Enable();

        private void OnDestroy() => movementAction.Disable();

        private void Update()
        {
            // 2D movement Up, Down, Left, Right
            var movement = movementAction.ReadValue<Vector2>();
            var movementVector = new Vector3(movement.x, movement.y, 0);
            transform.position += movementVector * (speed * Time.deltaTime);
        }
    }
}
