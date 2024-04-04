using MyProject.Core.Settings;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelSettings))]
public class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Get the target LevelSettings scriptable object.
        LevelSettings levelSettings = (LevelSettings)target;

        // Get the grid start layout from the LevelGroundTypeSettings.
        GridStartLayout gridStartLayout = levelSettings.LevelGroundTypeSettings.gridStartLayout;
        GridStartLayout gridMillitaryStartLayout = levelSettings.MilitaryBaseTypeSettings.gridStartLayout;

        // Check if the width or height of the grid start layout needs to be updated.
        if (gridStartLayout.Width != levelSettings.LevelGroundTypeSettings.Width ||
            gridStartLayout.Height != levelSettings.LevelGroundTypeSettings.Height)
        {
            // Update the grid start layout with new dimensions based on LevelGroundTypeSettings.
            levelSettings.LevelGroundTypeSettings.gridStartLayout =
                new GridStartLayout((int)levelSettings.LevelGroundTypeSettings.Width,
                                    (int)levelSettings.LevelGroundTypeSettings.Height);
            levelSettings.MilitaryBaseTypeSettings.gridStartLayout= new GridStartLayout((int)levelSettings.LevelGroundTypeSettings.Width,
                                    (int)levelSettings.LevelGroundTypeSettings.Height);

            levelSettings.LevelMountainSettings.gridStartLayout = new GridStartLayout((int)levelSettings.LevelGroundTypeSettings.Width,
                                    (int)levelSettings.LevelGroundTypeSettings.Height);
        }
    }
}
#endif
