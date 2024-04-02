using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Magnistart_CharacterPack
{
    public class CharacterSkin : MonoBehaviour
    {
        //Sprite renderers of each part of the character.
        public SpriteRenderer[] characterSpriteRenderers = null;
        //Sprites for each character level.
        public SpriteArray[] characterSprites = null;
        //Gameobjects to enable for each character level.
        public GameObjectArray[] characterGameObjects = null;

        //Sprite renderers of all weapons.
        public SpriteRenderer[] weaponSpriteRenderers = null;
        //Sprites for each weapon types and levels.
        public SpriteArrayArray[] weaponSprites = null;
        //Gameobjects to enable for each weapon type and level.
        public GameObjectArray[] weaponGameObjects = null;

        //Sprite renderers for each weapon trail.
        public SpriteRenderer[] trails = null;
        //Color data for each weapon type and level.
        public Color[] trailColors = null;
        //Scale data for each weapon type and level.
        public Vector3[] trailScales = null;
        //Rotation data for each weapon type and level.
        public Vector3[] trailRotations = null;
        //Position data for each weapon type and level.
        public Vector3[] trailPositions = null;

        //Animation and fade data
        public CharacterAnimationData animationData = null;

        [HideInInspector] public UnityEvent OnWeaponChanged = new UnityEvent();

        public int heroLevel { get; private set; } = 0;
        public int weaponType { get; private set; } = 0;
        public int weaponLevel { get; private set; } = 0;
        public void SetNextCharacterLevel() => SetCharacterLevel((heroLevel + 1) % characterSprites.Length);
        public void SetPrevCharacterLevel() => SetCharacterLevel((heroLevel + characterSprites.Length - 1) % characterSprites.Length);
        public void SetCharacterLevel(int level)
        {
            if (characterSprites.Length == 0) return;
            heroLevel = level;
            //Change sprite of all sprite renderers to sprite of new level.
            for (int i = 0; i < characterSpriteRenderers.Length; i++) characterSpriteRenderers[i].sprite = characterSprites[level].sprites[i];
            //Enable and disable all gameObjects for character levels.
            for (int i = 0; i < characterGameObjects.Length; i++) for (int k = 0; k < characterGameObjects[i].gameObjects.Length; k++) characterGameObjects[i].gameObjects[k].SetActive(level == i);
        }
        public void SetNextWeapon() => SetWeapon((weaponType + 1) % weaponSprites.Length);
        public void SetPrevWeapon() => SetWeapon((weaponType + weaponSprites.Length - 1) % weaponSprites.Length);
        public void SetWeapon(int weaponIndex)
        {
            if (weaponSprites.Length == 0) return;
            this.weaponType = weaponIndex;
            //Change sprite of all sprite renderers to sprite of new level.
            for (int i = 0; i < weaponSpriteRenderers.Length; i++) weaponSpriteRenderers[i].sprite = weaponSprites[weaponIndex].spritesArrays[weaponLevel].sprites[i];
            //Enable and disable all gameObjects for weapon levels.
            for (int i = 0; i < weaponGameObjects.Length; i++) for (int m = 0; m < weaponGameObjects[i].gameObjects.Length; m++) weaponGameObjects[i].gameObjects[m].SetActive(false);
            for (int i = 0; i < weaponGameObjects[weaponIndex].gameObjects.Length; i++) weaponGameObjects[weaponIndex].gameObjects[i].SetActive(true);
            //Apply trail data for this weapon.
            for (int i = 0; i < trails.Length; i++)
            {
                trails[i].color = trailColors[weaponIndex];
                trails[i].transform.localScale = trailScales[weaponIndex];
                trails[i].transform.localRotation = Quaternion.Euler(trailRotations[weaponIndex]);
                trails[i].transform.localPosition = trailPositions[weaponIndex];
            }
            OnWeaponChanged?.Invoke();
        }
        public void SetNextWeaponLevel() => SetWeaponLevel((weaponLevel + 1) % weaponSprites[weaponType].spritesArrays.Length);
        public void SetPrevWeaponLevel() => SetWeaponLevel((weaponLevel + weaponSprites[weaponType].spritesArrays.Length - 1) % weaponSprites[weaponType].spritesArrays.Length);
        public void SetWeaponLevel(int level)
        {
            if (weaponSprites.Length == 0) return;
            weaponLevel = level;
            //Change sprite of all sprite renderers to sprite of new level.
            for (int i = 0; i < weaponSpriteRenderers.Length; i++) weaponSpriteRenderers[i].sprite = weaponSprites[weaponType].spritesArrays[weaponLevel].sprites[i];
            //Enable and disable all gameObjects for weapon levels.

            for (int i = 0; i < weaponGameObjects.Length; i++) for (int m = 0; m < weaponGameObjects[i].gameObjects.Length; m++) weaponGameObjects[i].gameObjects[m].SetActive(false);
            for (int i = 0; i < weaponGameObjects[weaponType].gameObjects.Length; i++) weaponGameObjects[weaponType].gameObjects[i].SetActive(true);
            OnWeaponChanged?.Invoke();
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CharacterSkin))]
    [CanEditMultipleObjects()]
    class CharacterScriptEditor : Editor
    {
        CharacterSkin t => target as CharacterSkin;
        private int characterLevel = 0;
        private int weaponIndex = 0;
        private int weaponLevel = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            if (GUILayout.Button("Next Character Level")) { if (t.characterSprites.Length == 0) return; Undo.RegisterCompleteObjectUndo(target, "Changed Skin"); characterLevel = (characterLevel + 1) % t.characterSprites.Length; t.SetCharacterLevel(characterLevel); }
            if (GUILayout.Button("Next Weapon Type")) { if (t.weaponSprites.Length == 0) return; Undo.RegisterCompleteObjectUndo(target, "Changed Skin"); weaponIndex = (weaponIndex + 1) % t.weaponSprites.Length; t.SetWeapon(weaponIndex); }
            if (GUILayout.Button("Next Weapon Level")) { if (t.weaponSprites.Length == 0) return; Undo.RegisterCompleteObjectUndo(target, "Changed Skin"); weaponLevel = (weaponLevel + 1) % t.weaponSprites[weaponIndex].spritesArrays.Length; t.SetWeaponLevel(weaponLevel); }
        }
    }
#endif
}