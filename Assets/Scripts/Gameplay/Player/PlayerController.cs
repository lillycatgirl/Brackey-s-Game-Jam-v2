using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject flashlight;
        [SerializeField] private Transform waterRaycastPoint;
        [SerializeField] private LayerMask waterLayer;
        [SerializeField] private float speed = 5f;
        [SerializeField] private float friction = 0.9f;

        private Rigidbody2D _rb;
        private Camera _camera;
        
        public static PlayerController Instance;
        
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        
        public Direction CurrentDirection { get; private set; } = Direction.Up;
        public Direction LastPressedDirection { get; private set; } = Direction.Up;
        
        private List<Direction> _pressedDirections = new List<Direction>();
        public bool moving = false;
        public bool inWater = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            _rb = GetComponent<Rigidbody2D>();
            _camera = Camera.main;
        }

        private void Update()
        {
            var rc = Physics2D.Raycast(waterRaycastPoint.position, Vector2.down, 0.01f, waterLayer);
            if (rc.collider != null)
            {
                inWater = true;
            }
            else
            {
                inWater = false;
            }
            
            
            if (Input.GetKeyDown(KeyCode.W))
            {
                _pressedDirections.Add(Direction.Up);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _pressedDirections.Add(Direction.Left);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _pressedDirections.Add(Direction.Down);
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                _pressedDirections.Add(Direction.Right);
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                _pressedDirections.Remove(Direction.Up);
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                _pressedDirections.Remove(Direction.Left);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                _pressedDirections.Remove(Direction.Down);
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                _pressedDirections.Remove(Direction.Right);
            }
            if (_pressedDirections.Count > 0)
                LastPressedDirection = _pressedDirections[^1];
        }

        private void FixedUpdate()
        {
            Vector2 input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
            
            var screenMousePos = Input.mousePosition;
            Vector2 worldMousePos = _camera.ScreenToWorldPoint(screenMousePos);

            input = input.normalized;
            if (!input.Equals(Vector2.zero))
            {
                moving = true;
                if (inWater)
                {
                    _rb.linearVelocity = input * (speed * 0.75f);
                }
                else
                {
                    _rb.linearVelocity = input * (speed);
                }
                
            }
            else
            {
                moving = false;
                _rb.linearVelocity *= friction;
            }
            
            if (input.x != 0 && input.y != 0)
            {
                input.y = 0;
            }
            input = input.normalized;

            if (!input.Equals(Vector2.zero))
            {
                if (input.x != 0)
                {
                    CurrentDirection = input.x > 0 ? Direction.Right : Direction.Left;
                }
                else
                {
                    CurrentDirection = input.y > 0 ? Direction.Up : Direction.Down;
                }
            }
        }
    }
}