using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class UiCanvas : MonoBehaviour
    {
        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);
    }
}
