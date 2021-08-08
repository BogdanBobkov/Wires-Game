using UnityEngine;
using System;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WidgetBase : MonoBehaviour, IWidget
    {
        [SerializeField] private float _showTime = 1f;
        [SerializeField] private float _hideTime = 1f;

        public virtual void Hide(Action afterHide = null)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            this.InvokeDelegate((value) => { canvasGroup.alpha = 1f - value; }, _hideTime, () => { afterHide?.Invoke(); });
        }
        public virtual void Show(Action afterShow = null)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            this.InvokeDelegate((value) => { canvasGroup.alpha = value; }, _hideTime, () => { canvasGroup.blocksRaycasts = true; afterShow?.Invoke(); });
        }

        public abstract void Register();
        public abstract void Unregister();
    }
}