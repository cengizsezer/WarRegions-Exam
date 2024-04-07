using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public abstract class BaseView : MonoBehaviour, IInitializable, IDisposable
{
    #region Injection

    protected SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }
    #endregion

    public abstract void Initialize();

    public virtual void Dispose()
    {

    }

    public void DestroyView(float delay = 0)
    {
        Dispose();
        Destroy(gameObject, delay);
    }

    public Color ToColorFromHex(string hexademical)
    {
        string s = "#" + hexademical;
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

}

