using System;
using UnityEngine;

namespace Gameplay.Game
{
    public class Exit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            StoryStateLoader.Instance.OnExitFound();
            print("Exit found!!!!!!!!!");
        }
    }
}