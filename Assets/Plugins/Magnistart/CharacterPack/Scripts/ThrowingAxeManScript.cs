using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnistart_CharacterPack
{
    [ExecuteAlways]
    public class ThrowingAxeManScript : MonoBehaviour
    {
        //A reference to characterScript to get current weapon type and weapon level.
        [SerializeField] private CharacterSkin characterSkin = null;
        //Parent for the axes that will be instantiated in character hierarchy.
        [SerializeField] private Transform axePlace = null;
        //An array of array for every axe prefab character has.
        [SerializeField] private ThrowingAxeArray[] axePrefabs = null;

        //Axe instance currently held by character.
        [SerializeField] private ThrowingAxe axeInHand = null;

        private void Awake()
        {
            //If there is an axe in hand, change it when weapon index or level is changed.
            characterSkin.OnWeaponChanged.AddListener(SetWeapon);
        }
        public void SetWeapon()
        {
            //We don't need to do anything if there is no axe in hand.
            if (axeInHand == null) return;
            //Destroy old axe in hand.
            DestroyImmediate(axeInHand.gameObject);
            //Instantiate an axe in hand with new type and level.
            axeInHand = Instantiate(axePrefabs[characterSkin.weaponType].transforms[characterSkin.weaponLevel], axePlace);
        }
        //This will be called by ThrowingAxeMan_Attack animation.
        public void GetAxe()
        {
            //Instantiate a new axe if there is no axe in hand.
            if (axeInHand == null)
                axeInHand = Instantiate(axePrefabs[characterSkin.weaponType].transforms[characterSkin.weaponLevel], axePlace);
        }
        //This will be called by ThrowingAxeMan_Attack animation.
        public void ThrowAxe()
        {
            if (axeInHand == null) return;

            //Reset axe scale and rotation
            axeInHand.transform.rotation = Quaternion.identity;
            axeInHand.transform.localScale = Vector3.one;
            //Enable trail after shooting
            axeInHand.Shoot();
            //Release axe from gameobject
            axeInHand.transform.SetParent(null);
            //Axe is not in hand anymore
            axeInHand = null;
        }
    }
}