using System;using UnityEngine;

namespace Looting
{
    public interface IItem
    {
        public void CollectItem();

        public void UseItem();

        public void DropItem();
    }
}

public enum ItemsTypes
{
    None = 0,
    HealPotion = 1,
}
