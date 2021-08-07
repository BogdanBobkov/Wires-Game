using Internals;
using System;

namespace UI
{
    public interface IWidget : IRegistrable
    {
        void Show(Action afterShow = null);
        void Hide(Action afterHide = null);
    }
}
