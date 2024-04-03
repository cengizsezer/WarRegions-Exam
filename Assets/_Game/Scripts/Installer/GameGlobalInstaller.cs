using MyProject.Core.Controllers;
using MyProject.Core.Models;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using MyProject.Core.Const;
using MyProject.GamePlay.Controllers;
using MyProject.GamePlay.Characters;
using UnityEngine;
using Zenject;


namespace MyProject.Core.Installer
{
    public class GameGlobalInstaller : MonoInstaller
    {
        #region Injection

        private ApplicationPrefabSettings _applicationPrefabSettings;
        private CurrencySettings _currencySettings;
        private CharacterItemSettings _characterItemsettings;

        [Inject]
        private void Construct
        (
              ApplicationPrefabSettings applicationPrefabSetting
            , CurrencySettings currencySettings
            , CharacterItemSettings characterItemsettings
        )
        {
            _applicationPrefabSettings = applicationPrefabSetting;
            _currencySettings = currencySettings;
            _characterItemsettings = characterItemsettings;
        }

        #endregion

        public override void InstallBindings()
        {

            //Signal
            GameSignalInstaller.Install(Container);

            ////Helper
            Container.BindInterfacesTo<GameLogger>().AsSingle();

            ////Model
            Container.BindInterfacesAndSelfTo<CurrencyModel>().AsSingle();
            Container.Bind<UserModel>().AsSingle();
           
            ////Service
            Container.BindInterfacesAndSelfTo<TaskService>().AsSingle();
            Container.Bind<CoroutineService>().AsSingle();
            Container.Bind<FlagService>().AsSingle();
            Container.BindInterfacesAndSelfTo<TimerService>().AsSingle();
            Container.BindExecutionOrder<PopupService>(-10000);
            Container.Bind<PopupService>().AsSingle();

            ////Controller
            Container.BindInterfacesAndSelfTo<CurrencyController>().AsSingle();
            Container.Bind<ScreenController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ApplicationController>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
            Container.BindInterfacesAndSelfTo<UserController>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerMobController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardFXController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardCoordinateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardGamePlayController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardDataController>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemySpawnController>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyMobController>().AsSingle();
            Container.BindInterfacesAndSelfTo<WeopanVFXController>().AsSingle();
            
            ////Factory
            Container.BindFactory<Object, BaseScreen, BaseScreen.Factory>().FromFactory<PrefabFactory<BaseScreen>>();
            Container.BindFactory<Object, PlayerBossView, PlayerBossView.Factory>().FromFactory<PrefabFactory<PlayerBossView>>();
            Container.BindFactory<Object, EnemyBossView, EnemyBossView.Factory>().FromFactory<PrefabFactory<EnemyBossView>>();

            InstallCurrencyVfx();
            InstallGridViews();
            InstallPlayerMobView();
            InstallBoardVFX();
            InstallDamageTextFeedbacksView();
            InstallEnemyMobView();
            InstallBombWeopanVFXView();
            InstallArrowWeopanVFXView();
            InstallAxeWeopanVFXView();
        }


        private void InstallGridViews()
        {
            Container.BindFactory<GridView.Args, GridView, GridView.Factory>()
               .FromPoolableMemoryPool<GridView.Args, GridView, GridView.Pool>(poolbinder => poolbinder
                   .WithInitialSize(GlobalConsts.BoardConsts.ROWS* GlobalConsts.BoardConsts.COLUMNS)
                   .FromComponentInNewPrefab(_applicationPrefabSettings.GridViewPrefab)
                   .UnderTransformGroup("GridViews"));
        }

        private void InstallCurrencyVfx()
        {
            Container.BindFactory<CurrencyVFXView.Args, CurrencyVFXView, CurrencyVFXView.Factory>()
                .FromPoolableMemoryPool<CurrencyVFXView.Args, CurrencyVFXView, CurrencyVFXView.Pool>(poolbinder => poolbinder
                    .WithInitialSize(5)
                    .FromComponentInNewPrefab(_currencySettings.CurrencyAnim)
                    .UnderTransformGroup("CurrencyAnimViews"));
        }

        private void InstallBoardVFX()
        {
            Container.BindFactory<VFXArgs, BoardVFXView, BoardVFXView.Factory>()
                .FromPoolableMemoryPool<VFXArgs, BoardVFXView, BoardVFXView.Pool>(poolBinder => poolBinder
                    .WithInitialSize(3)
                    .FromComponentInNewPrefab(_applicationPrefabSettings.BoardVFXView)
                    .UnderTransformGroup("BoardVFXs"));
        }

        private void InstallPlayerMobView()
        {
            Container.BindFactory<PlayerMobView, PlayerMobView.Factory>()
                .FromPoolableMemoryPool<PlayerMobView, PlayerMobView.Pool>(poolBinder => poolBinder
                    .WithInitialSize(_characterItemsettings.characterGridItemPoolSize)
                    .FromComponentInNewPrefab(_characterItemsettings.PlayerMobViewPrefab)
                    .UnderTransformGroup(_characterItemsettings.characterGridItemPoolName + "----" + "Player"));
        }

        private void InstallEnemyMobView()
        {
            Container.BindFactory<EnemyMobView, EnemyMobView.Factory>()
                .FromPoolableMemoryPool<EnemyMobView, EnemyMobView.Pool>(poolBinder => poolBinder
                    .WithInitialSize(_characterItemsettings.characterGridItemPoolSize)
                    .FromComponentInNewPrefab(_characterItemsettings.EnemyMobViewPrefab)
                    .UnderTransformGroup(_characterItemsettings.characterGridItemPoolName + "----" + "Enemy"));
        }

        private void InstallDamageTextFeedbacksView()
        {
            Container.BindFactory<DamageTextFeedbackView, DamageTextFeedbackView.Factory>()
                .FromPoolableMemoryPool<DamageTextFeedbackView, DamageTextFeedbackView.Pool>(poolBinder => poolBinder
                .WithInitialSize(3)
                .FromComponentInNewPrefab(_applicationPrefabSettings.DamageTextPrefab)
                .UnderTransformGroup("DamageTexts"));
        }

        private void InstallBombWeopanVFXView()
        {
            Container.BindFactory<BombWeopanVFXView, BombWeopanVFXView.Factory>()
                .FromPoolableMemoryPool<BombWeopanVFXView, BombWeopanVFXView.Pool>(poolBinder => poolBinder
                .WithInitialSize(20)
                .FromComponentInNewPrefab(_applicationPrefabSettings.BombWeopanVFXViewPrefab)
                .UnderTransformGroup("BombWeopanVFXViews"));
        }

        private void InstallArrowWeopanVFXView()
        {
            Container.BindFactory<ArrowWeopanVFXView, ArrowWeopanVFXView.Factory>()
                .FromPoolableMemoryPool<ArrowWeopanVFXView, ArrowWeopanVFXView.Pool>(poolBinder => poolBinder
                .WithInitialSize(20)
                .FromComponentInNewPrefab(_applicationPrefabSettings.ArrowWeopanVFXViewPrefab)
                .UnderTransformGroup("ArrowWeopanVFXViews"));
        }

        private void InstallAxeWeopanVFXView()
        {
            Container.BindFactory<AxeWeopanVFXView, AxeWeopanVFXView.Factory>()
                .FromPoolableMemoryPool<AxeWeopanVFXView, AxeWeopanVFXView.Pool>(poolBinder => poolBinder
                .WithInitialSize(20)
                .FromComponentInNewPrefab(_applicationPrefabSettings.AxeWeopanVFXViewPrefab)
                .UnderTransformGroup("AxeWeopanVFXViews"));
        }

    }
}


