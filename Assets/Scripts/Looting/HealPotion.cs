using UnityEngine;

namespace Looting
{
    public class HealPotion : MonoBehaviour, IItem
    {
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField] 
        private GameObject mainObj;

        public bool CanTake { get; set; }

        void Start()
        {
            DropItem();
        }

        public void CollectItem()
        {
            Destroy(mainObj);
        }

        public void UseItem()
        {
            
        }

        public void DropItem()
        {
            var xForce = Random.Range(-25f, 25f);
            var yForce = Random.Range(90f, 125f);
            rb.AddForce(new Vector2(xForce, yForce));
        }
    }
}
