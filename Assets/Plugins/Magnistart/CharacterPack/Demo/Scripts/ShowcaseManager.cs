using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Magnistart_CharacterPack
{
    public class ShowcaseManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCam = null;
        [SerializeField] private CharacterMover firstSelectedCharacter = null;
        [SerializeField] private Transform targetPosIndicator = null;
        [SerializeField] private Button toDemoSceneButton = null;

        //A variable to keep track of current selected character.
        private CharacterMover currentSelectedCharacter = null;
        //An array to keep raycasting results.
        private RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[1];

        private void Awake()
        {
            //Inform first selected character.
            currentSelectedCharacter = firstSelectedCharacter;
            firstSelectedCharacter.isSelected = true;

            toDemoSceneButton.onClick.AddListener(() => SceneManager.LoadScene("CharacterPack_DemoScene"));
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosInWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
                if (Physics2D.RaycastNonAlloc(mousePosInWorld, Vector2.zero, raycastHit2Ds) > 0)
                {
                    //Check if the object hit by the raycast is a character.
                    CharacterMover pressedCharacter = raycastHit2Ds[0].collider.GetComponent<CharacterMover>();
                    if (pressedCharacter != null && pressedCharacter != currentSelectedCharacter)
                    {
                        currentSelectedCharacter.isSelected = false;
                        pressedCharacter.isSelected = true;
                        currentSelectedCharacter = pressedCharacter;
                        targetPosIndicator.gameObject.SetActive(false);
                    }
                }
                else //If there is no character hit by a raycast, then player clicked on the ground. Move the character.
                {
                    currentSelectedCharacter.MoveTo(mousePosInWorld);
                    targetPosIndicator.gameObject.SetActive(true);
                    targetPosIndicator.position = mousePosInWorld;
                }
            }
        }
    }
}