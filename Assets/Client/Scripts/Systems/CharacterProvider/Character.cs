//-----------------------------------------------------------------
// File:         Character.cs
// Description:  Wire Character data
// Module:       Characters
// Author:       Noé Masse
// Date:         16/04/2021
//-----------------------------------------------------------------
using UnityEngine;

namespace MonsterWorld.Unity.Systems.Characters
{
    public class Character : MonoBehaviour
    {
        private SpriteRenderer _SpriteRenderer;

        public Sprite Sprite
        {
            get { return _SpriteRenderer.sprite; }
            set { _SpriteRenderer.sprite = value; }
        }

        void Start()
        {
            _SpriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
            if (_SpriteRenderer == null)
            {
                Debug.LogWarning("Couldn't find Sprite Renderer component for the character : " + gameObject.name);
            }
        }
    }
}