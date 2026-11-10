using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.Player
{
    public class FlashlightController : MonoBehaviour
    {
        private List<Light2D> _lights = new List<Light2D>();
        private PlayerController _playerController;

        [Header("Rotation")]
        [SerializeField] private float turnSpeed;

        [Header("Detection")]
        [SerializeField] private float range;
        [SerializeField] private float coneAngle;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private float ambientRange;

        private void Start()
        {
            _lights = GetComponentsInChildren<Light2D>().ToList();
            _playerController = GetComponentInParent<PlayerController>();
        }

        private void LateUpdate()
        {
            switch (_playerController.LastPressedDirection)
            {
                case PlayerController.Direction.Up:
                    transform.rotation = Quaternion.Lerp(
                        Quaternion.Euler(0, 0, 0),
                        transform.rotation,
                        turnSpeed);
                    break;

                case PlayerController.Direction.Down:
                    transform.rotation = Quaternion.Lerp(
                        Quaternion.Euler(0, 0, 180),
                        transform.rotation,
                        turnSpeed);
                    break;

                case PlayerController.Direction.Left:
                    transform.rotation = Quaternion.Lerp(
                        Quaternion.Euler(0, 0, 90),
                        transform.rotation,
                        turnSpeed);
                    break;

                case PlayerController.Direction.Right:
                    transform.rotation = Quaternion.Lerp(
                        Quaternion.Euler(0, 0, 270),
                        transform.rotation,
                        turnSpeed);
                    break;
            }
        }

        public bool IsPointLOS(Vector2 origin, Vector2 direction, float distance)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                direction.normalized,
                distance,
                obstacleMask
            );

            return hit.collider == null;
        }

        public bool IsPointLOS(Vector2 target)
        {
            Vector2 direction = target - (Vector2)transform.position;

            return IsPointLOS(
                transform.position,
                direction,
                direction.magnitude
            );
        }
        
        public bool IsPointIlluminated(Vector2 point)
        {
            Vector2 origin = transform.position;
            Vector2 direction = point - origin;

            float distance = direction.magnitude;
            if (distance > range)
                return false;
            

            float angle = Vector2.Angle(transform.up, direction);

            if (angle > coneAngle * 0.5f && distance > ambientRange)
                return false;

            return IsPointLOS(origin, direction, distance);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                foreach (var light in _lights)
                {
                    light.enabled = false;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                foreach (var light in _lights)
                {
                    light.enabled = true;
                }
            }
        }
    }
}
