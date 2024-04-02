using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Magnistart_CharacterPack
{
    public class DemoSceneManager : MonoBehaviour
    {
        [SerializeField] private CharacterSkin[] characterPrefabs = null;

        [SerializeField] private Button toShowcaseSceneButton = null;

        [SerializeField] private Button prevHeroButton = null;
        [SerializeField] private Button prevSkinButton = null;
        [SerializeField] private Button prevWeaponButton = null;
        [SerializeField] private Button prevWeaponSkinButton = null;

        [SerializeField] private Button nextHeroButton = null;
        [SerializeField] private Button nextSkinButton = null;
        [SerializeField] private Button nextWeaponButton = null;
        [SerializeField] private Button nextWeaponSkinButton = null;

        [SerializeField] private Transform buttonsParent = null;


        private int currentCharacterIndex = 0;
        private CharacterSkin currentCharacter = null;
        private Animator currentCharacterAnimator = null;
        public static UnityEvent OnAnimationChanged = new UnityEvent();
        private void Start()
        {
            Application.targetFrameRate = 60;
            SetCharacter(0);
            toShowcaseSceneButton.onClick.AddListener(() => SceneManager.LoadScene("CharacterPack_ShowcaseScene"));

            prevHeroButton.onClick.AddListener(SetPrevCharacter);
            nextHeroButton.onClick.AddListener(SetNextCharacter);

            prevSkinButton.onClick.AddListener(() => currentCharacter.SetPrevCharacterLevel());
            nextSkinButton.onClick.AddListener(() => currentCharacter.SetNextCharacterLevel());

            prevWeaponButton.onClick.AddListener(() => currentCharacter.SetPrevWeapon());
            nextWeaponButton.onClick.AddListener(() => currentCharacter.SetNextWeapon());

            prevWeaponSkinButton.onClick.AddListener(() => currentCharacter.SetPrevWeaponLevel());
            nextWeaponSkinButton.onClick.AddListener(() => currentCharacter.SetNextWeaponLevel());
        }
        private void SetNextCharacter() => SetCharacter((currentCharacterIndex + 1) % characterPrefabs.Length);
        private void SetPrevCharacter() => SetCharacter((currentCharacterIndex + characterPrefabs.Length - 1) % characterPrefabs.Length);
        private void SetCharacter(int characterIndex)
        {
            //Hide current character
            if (currentCharacter != null)
            {
                Destroy(currentCharacter.gameObject);
            }
            //Instantiate new character and set as current
            currentCharacterIndex = characterIndex;
            currentCharacter = Instantiate(characterPrefabs[currentCharacterIndex]);
            currentCharacterAnimator = currentCharacter.GetComponent<Animator>();
            currentCharacter.SetCharacterLevel(0);
            currentCharacter.SetWeapon(0);
            currentCharacter.SetWeaponLevel(0);

            //Remove all animation buttons
            foreach (var child in buttonsParent.GetComponentsInChildren<Button>())
                Destroy(child.gameObject);
            //Create new animation buttons for each animation this character has.
            foreach (var animationData in currentCharacter.animationData.data)
            {
                var button = Instantiate(prevHeroButton, buttonsParent);
                button.onClick.RemoveAllListeners();
                button.GetComponentInChildren<Text>().text = animationData.name;
                button.onClick.AddListener(() =>
                {
                    if (currentCharacterAnimator.IsInTransition(0)) return;
                    if (currentCharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationData.name)) return;
                    currentCharacterAnimator.CrossFadeInFixedTime(animationData.name, animationData.crossFadeDur);
                    OnAnimationChanged.Invoke();
                });
            }

            prevSkinButton.gameObject.SetActive(currentCharacter.characterSprites.Length > 0);
            nextSkinButton.gameObject.SetActive(currentCharacter.characterSprites.Length > 0);

            prevWeaponButton.gameObject.SetActive(currentCharacter.weaponSprites.Length > 0);
            nextWeaponButton.gameObject.SetActive(currentCharacter.weaponSprites.Length > 0);
            prevWeaponSkinButton.gameObject.SetActive(currentCharacter.weaponSprites.Length > 0);
            nextWeaponSkinButton.gameObject.SetActive(currentCharacter.weaponSprites.Length > 0);
        }
    }
}