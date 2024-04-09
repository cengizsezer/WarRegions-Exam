using UnityEngine;
using UnityEditor;
using MyProject.Core.Settings;

#if UNITY_EDITOR
// Custom property drawer for the GridStartLayout class.
[CustomPropertyDrawer(typeof(GridStartLayout))]
public class ArrayPropertyDrawer : PropertyDrawer
{
    private readonly float cellSize = 50;
    private readonly float soHandlePixelSize = 15;

    // Override the OnGUI method to draw the custom property field.
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Display the label associated with the property.
        EditorGUI.PrefixLabel(position, label);

        // Get the SerializedProperty representing the array of BlockDatas.
        SerializedProperty data = property.FindPropertyRelative("BlockDatas").FindPropertyRelative("Blocks");

        float cellWidth = cellSize * 1.3f;
        float x = position.x;
        float y = position.y + cellSize;

        // Retrieve the dimensions of the grid from the serialized properties.
        int width = property.FindPropertyRelative("Width").intValue;
        int height = property.FindPropertyRelative("Height").intValue;

        for (int i = 0; i < width * height; i++)
        {
            Rect newPosition = new Rect(x, y, cellWidth, cellSize);


            // Draw the property field for each element.
            EditorGUI.PropertyField(newPosition, data.GetArrayElementAtIndex(i), GUIContent.none);

            // Get the BaseBlockEntityTypeDefinition from the property.
            //  BaseBlockEntityTypeDefinition typeDef = (data.GetArrayElementAtIndex(i).objectReferenceValue as BaseBlockEntityTypeDefinition);
            BaseBlockEntityTypeDefinition typeDef = (data.GetArrayElementAtIndex(i).objectReferenceValue as BaseBlockEntityTypeDefinition);

            // If a type definition exists, draw its entity sprite as a texture.
            if (typeDef != null)
            {
                // Adjust the position to draw the sprite.
                newPosition.width -= soHandlePixelSize;
                Sprite entitySprite = typeDef.HelperSprite;
                EditorGUI.DrawTextureTransparent(newPosition, entitySprite.texture);
                newPosition.width += soHandlePixelSize;
            }

            x += cellWidth;

            // Move to the next row when necessary.
            if ((i + 1) % width == 0)
            {
                x = position.x;
                y += cellSize;
            }
        }
    }

    // Override the GetPropertyHeight method to determine the height of the property field.
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int height = property.FindPropertyRelative("Height").intValue;
        // Calculate the total height required for the property field.
        return EditorGUIUtility.singleLineHeight + cellSize * (height + 1); // Add extra height for spacing.
    }
}
#endif

