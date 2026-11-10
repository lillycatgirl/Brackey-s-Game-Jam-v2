using System;
using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;

namespace FX
{
    [System.Serializable]
    public struct Animation
    {
        public PlayerController.Direction direction;
        public Sprite[] sprites;
        public bool flip;
    }
    
    public class PlayerSpriteController : MonoBehaviour
    {
        private PlayerController _playerController;
        private SpriteRenderer _rend;
        [SerializeField] public List<Animation> walkAnimations;
        [SerializeField] public List<Animation> swimAnimations;

        private int _animationProgress = 0;
        private float _animationTime = 0;
        [SerializeField] private float animationSpeed;
        
        private void Awake()
        {
            _rend = GetComponent<SpriteRenderer>();
            _playerController = GetComponentInParent<PlayerController>();
        }

        private void LateUpdate()
        {
            if (Time.timeScale == 0)
                return;
            
            _animationTime -= Time.deltaTime;
            if (_animationTime <= 0 && _playerController.moving)
            {
                _animationTime = animationSpeed;
                _animationProgress += 1;
            }

            if (_playerController.moving)
            {
                if (_playerController.inWater)
                {
                    _rend.sprite = swimAnimations[(int)_playerController.LastPressedDirection].sprites[_animationProgress % swimAnimations.Count];
                    _rend.flipX = swimAnimations[(int)_playerController.LastPressedDirection].flip;
                }
                else
                {
                    _rend.sprite = walkAnimations[(int)_playerController.LastPressedDirection].sprites[_animationProgress % walkAnimations.Count];
                    _rend.flipX = walkAnimations[(int)_playerController.LastPressedDirection].flip;
                }
            }
            else
            {
                if (_playerController.inWater)
                {
                    _rend.sprite = swimAnimations[(int)_playerController.LastPressedDirection].sprites[0];
                    _rend.flipX = swimAnimations[(int)_playerController.LastPressedDirection].flip;
                }
                else
                {
                    _rend.sprite = walkAnimations[(int)_playerController.LastPressedDirection].sprites[0];
                    _rend.flipX = walkAnimations[(int)_playerController.LastPressedDirection].flip;
                }
            }
        }
    }
}