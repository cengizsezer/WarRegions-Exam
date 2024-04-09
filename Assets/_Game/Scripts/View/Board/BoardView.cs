using System.Collections.Generic;
using Zenject;

namespace MyProject.GamePlay.Views
{
    public class BoardView : BaseView
    {
        public List<ColorRegion> BoardRegion = new();
        public List<GridView> lsAllGridView = new();

        #region Injection


        [Inject]
        private void Construct()
        {

        }

        #endregion

        public override void Initialize()
        {
            _signalBus.Subscribe<LevelFailSignal>(Disable);
            _signalBus.Subscribe<LevelSuccessSignal>(Disable);
        }

        public void Init()
        {
            gameObject.SetActive(true);

        }

        public void Disable()
        {
            BoardRegion.Clear();
            lsAllGridView.Clear();
        }

        public override void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelFailSignal>(Disable);
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(Disable);
        }
    }
}

