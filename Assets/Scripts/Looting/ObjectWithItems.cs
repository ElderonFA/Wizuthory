using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Looting
{
    public interface IObjectWithItems
    {
        IEnumerator AnimDestroy();
        void DropItems();
    }
}