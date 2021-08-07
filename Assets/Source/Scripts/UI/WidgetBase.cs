using UnityEngine;
using System;

namespace UI
{
    public abstract class WidgetBase : MonoBehaviour, IWidget
    {
        public virtual void Hide(Action afterHide = null)
        {
            gameObject.SetActive(false);
            afterHide?.Invoke();
        }
        public virtual void Show(Action afterShow = null)
        {
            gameObject.SetActive(true);
            afterShow?.Invoke();
        }

        public abstract void Register();
        public abstract void Unregister();
    }
}