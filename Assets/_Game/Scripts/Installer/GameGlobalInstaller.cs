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
        private MobGamePlaySettings _characterItemsettings;

        [Inject]
        private void Construct
        (
              ApplicationPrefabSettings applicationPrefabSetting
            , CurrencySettings currencySettings
            , MobGamePlaySettings characterItemsettings
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
            Container.BindInterfacesAndSelfTo<MobVFXController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardCoordinateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardGamePlayController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardDataController>().AsSingle();
            Container.BindInterfacesAndSelfTo<PathFinderController>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyMobController>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemySpawnController>().AsSingle();
            
            ////Factory
            Container.BindFactory<Object, BaseScreen, BaseScreen.Factory>().FromFactory<PrefabFactory<BaseScreen>>();

            ////VFX
            InstallCurrencyVfx();
            InstallMobVFX();
          

            ////GAMEOBJECT
            InstallGridViews();
            InstallMobView();
            InstallMilitaryBaseViews();
            InstallDamageTextFeedbacksView();

           
           
        }


        private void InstallGridViews()
        {
            Container.BindFactory<GridView.Args, GridView, GridView.Factory>()
               .FromPoolableMemoryPool<GridView.Args, GridView, GridView.Pool>(poolbinder => poolbinder
                   .WithInitialSize(GlobalConsts.BoardConsts.ROWS* GlobalConsts.BoardConsts.COLUMNS)
                   .FromComponentInNewPrefab(_applicationPrefabSettings.GridViewPrefab)
                   .UnderTransformGroup("GridViews"));
        }

        private void InstallMilitaryBaseViews()
        {
            Container.BindFactory<MilitaryBaseView.Args, MilitaryBaseView, MilitaryBaseView.Factory>()
              .FromPoolableMemoryPool<MilitaryBaseView.Args, MilitaryBaseView, MilitaryBaseView.Pool>(poolbinder => poolbinder
                  .WithInitialSize(10)
                  .FromComponentInNewPrefab(_applicationPrefabSettings.MilitaryBaseViewPrefab)
                  .UnderTransformGroup("MilitaryBaseViews"));
        }
        private void InstallCurrencyVfx()
        {
            Container.BindFactory<CurrencyVFXView.Args, CurrencyVFXView, CurrencyVFXView.Factory>()
                .FromPoolableMemoryPool<CurrencyVFXView.Args, CurrencyVFXView, CurrencyVFXView.Pool>(poolbinder => poolbinder
                    .WithInitialSize(5)
                    .FromComponentInNewPrefab(_currencySettings.CurrencyAnim)
                    .UnderTransformGroup("CurrencyAnimViews"));
        }

        private void InstallMobVFX()
        {
            Container.BindFactory<VFXArgs, MobVFXView, MobVFXView.Factory>()
                .FromPoolableMemoryPool<VFXArgs, MobVFXView, MobVFXView.Pool>(poolBinder => poolBinder
                    .WithInitialSize(3)
                    .FromComponentInNewPrefab(_applicationPrefabSettings.MobVFXView)
                    .UnderTransformGroup("MobVFXs"));
        }

        private void InstallMobView()
        {
            Container.BindFactory<MobView, MobView.Factory>()
                .FromPoolableMemoryPool<MobView, MobView.Pool>(poolBinder => poolBinder
                    .WithInitialSize(_characterItemsettings.characterGridItemPoolSize)
                    .FromComponentInNewPrefab(_characterItemsettings.MobViewPrefab)
                    .UnderTransformGroup(_characterItemsettings.characterGridItemPoolName + "----" + "Player"));
        }
       

        private void InstallDamageTextFeedbacksView()
        {
            Container.BindFactory<DamageTextFeedbackView, DamageTextFeedbackView.Factory>()
                .FromPoolableMemoryPool<DamageTextFeedbackView, DamageTextFeedbackView.Pool>(poolBinder => poolBinder
                .WithInitialSize(3)
                .FromComponentInNewPrefab(_applicationPrefabSettings.DamageTextPrefab)
                .UnderTransformGroup("DamageTexts"));
        }
       

    }
}


