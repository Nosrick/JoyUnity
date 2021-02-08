using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

//This script was modified from https://forum.unity.com/threads/executing-first-scene-in-build-settings-when-pressing-play-button-in-editor.157502/#post-4152451
[InitializeOnLoad]
public class AutoPlayModeSceneSetup
{
    [MenuItem("Tools/Set As First Scene", false, 0)]
    public static void SetAsFirstScene()
    {
 
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        List<string> scenePaths = editorBuildSettingsScenes.Select(i => i.path).ToList();
 
        //Add the scene to build settings if not already there; place as the first scene
        if (!scenePaths.Contains(EditorSceneManager.GetActiveScene().path))
        {
            editorBuildSettingsScenes.Insert(0, new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true));
 
        }
        else
        {
 
            //Reference the current scene
            EditorBuildSettingsScene scene = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
 
            int index = -1;
 
            //Loop and find index from scene; we are doing this way cause IndexOf returns a -1 index for some reason
            for(int i = 0; i < editorBuildSettingsScenes.Count; i++)
            {
                if(editorBuildSettingsScenes[i].path == scene.path)
                {
                    index = i;
 
                }
 
            }
 
            if (index != 0)
            {
                //Remove from current index
                editorBuildSettingsScenes.RemoveAt(index);
 
                //Then place as first scene in build settings
                editorBuildSettingsScenes.Insert(0, scene);
 
            }
 
        }
 
        //copy arrays back to build setting scenes
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
 
    }
 
    static AutoPlayModeSceneSetup()
    {
 
        EditorBuildSettings.sceneListChanged += SceneListChanged;
 
    }
 
    static void SceneListChanged()
    {
 
        // Ensure at least one build scene exist.
        if (EditorBuildSettings.scenes.Length == 0)
        {
            return;
        }
 
        //Reference the first scene
        SceneAsset theScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
 
        // Set Play Mode scene to first scene defined in build settings.
        EditorSceneManager.playModeStartScene = theScene;
 
    }
 
}