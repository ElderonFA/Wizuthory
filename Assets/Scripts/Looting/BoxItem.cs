using System;
using System.Collections;
using UnityEngine;

namespace Looting
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BoxItem : MonoBehaviour, IObjectWithItems
    {
        [SerializeField] 
        private AudioClip sound;

        [Space] 
        [SerializeField] 
        private GameObject currentItem;
        [SerializeField] 
        private int itemsCount;

        private SpriteRenderer spriteRenderer;
        private AudioSource audioSource;

        private float minusEf = 2f;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = sound;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "DamageAttack")
            {
                StartCoroutine(AnimDestroy());
            }
        }

        public IEnumerator AnimDestroy()
        {
            audioSource.Play();

            while (spriteRenderer.color.r > 0)
            {
                var currentColor = spriteRenderer.color;
                
                var newColor = new Color(
                    currentColor.r - Time.deltaTime * minusEf,
                    currentColor.g - Time.deltaTime * minusEf,
                    currentColor.b - Time.deltaTime * minusEf);
                spriteRenderer.color = newColor;
                
                yield return null;
            }
            
            DropItems();
            Destroy(gameObject);
        }

        public void DropItems()
        {
            for (var i = 0; i < itemsCount; i++)
            {
                Instantiate(currentItem, transform.position, Quaternion.identity);
            }
        }
    }
}
