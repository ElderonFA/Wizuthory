using System.Collections;
using System.Collections.Generic;
using EntityComponent;
using UnityEngine;

public class NpcProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [Space] 
    [SerializeField] private SpriteMask lightMask;
    [SerializeField] private ImagesAnim imgAnim;
    [Space] 
    [SerializeField] private List<Sprite> despawnAnim;
    
    public void Spawn(Quaternion rotation, Vector2 target)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Moving(Vector2 target)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator DestroySelf()
    {
        throw new System.NotImplementedException();
    }
}
