using System;
using Gameplay.Game;
using Gameplay.Player;
using UI.Dialogue;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking.PlayerConnection;

namespace Gameplay.NPC
{
    public class NpcController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        
        [SerializeField] private GameObject player;
        [SerializeField] private DialogueLoader.Character npcType;
        public PlayerController.Direction LastPressedDirection;
        public bool moving;
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        private void Update()
        {
            if ((StoryStateLoader.Instance.storyState.oldManDead && npcType == DialogueLoader.Character.OldMan) ||
                (StoryStateLoader.Instance.storyState.girlDead && npcType == DialogueLoader.Character.Girl))
            {
                if (_agent != null)
                {
                    Destroy(_agent);
                    _agent = null;
                }
            }
            
            if (!StoryStateLoader.Instance.storyState.alone && _agent != null)
            {
                _agent.SetDestination(player.transform.position);
                UpdateMovementDirection();
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                if ((transform.position - player.transform.position).magnitude < 1f)
                    StoryStateLoader.Instance.OnTalkToNPC();
            }
        }
        
        private void UpdateMovementDirection()
        {
            Vector3 velocity = _agent.velocity;
            
            moving = velocity.magnitude > 0.1f;

            if (velocity.sqrMagnitude < 0.01f)
            {
                moving = false;
                return;
            }

            moving = true;

            if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
            {
                LastPressedDirection = velocity.x > 0
                    ? PlayerController.Direction.Right
                    : PlayerController.Direction.Left;
            }
            else
            {
                LastPressedDirection = velocity.y > 0
                    ? PlayerController.Direction.Up
                    : PlayerController.Direction.Down;
            }
        }
    }
}