#if UNITY_EDITOR

//Code based of crekri1's from https://discussions.unity.com/t/custom-editor-open-new-inspector-window-and-show-asset/854882
//Made by Euphie_E https://github.com/Euphie-E


/*
    Call ShowMe.Create(code, optional error msg, optional gameObject of the code)
*/

using UnityEditor;
using UnityEngine;

public class ShowMe : EditorWindow
{
    private Object asset;
    private string info = "Information about the error:\n";
    private Editor assetEditor;
    GameObject obj;
    GUIStyle style;
    static Dock dc;
    static bool open = false;


    public static ShowMe Create(Object asset)
    {
        return Create(asset,"",null);
    }
    public static ShowMe Create(Object asset, string error)
    {
        return Create(asset,error,null);
    }
    public static ShowMe Create(Object asset, GameObject gameObject)
    {
        return Create(asset,"",gameObject);
    }
    public static ShowMe Create(Object asset, string error, GameObject gameObject)
    {
        if(!open)
        {
            dc = CreateWindow<Dock>("Errors");
            open = true;
        } 
        var window = CreateWindow<ShowMe>($"{asset.name} | {asset.GetType().Name}",new System.Type[]{typeof(Dock)});
        window.obj = gameObject;
        window.asset = asset;
        window.assetEditor = Editor.CreateEditor(asset);
        window.info += error;
        window.style = new GUIStyle(EditorStyles.wordWrappedLabel)
        {
            fontSize = 15
            
        };
        dc.Add();
        return window;
    }

    private void OnGUI()
    {
        GUI.enabled = false;
        asset = EditorGUILayout.ObjectField("Asset", asset, asset.GetType(), true);
        GUI.enabled = true;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        assetEditor.OnInspectorGUI();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(info,style);
        if(obj != null && GUILayout.Button("Select")) 
            Select();
        EditorGUILayout.EndVertical();
    }
    void Select()
    {
        Selection.activeGameObject = obj;
    }
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            dc.Close();
            Close();
        }
    }
}
public class Dock : EditorWindow
{
    static int c;
    public void Add()
    {
        c++;
    }
    public void CClose()
    {
        Close();
    }
}

#endif