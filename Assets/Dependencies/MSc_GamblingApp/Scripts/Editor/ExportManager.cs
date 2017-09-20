using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExportManager : EditorWindow
{
    [MenuItem("Window/Export Manager")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ExportManager));
    }
    
    private string buildName = "Diamond Pairs";
    private readonly string[] buildExt = new string[3] { " (None)", " (Game)", " (Pause)" };
    private readonly string[] appIdExt = new string[3] { "dp_none", "dp_game", "dp_pause" };
    private Vector2 scrollPosition;

    public Configs config;

    private void OnGUI()
    {
        SerializedObject serializedConfigs = new SerializedObject(config);
        SerializedProperty diceStopSignalSequence = serializedConfigs.FindProperty("diceSceneStopSignalTurns");
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Configuration Settings", EditorStyles.boldLabel);
        config = (Configs)EditorGUILayout.ObjectField(config, typeof(Configs), false);

        if (config)
        {
            Configs.Intervention lastIntervention = config.intervention;
            config.intervention = (Configs.Intervention)EditorGUILayout.EnumPopup("Intervention Type", config.intervention);

            if (config.intervention != lastIntervention)
            {
                config.interventionSceneIndex = (int)config.intervention;

                if (config.intervention == Configs.Intervention.Game)
                    config.interventionTransition = SceneMngr.TransitionMode.Lerp;
                else config.interventionTransition = SceneMngr.TransitionMode.Instant;

                PlayerSettings.productName = buildName + buildExt[(int)config.intervention];
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.liamwilson." + appIdExt[(int)config.intervention]);
            }

            #region Scene Configurations
            GUILayout.Label("Cards Configuration", EditorStyles.boldLabel);
            config.cardSceneInterventionTurn = EditorGUILayout.IntField("Intervention Turn", config.cardSceneInterventionTurn);
            config.cardSceneDealSpeed = EditorGUILayout.FloatField("Deal Speed", config.cardSceneDealSpeed);
            config.cardSceneOrderFile = EditorGUILayout.ObjectField("Deal Order CSV", config.cardSceneOrderFile, typeof(Object), false);

            GUILayout.Label("Dice Configuration", EditorStyles.boldLabel);
            config.diceSceneMaxReactionTime = EditorGUILayout.FloatField("Max Reaction Time", config.diceSceneMaxReactionTime);
            EditorGUILayout.PropertyField(diceStopSignalSequence, new GUIContent("Stop Signal Sequence"), true);
            config.diceSceneOrderFile = EditorGUILayout.ObjectField("Dice Order CSV", config.diceSceneOrderFile, typeof(Object), false);

            GUILayout.Label("Freeze-out Configuration", EditorStyles.boldLabel);
            config.freezeSceneTime = EditorGUILayout.FloatField("Freeze-out Time", config.freezeSceneTime);
            #endregion
        }

        GUILayout.Label("Export Settings", EditorStyles.boldLabel);
        buildName = EditorGUILayout.TextField("Name", buildName);
        if (GUILayout.Button("Export"))
            Build();

        GUILayout.EndScrollView();

        serializedConfigs.ApplyModifiedProperties();
        EditorUtility.SetDirty(config);
    }

    private void Build()
    {
        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = AllScenes();
        buildOptions.locationPathName = "Build";
        buildOptions.target = BuildTarget.Android;
        buildOptions.options = BuildOptions.AcceptExternalModificationsToPlayer;
        BuildPipeline.BuildPlayer(buildOptions);
        
        ShowInExplorer(Application.dataPath + "/../" + buildOptions.locationPathName + "/" + PlayerSettings.productName);
    }

    private void ShowInExplorer(string path)
    {
        path = path.Replace(@"/", @"\");
        System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
    }

    private string[] AllScenes()
    {
        EditorBuildSettingsScene[] editorScenes = EditorBuildSettings.scenes;
        string[] scenes = new string[editorScenes.Length];

        for (int i = 0; i < scenes.Length; i++)
            scenes[i] = editorScenes[i].path;

        return scenes;
    }
}
