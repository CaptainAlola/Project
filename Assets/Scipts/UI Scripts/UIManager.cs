using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuBar;
    private bool isMenuActive;

    [SerializeField] private CanvasGroup statsMenu;
    [SerializeField] private CanvasGroup skillsMenu;
    [SerializeField] private CanvasGroup questsMenu;

    [SerializeField] private Image menuToggleImage;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;

    public void ToggleMenu(CanvasGroup target)
    {
        SetMenuState(statsMenu, false);
        SetMenuState(skillsMenu, false);
        SetMenuState(questsMenu, false);

        SetMenuState(target, true);
    }

    public void ToggleMainMenu()
    {
        isMenuActive = !isMenuActive;
        SetMenuState(menuBar, isMenuActive);
        menuToggleImage.sprite = isMenuActive ? closeSprite : openSprite;

        SetMenuState(statsMenu, false);
        SetMenuState(skillsMenu, false);
        SetMenuState(questsMenu, false);
    }

    private void SetMenuState(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }

    public void CloseMenu(CanvasGroup target)
    {
        SetMenuState(target, false);
    }
}
