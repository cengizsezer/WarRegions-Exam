using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Characters;
using System;
using System.Collections.Generic;
using Zenject;

namespace MyProject.GamePlay.Controllers
{
    public class PlayerMobController:IDisposable
    {
        public List<PlayerMobView> LsPlayerMobViews = new();

        #region Injection


        private readonly PlayerMobView.Factory _playerMobFactory;
        private readonly SignalBus _signalBus;

        [Inject]
        public PlayerMobController(
            PlayerMobView.Factory playerMobFactory
            ,SignalBus signalBus
            )
        {

            _playerMobFactory = playerMobFactory;
            _signalBus = signalBus;
        }

        #endregion

        public void Init()
        {
            _signalBus.Subscribe<LevelFailSignal>(Disable);
        }
       
        public BaseGridItemView GetRandomCharacter()
        {
            BaseGridItemView character = _playerMobFactory.Create();

            character.Init(new ItemData
            {
                GridItemType = GameplayMobType.Player,
                CharacterType = GetRandomCharacterType(),
                ItemLevel = 1,
                ItemName = ItemName.Player

            }); ;
           
            return character;
        }

        private CharMobType GetRandomCharacterType()
        {
            CharMobType[] allTypes = (CharMobType[])System.Enum.GetValues(typeof(CharMobType));

            int randomIndex = UnityEngine.Random.Range(0, allTypes.Length);
            return allTypes[randomIndex];
        }

        public void Disable()
        {
            LsPlayerMobViews.Clear();
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelFailSignal>(Disable);
        }
    }
}

