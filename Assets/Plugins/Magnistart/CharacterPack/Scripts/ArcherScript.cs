using UnityEngine;

namespace Magnistart_CharacterPack
{
    public class ArcherScript : MonoBehaviour
    {
        //A reference to characterScript to get current weapon type and weapon level.
        [SerializeField] private CharacterSkin characterScript = null;
        //Parent for the arrows that will be instantiated in character hierarchy.
        [SerializeField] private Transform arrowPlace = null;
        //An array of array for every arrow prefab archer has.
        [SerializeField] private TransformArray[] arrowPrefabs = null;

        //Arrow instance currently held by archer.
        private Transform arrowInHand = null;

        private void Awake()
        {
            //Remove arrow in hand when animation changed.
            DemoSceneManager.OnAnimationChanged.AddListener(() =>
            {
                if (arrowInHand != null) Destroy(arrowInHand.gameObject);
            });
        }

        //This will be called by Archer_Attack animation.
        public void DrawArrow()
        {
            if (arrowInHand != null) Destroy(arrowInHand.gameObject);
            //Get a new arrow from quiver.
            arrowInHand = Instantiate(arrowPrefabs[characterScript.weaponType].transforms[characterScript.weaponLevel], arrowPlace);
        }

        //This will be called by Archer_Attack animation.
        public void ShootArrow()
        {
            if (arrowInHand == null) return;

            //Enable trail after shooting
            arrowInHand.GetChild(1).gameObject.SetActive(true);
            //Release arrow from gameobject
            arrowInHand.SetParent(null);
            //Start moving arrow
            arrowInHand.GetComponent<ArrowMover>().enabled = true;
            //Arrow is not in hand anymore
            arrowInHand = null;
        }
    }
}