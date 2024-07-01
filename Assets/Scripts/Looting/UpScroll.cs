using UnityEngine;

namespace Looting
{
    public class UpScroll : MonoBehaviour, IItem
    {
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField] 
        private GameObject mainObj;
        [Space]
        [SerializeField] 
        private PlayerSkills currentSkill;

        void Start()
        {
            DropItem();
        }

        public void CollectItem()
        {
            UpPlayerSkill();
        }

        public void UseItem()
        {
            
        }

        public void DropItem()
        {
            var xForce = Random.Range(-25f, 25f);
            var yForce = Random.Range(-25f, 25f);
            rb.AddForce(new Vector2(xForce, yForce));
        }

        public void UpPlayerSkill()
        {
            var player = FindObjectOfType(typeof(PlayerController)) as PlayerController;
            if (player)
            {
                player.AddNewSkill(currentSkill);
            }
            
            Destroy(mainObj);
        }
    }

    public enum PlayerSkills
    {
        Dodge = 0,
        DistanceAttack = 1,
        RedSparks = 2,
    }
}
