using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Controllers;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MyProject.GamePlay.Views
{
    public class GridView : BaseView, IPoolable<GridView.Args, IMemoryPool>
    {
        [Header("RESOURCE DATA")]
        [SerializeField] private ResourceTypeData _resourceTypeData;
        public ResourceTypeData ResourceTypeData
        {
            get => _resourceTypeData;
            set
            {
                _resourceTypeData = value;
                SetProps();


            }
        }
        private IMemoryPool _pool;
        public ColorType ColorType;
        public IBlockEntityTypeDefinition TypeDefinition;
        public int ConfigureType;


        [Header("PROPS")]
        [SerializeField] SkinnedMeshRenderer smr;
        [SerializeField] SpriteRenderer tentSprite;
        [SerializeField] MeshRenderer tentRenderer;

        public SkinnedMeshRenderer GetSmr() => smr;

        public ColorType GetTileType() => ColorType;


        #region Injection

        private SignalBus _signalBus;
        private PlayerMobController _playerMobController;

        private BoardCoordinateSystem _boardCoordinateSystem;

        [Inject]
        private void Construct(SignalBus signalBus
            , PlayerMobController playerMobController

            , BoardCoordinateSystem boardCoordinateSystem)
        {
            _signalBus = signalBus;
            _playerMobController = playerMobController;
            _boardCoordinateSystem = boardCoordinateSystem;
        }

        #endregion

        #region PathFinder
        public List<GridView> neighborList = new();
        public bool IsSea = false;
        public int gridX; // Grid koordinatı X
        public int gridZ; // Grid koordinatı Y
        public int gCost; // Başlangıç düğümüne olan maliyet
        public int hCost; // Hedef düğüme olan tah
        public GridView parent;
        public int fCost // gCost + hCost
        {
            get { return gCost + hCost; }
        }


        public bool IsMountain = false;
       

        public void FindSeaNeighbor()
        {
            neighborList.Clear();

            foreach (GridView neighbor in _boardCoordinateSystem.LsAllGridViews)
            {
                if (neighbor == this || !neighbor.gameObject.activeInHierarchy)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, neighbor.transform.position);

                if (distance >= 8)
                {
                    continue;
                }

                neighborList.Add(neighbor);
            }
        }
        public void FindLandNeighbor()
        {
           

            neighborList.Clear();
            foreach (GridView neighbor in _boardCoordinateSystem.LsAllGridViews)
            {
                if (neighbor == this || !neighbor.gameObject.activeInHierarchy || neighbor.IsSea)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, neighbor.transform.position);

                if (distance >= 8)
                {
                    continue;
                }

                neighborList.Add(neighbor);
            }
        }
        #endregion

        public void SetColor(string s)
        {
            smr.material.color = ToColorFromHex(s);
        }

        public override void Initialize()
        {
            
        }

        public void SetProps()
        {
            SetColor(ResourceTypeData.HexColor);
            ColorType = ResourceTypeData.ColorType;
            ConfigureType = ResourceTypeData.ConfigureType;

        }
        public void DespawnItem()
        {
            neighborList.Clear();
           
            _pool.Despawn(this);
        }
        public void OnDespawned()
        {

        }
        public void OnSpawned(Args args, IMemoryPool pool)
        {
            _pool = pool;
            transform.SetParent(args.parent);
            transform.localScale = args.localScale;

            float xPos = args.xPos;
            float zPos = args.zPos;
            float yPos = args.yPos;
            transform.localPosition = new Vector3(xPos, yPos, zPos);
           
        }

        public class Factory : PlaceholderFactory<Args, GridView> { }
        public class Pool : MonoPoolableMemoryPool<Args, IMemoryPool, GridView> { }
        public readonly struct Args
        {
            public readonly Transform parent;
            public readonly Vector3 localScale;
            public readonly Vector3 offSet;
            public readonly int rowValue;
            public readonly int coloumValue;
            public readonly float xPos;
            public readonly float zPos;
            public readonly float yPos;

            public Args(Transform Parent, Vector3 LocalScale, int RowValue, int ColoumValue, float XPos, float YPos, float ZPos) : this()
            {
                parent = Parent;
                localScale = LocalScale;
                rowValue = RowValue;
                coloumValue = ColoumValue;
                xPos = XPos;
                yPos = YPos;
                zPos = ZPos;
            }
        }
    }
}


