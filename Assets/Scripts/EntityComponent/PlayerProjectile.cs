using System;
using System.Collections;
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
        [SerializeField] private SpriteRenderer renderer;

        public void Spawn(Quaternion rotation, Vector2 target)
        {
            gameObject.SetActive(true);
            transform.rotation = rotation;
            StartCoroutine(Moving(target));
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

            var currentAlphaCutoff = lightMask.alphaCutoff;
            while (lightMask.alphaCutoff < 1)
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                currentAlphaCutoff += Time.deltaTime * 2f;
                lightMask.alphaCutoff = currentAlphaCutoff;
                
                yield return null;
            }
            
            DestroySelf();
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
