using System;
using System.Collections.Generic;
using Gameplay.NPC;
using Gameplay.Player;
using UnityEngine;

namespace FX
{
    [Serializable]
    public struct NPCAnimation
    {
        public PlayerController.Direction direction;
        public Sprite[] sprites;
        public bool flip;
    }
    
    public class NPCSpriteController : MonoBehaviour
    {
        private NpcController _npcController;
        private SpriteRenderer _rend;
        [SerializeField] public List<Animation> walkAnimations;

        private int _animationProgress = 0;
        private float _animationTime = 0;
        [SerializeField] private float animationSpeed;
        
        private void Awake()
        {
            _rend = GetComponent<SpriteRenderer>();
            _npcController = GetComponentInParent<NpcController>();
        }

        private void LateUpdate()
        {
            if (Time.timeScale == 0)
                return;
            
            _animationTime -= Time.deltaTime;
            if (_animationTime <= 0 && _npcController.moving)
            {
                _animationTime = animationSpeed;
                _animationProgress += 1;
            }

            if (_npcController.moving)
            {
                _rend.sprite = walkAnimations[(int)_npcController.LastPressedDirection].sprites[_animationProgress % walkAnimations.Count];
                _rend.flipX = walkAnimations[(int)_npcController.LastPressedDirection].flip;
                
            }
            else
            {
                _rend.sprite = walkAnimations[(int)_npcController.LastPressedDirection].sprites[0];
                _rend.flipX = walkAnimations[(int)_npcController.LastPressedDirection].flip;
            }
        }
    }
}