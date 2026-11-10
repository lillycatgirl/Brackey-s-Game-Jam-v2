using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class FadeAbleImage : MonoBehaviour, IFadeAble
    {
        Image _image;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetAlpha(float alpha)
        {
            if (_image != null)
            {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
            }
        }
    }
}