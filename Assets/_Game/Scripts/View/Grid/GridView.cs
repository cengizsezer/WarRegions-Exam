using MyProject.Core.Const;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Characters;
using MyProject.GamePlay.Controllers;
using UnityEngine;
using Zenject;


public  class GridView : BaseView, IPoolable<GridView.Args, IMemoryPool>
{
    [SerializeField] private Transform _itemHolder;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public Vector2Int Coordinates;
    public BaseGridItemView PlayerCharacter;
    [SerializeField] private GameObject _selectionFrame;
    [SerializeField] private Animation _selectionAnimation, _itemSpawnAnimation;
    private IMemoryPool _pool;

    public Transform ItemHolder => _itemHolder;

    #region GridState

    public GridState _gridState;

    public GridState GridState
    {
        get => _gridState;
        set
        {
            _gridState = value;

        }
    }

    #endregion

    #region Injection

    private SignalBus _signalBus;
    private PlayerMobController _playerMobController;
    private BoardFXController _boardFXController;

    [Inject]
    private void Construct(SignalBus signalBus
        , PlayerMobController playerMobController
        , BoardFXController boardFXController)
    {
        _signalBus = signalBus;
        _playerMobController = playerMobController;
        _boardFXController = boardFXController;
    }

    #endregion

    public void Init(GridData gridData)
    {
        Initialize();
    }

    
    public override void Initialize()
    {
        
    }
    public void DespawnItem()
    {
        
    }
    public void OnDespawned()
    {
        
    }
    public void OnSpawned(Args args, IMemoryPool pool)
    {
        _pool = pool;
        transform.SetParent(args.parent);
        _spriteRenderer.sortingOrder = GlobalConsts.SortingOrders.GridViewDefault;
        transform.localScale = args.localScale;
       
        var pos = new Vector3(
            args.offSet.x + args.coloumValue * args.localScale.x,
            args.offSet.y + args.rowValue * args.localScale.y,
            args.offSet.z
        );

        transform.localPosition = pos;

    }
    
    public void ChangeState(GridState gridState) => _gridState = gridState;
    
    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        

    }
    public void SelectGrid(bool animate)
    {
        _selectionFrame.SetActive(true);

        if (animate)
        {
            _selectionAnimation.Stop();
            _itemSpawnAnimation.Stop();
            _selectionAnimation.Play();
            _itemSpawnAnimation.Play();
        }
    }

    public void UnSelectGrid()
    {
        _selectionFrame.SetActive(false);
    }

    public void PlayVFX(VFXType vfxType)
    {
        _boardFXController.PlayVFX(new VFXArgs(vfxType, transform, 2f));
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
        
               
        public Args(Transform Parent,Vector3 LocalScale,Vector3 OffSet, int RowValue, int ColoumValue) : this()
        {
            parent = Parent;
            localScale = LocalScale;
            offSet = OffSet;
            rowValue = RowValue;
            coloumValue = ColoumValue;
        }
    }
}
