using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internals
{
    public interface IRegistrable
    {
        void Register();
        void Unregister();
    }
}
