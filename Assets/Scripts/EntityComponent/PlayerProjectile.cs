using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityComponent
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class PlayerProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField] private float damage;
        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [Space] 
        [SerializeField] private SpriteMask lightMask;
        [SerializeField] private ImagesAnim imgAnim;
        [Space] 
        [SerializeField] private List<Sprite> despawnAnim;

        private Coroutine movingCoroutine;

        public void Spawn(Quaternion rotation, Vector2 target)
        {
            gameObject.SetActive(true);
            transform.rotation = rotation;
            movingCoroutine = StartCoroutine(Moving(target));
        }

        public IEnumerator Moving(Vector2 target)
        {
            var currentLifeTime = lifeTime;

            while (currentLifeTime > 0)
            {
                //transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed);
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                currentLifeTime -= Time.deltaTime;
                
                yield return null;
            }

            StartCoroutine(DestroySelf());
        }

        public IEnumerator DestroySelf()
        {
            StartCoroutine(imgAnim.PlayOneShot(despawnAnim));
                
            var currentAlphaCutoff = lightMask.alphaCutoff;

            if (currentAlphaCutoff <= 1f)
            {
                while (lightMask.alphaCutoff < 1f)
                {
                    currentAlphaCutoff += Time.deltaTime * 2f;
                    lightMask.alphaCutoff = currentAlphaCutoff;

                    yield return null;
                }
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            StopCoroutine(movingCoroutine);
            StartCoroutine(DestroySelf());
        }
    }
}
