using System;
using Gameplay.Game;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Pickups
{
    enum PickupType
    {
        Food,
        Tile
    }
    
    public class PickupController : MonoBehaviour
    {
        [SerializeField] private float pullDistance;
        [SerializeField] private float pickupDistance;
        [SerializeField] private float pullSpeed;
        [SerializeField] private PickupType pickupType;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var distance = (PlayerController.Instance.transform.position - transform.position);
            if (distance.magnitude < pullDistance)
            {
                _rb.linearVelocity +=  (Vector2)distance.normalized * pullSpeed;
            }

            if (distance.magnitude < pickupDistance)
            {
                switch (pickupType)
                {
                    case PickupType.Food:
                        StoryStateLoader.Instance.OnCollectFood();
                        Destroy(gameObject);
                        break;
                    case PickupType.Tile:
                        StoryStateLoader.Instance.OnCollectTile();
                        Destroy(gameObject);
                        break;
                }
            }
        }
    }
}