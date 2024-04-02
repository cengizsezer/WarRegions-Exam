using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetterView : MonoBehaviour
{
    [SerializeField] public Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI text;

    [FoldoutGroup("TextSettings")]
    [SerializeField] private Color32 PassiveTextColor;
    [FoldoutGroup("TextSettings")]
    [SerializeField] private Color32 ActiveTextColor;

    [FoldoutGroup("ButtonSettings")]
    [SerializeField] private Sprite PassiveButtonSprite;
    [FoldoutGroup("ButtonSettings")]
    [SerializeField] private Sprite ActiveButtonSprite;


    public bool IsButtonActive { get; private set; }

    public void ButtonActivate()
    {
        if (!button) return;
        if (!buttonImage) return;
        if (!text) return;

        buttonImage.sprite = ActiveButtonSprite;
        buttonImage.raycastTarget = true;
        button.interactable = true;
        IsButtonActive = true;

        //text.color = ActiveTextColor;
    }

    public void ButtonDeactivate()
    {
        if (!button) return;
        if (!buttonImage) return;
        if (!text) return;

        buttonImage.sprite = PassiveButtonSprite;
        buttonImage.raycastTarget = false;
        button.interactable = false;
        IsButtonActive = false;

        //text.color = PassiveTextColor;
    }

    public void SetText(string message)
    {
        text.text = message;
    }


}

