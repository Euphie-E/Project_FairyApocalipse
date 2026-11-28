using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MinimapMaterialEditor : EditorWindow
{
    private GameObject objetoPai;
    private Material material;
    private bool incluirInativos = true;

    [MenuItem("Tools/Minimap/Aplicar Materiais")]
    public static void Abrir()
    {
        GetWindow<MinimapMaterialEditor>(
            "Minimap Materials"
        );
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(
            "Aplicar materiais em massa",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        objetoPai = (GameObject)EditorGUILayout.ObjectField(
            "Objeto Pai",
            objetoPai,
            typeof(GameObject),
            true
        );

        material = (Material)EditorGUILayout.ObjectField(
            "Material",
            material,
            typeof(Material),
            false
        );

        incluirInativos = EditorGUILayout.Toggle(
            "Incluir Inativos",
            incluirInativos
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Usar objeto selecionado"))
        {
            objetoPai = Selection.activeGameObject;
        }

        if (objetoPai == null)
            return;

        MeshRenderer[] renderers =
            objetoPai.GetComponentsInChildren<MeshRenderer>(
                incluirInativos
            );

        EditorGUILayout.HelpBox(
            $"Mesh Renderers encontrados: {renderers.Length}",
            MessageType.Info
        );

        bool valido =
            !Application.isPlaying &&
            !EditorUtility.IsPersistent(objetoPai) &&
            objetoPai.scene.IsValid() &&
            PrefabStageUtility.GetCurrentPrefabStage() == null;

        if (!valido)
        {
            EditorGUILayout.HelpBox(
                "Selecione um objeto da cena, fora do Prefab Mode.",
                MessageType.Warning
            );
        }

        using (new EditorGUI.DisabledScope(!valido))
        {
            if (GUILayout.Button("Selecionar todos os Renderers"))
            {
                GameObject[] objetos =
                    new GameObject[renderers.Length];

                for (int i = 0; i < renderers.Length; i++)
                    objetos[i] = renderers[i].gameObject;

                Selection.objects = objetos;
            }

            using (new EditorGUI.DisabledScope(material == null))
            {
                if (GUILayout.Button(
                    "Aplicar material em todos",
                    GUILayout.Height(35)))
                {
                    Aplicar(renderers);
                }
            }
        }
    }

    private void Aplicar(MeshRenderer[] renderers)
    {
        bool confirmar = EditorUtility.DisplayDialog(
            "Aplicar materiais",
            $"Aplicar o material em {renderers.Length} Mesh Renderers?",
            "Aplicar",
            "Cancelar"
        );

        if (!confirmar)
            return;

        Undo.IncrementCurrentGroup();

        int grupo = Undo.GetCurrentGroup();

        Undo.SetCurrentGroupName("Aplicar materiais do minimapa");

        int alterados = 0;

        foreach (MeshRenderer renderer in renderers)
        {
            if (EditorUtility.IsPersistent(renderer))
                continue;

            Material[] materiais = renderer.sharedMaterials;

            if (materiais.Length == 0)
                continue;

            Undo.RecordObject(renderer, "Alterar materiais");

            for (int i = 0; i < materiais.Length; i++)
            {
                materiais[i] = material;
            }

            renderer.sharedMaterials = materiais;

            PrefabUtility.RecordPrefabInstancePropertyModifications(
                renderer
            );

            EditorUtility.SetDirty(renderer);

            EditorSceneManager.MarkSceneDirty(
                renderer.gameObject.scene
            );

            alterados++;
        }

        Undo.CollapseUndoOperations(grupo);

        EditorUtility.DisplayDialog(
            "Concluido",
            $"{alterados} Mesh Renderers atualizados.",
            "OK"
        );
    }
}