using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] Vector2 spacing;
    [SerializeField] float rotationDuration;
    [SerializeField] Ease rotationEase;
    [SerializeField] float expandDuration;
    [SerializeField] float collapseDuration;
    [SerializeField] Ease expandEase;
    [SerializeField] Ease collapseEase;
    [SerializeField] float expandFadeDuration;
    [SerializeField] float collapseFadeDuration;
    [SerializeField] Button mainButton;
    public bool IsRotate = true;
    bool isExpanded = false;
    int itemsCount;

    public RectTransform content;
    public Vector2 resetPosition;
    [SerializeField] float sizeValue;
    public List<SettingsPopupChild> lsChild = new List<SettingsPopupChild>();

    private void Start()
    {
        Init(transform);
    }
    public void Init(Transform Parent)
    {
        DOTween.SetTweensCapacity(500, 50);
        itemsCount = transform.childCount - 1;

        for (int i = 0; i < itemsCount; i++)
        {
            lsChild.Add(Parent.GetChild(i + 1).GetComponent<SettingsPopupChild>());
            lsChild[i].img.DOFade(0f, .1f).From(1f);

            if (lsChild[i].img_Active != null && lsChild[i].txt != null)
            {
                lsChild[i].img_Active.DOFade(0f, .1f).From(1f);
                lsChild[i].txt.DOFade(0f, .1f).From(1f);
            }

        }


        mainButton.onClick.AddListener(ToggleMenu);
        mainButton.transform.SetAsLastSibling();
        ResetPositions();
    }

    void OnDestroy() => mainButton.onClick.RemoveListener(ToggleMenu);


    void ResetPositions()
    {
        for (int i = 0; i < lsChild.Count; i++)
        {
            lsChild[i].rectTrans.anchoredPosition = resetPosition;
        }
    }

    void ToggleMenu()
    {

        isExpanded = !isExpanded;

        if (isExpanded)
        {
            for (int i = 0; i < lsChild.Count; i++)
            {
                OpenedMenu(lsChild[i], i);
            }
        }
        else
        {
            for (int i = 0; i < lsChild.Count; i++)
            {
                ClosedMenu(lsChild[i]);
            }

        }

        if (!IsRotate) return;
        RotateMainButton();

    }
    public bool IsManuelResetPosition = false;
    void OpenedMenu(SettingsPopupChild child, int idx)
    {
        child.rectTrans.DOAnchorPos((resetPosition = IsManuelResetPosition ? resetPosition : mainButton.GetComponent<RectTransform>().anchoredPosition) + spacing * (idx + 1)
            , expandDuration).SetEase(expandEase);
        child.img.DOFade(1f, expandFadeDuration).From(0f);


        if (child.img_Active != null && child.txt != null)
        {
            child.img_Active.DOFade(1f, expandFadeDuration).From(0f);
            child.txt.DOFade(1f, expandFadeDuration).From(0f);
        }

        if (content != null)
        {
            Vector2 size = content.sizeDelta;
            size.y += (sizeValue * (lsChild.Count + 1)) * idx + 1;
            content.DOSizeDelta(size, expandDuration + 0.02f);

        }

    }

    void ClosedMenu(SettingsPopupChild child)
    {
        child.rectTrans.DOAnchorPos((resetPosition = IsManuelResetPosition ? resetPosition : mainButton.GetComponent<RectTransform>().anchoredPosition)
            , collapseDuration).SetEase(collapseEase);
        child.img.DOFade(0f, collapseFadeDuration);

        if (child.img_Active != null && child.txt != null)
        {
            child.txt.DOFade(0f, collapseFadeDuration).From(1f);
            child.img_Active.DOFade(0f, collapseFadeDuration).From(1f);
        }

        if (content != null)
        {
            Vector2 size = content.sizeDelta;
            size.y = 260f;
            content.DOSizeDelta(size, expandDuration + 0.02f);
        }

    }

    void RotateMainButton()
    {
        mainButton.transform
              .DORotate(Vector3.forward * 180f, rotationDuration)
              .From(Vector3.zero)
              .SetEase(rotationEase);
    }
}
