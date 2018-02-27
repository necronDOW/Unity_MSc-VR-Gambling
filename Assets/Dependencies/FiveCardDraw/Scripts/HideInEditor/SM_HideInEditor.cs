using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SM_HideInEditor : MonoBehaviour
{
    private bool lastHideAllButInitialScene;
    public bool hideAllButInitialScene = true;

    private int lastFirstScene;
    private SceneManager_v2 _sceneMngr;
    private SceneManager_v2 sceneMngr
    {
        get {
            if (_sceneMngr == null) {
                _sceneMngr = GetComponent<SceneManager_v2>();
            }

            return _sceneMngr;
        }
    }
    
    private HideInEditor[] hideScripts
    {
        get {
            List<HideInEditor> hideScriptsList = new List<HideInEditor>();

            for (int i = 0; i < sceneMngr.scenes.Length; i++) {
                AddHideScript(sceneMngr.scenes[i].container, ref hideScriptsList);
                AddHideScript(sceneMngr.scenes[i].hudContainer, ref hideScriptsList);
            }

            return hideScriptsList.ToArray();
        }
    }

    private void Start()
    {
        lastHideAllButInitialScene = hideAllButInitialScene;
        lastFirstScene = sceneMngr.firstScene;

        foreach (HideInEditor h in hideScripts) {
            h.OverrideHidden(true);
        }
#if UNITY_EDITOR
        EditorApplication.playmodeStateChanged = UpdateHidden;
#endif
    }

    private void Update()
    {
        if (RequiresVisualsUpdate())
            UpdateHidden();
    }

    private void OnApplicationQuit()
    {
        UpdateHidden();
    }
    
    public void UpdateHidden()
    {
        if (hideAllButInitialScene) {
            InternalScene_v2 firstScene = sceneMngr.scenes[sceneMngr.firstScene];

            foreach (HideInEditor h in hideScripts) {
                h.OverrideHidden(h.gameObject == firstScene.container || h.gameObject == firstScene.hudContainer);
            }
        }
        else {
            foreach (HideInEditor h in hideScripts) {
                h.OverrideHidden(true);
            }
        }
    }

    private void AddHideScript(GameObject target, ref List<HideInEditor> initializedList)
    {
        if (!target)
            return;

        HideInEditor hideScript = target.GetComponent<HideInEditor>();
        if (hideScript)
            initializedList.Add(hideScript);
    }

    private bool RequiresVisualsUpdate()
    {
        bool requiresUpdate = (lastHideAllButInitialScene != hideAllButInitialScene)
            || (lastFirstScene != sceneMngr.firstScene);

        lastHideAllButInitialScene = hideAllButInitialScene;
        lastFirstScene = sceneMngr.firstScene;

        return requiresUpdate;
    }
}
