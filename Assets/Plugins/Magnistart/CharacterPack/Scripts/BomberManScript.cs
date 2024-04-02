using UnityEngine;

namespace Magnistart_CharacterPack
{
    [ExecuteAlways]
    public class BomberManScript : MonoBehaviour
    {
        //A reference to characterScript to get current weapon type and weapon level.
        [SerializeField] private CharacterSkin characterSkin = null;
        //Parent for the bombs that will be instantiated in character hierarchy.
        [SerializeField] private Transform bombPlace = null;
        //An array of array for every bomb prefab bomber has.
        [SerializeField] private TransformArray[] bombPrefabs = null;

        //Bomb instance currently held by bomber.
        [SerializeField] private Transform bombInHand = null;
        private void Awake()
        {
            //If there is a bomb in hand, change it when weapon index or level is changed.
            characterSkin.OnWeaponChanged.AddListener(SetWeapon);
        }
        public void SetWeapon()
        {
            //We don't need to do anything if there is no bomb in hand.
            if (bombInHand == null) return;
            //Destroy old bomb in hand.
            DestroyImmediate(bombInHand.gameObject);
            //Instantiate a bomb in hand with new type and level.
            bombInHand = Instantiate(bombPrefabs[characterSkin.weaponType].transforms[characterSkin.weaponLevel], bombPlace);
        }
        //This will be called by BomberMan_Attack animation.
        public void GetBomb()
        {
            //Instantiate a new bomb if there is no bomb in hand.
            if (bombInHand == null)
                bombInHand = Instantiate(bombPrefabs[characterSkin.weaponType].transforms[characterSkin.weaponLevel], bombPlace);
        }
        //This will be called by BomberMan_Attack animation.
        public void ThrowBomb()
        {
            if (bombInHand == null) return;

            //Reset bomb scale and rotation
            bombInHand.rotation = Quaternion.identity;
            bombInHand.localScale = Vector3.one;
            //Enable trail after shooting
            bombInHand.GetChild(0).GetChild(0).gameObject.SetActive(true);
            //Release bomb from gameobject
            bombInHand.SetParent(null);
            //Start moving bomb
            bombInHand.GetComponent<Animator>().enabled = true;
            //Bomb is not in hand anymore
            bombInHand = null;
        }
    }
}