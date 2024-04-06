using MyProject.Core.Settings;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelData))]
public class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Get the target LevelSettings scriptable object.
        LevelData levelData = (LevelData)target;

        // Get the grid start layout from the LevelGroundTypeSettings.
        GridStartLayout gridStartLayout = levelData.LevelGroundTypeSettings.gridStartLayout;
        GridStartLayout gridMillitaryStartLayout = levelData.MilitaryBaseTypeSettings.gridStartLayout;

        // Check if the width or height of the grid start layout needs to be updated.
        if (gridStartLayout.Width != levelData.LevelGroundTypeSettings.Width ||
            gridStartLayout.Height != levelData.LevelGroundTypeSettings.Height)
        {
            // Update the grid start layout with new dimensions based on LevelGroundTypeSettings.
            levelData.LevelGroundTypeSettings.gridStartLayout =
                new GridStartLayout((int)levelData.LevelGroundTypeSettings.Width,
                                    (int)levelData.LevelGroundTypeSettings.Height);
            levelData.MilitaryBaseTypeSettings.gridStartLayout= new GridStartLayout((int)levelData.LevelGroundTypeSettings.Width,
                                    (int)levelData.LevelGroundTypeSettings.Height);

            levelData.LevelMountainSettings.gridStartLayout = new GridStartLayout((int)levelData.LevelGroundTypeSettings.Width,
                                    (int)levelData.LevelGroundTypeSettings.Height);
        }
    }
}
#endif
