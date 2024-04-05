using DG.Tweening;
using MyProject.Core.Const;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Characters;
using MyProject.GamePlay.Controllers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
public enum HexTypes
{
    None,
    Addition,
    Substract,
    Multiply,
    Divide,
    Health,
    Merge,
    Upgrade,
    Tent,
    Speed,
    Empty
}

public enum InitialMobType
{
    None = -1,
    Player = 0,
    Enemy = 1
}

public  class GridView : BaseView, IPoolable<GridView.Args, IMemoryPool>
{
    [Header("VALUES AND TEXT")]
    [HideInInspector]
    internal int value;
    [HideInInspector]
    internal BlockType mType;

    [Header("INITIAL VALUES")]
    [SerializeField] bool hasInitialOwner;
    
    [SerializeField] InitialMobType initialMob;


    [Header("PROPS")]
    [SerializeField] SkinnedMeshRenderer smr;
    [SerializeField] TextMeshPro txt;
    [SerializeField] ParticleSystem psConvert;
    public GameObject[] borders;
    [SerializeField] GameObject goUpgrade, goSpeed;
    [SerializeField] SpriteRenderer tentSprite;
    [SerializeField] MeshRenderer tentRenderer;
    [SerializeField] Collider colTrigger;

    //[HideInInspector]
    public int x, z;

    delegate void OnCollected();
    OnCollected onCollected = null;

    bool hasCollectedBefore = false;

    public SkinnedMeshRenderer GetSmr() => smr;

    public BlockType GetTileType() => mType;

    #region Coloring
    public Color ToColorFromHex(string hexademical)
    {
        string s =hexademical;
        Color newCol = Color.white;
        if (ColorUtility.TryParseHtmlString(s, out newCol))
        {
            return newCol;
        }

        return newCol;
    }

    public string ToHexFromColor(Color color)
    {
        // Renk değerlerini [0, 1] aralığından [0, 255] aralığına dönüştür
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        int a = Mathf.RoundToInt(color.a * 255f);

        // Renk değerlerini hexadecimal formatına dönüştür ve birleştir
        string hex = "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + a.ToString("X2");

        return hex;
    }

    public void SetColor(string s)
    {
        smr.material.color = ToColorFromHex(s);
    }

    public string GetColor() => ToHexFromColor(smr.material.color);

    #endregion

  
    private IMemoryPool _pool;

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
        transform.localScale = args.localScale;

        float xPos = args.xPos;
        float zPos = args.zPos;
        float yPos = args.yPos;
        transform.localPosition = new Vector3(xPos, yPos, zPos);
       
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
        public readonly float xPos;
        public readonly float zPos;
        public readonly float yPos;
               
        public Args(Transform Parent,Vector3 LocalScale,int RowValue, int ColoumValue, float XPos, float YPos,float ZPos) : this()
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
