using System.Collections;
using UnityEngine;

namespace EntityComponent
{
    interface IProjectile
    {
        void Spawn(Quaternion rotation, Vector2 target);
        
        IEnumerator Moving(Vector2 target);
        
        void DestroySelf();
    }
}
