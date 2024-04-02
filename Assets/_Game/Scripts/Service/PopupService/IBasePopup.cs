using System;

namespace MyProject.Core.Services
{
    public interface IBasePopup
    {
        void Init(BasePopupParameters parameters);
        void Show();
        void ClosePopup();
        void DestroyPopup(Action destroyCallback = null);
    }
}

