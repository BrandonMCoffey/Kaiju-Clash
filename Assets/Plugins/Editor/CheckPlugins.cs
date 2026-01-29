using System.Linq;
using UnityEditor;
using UnityEditor.Build;

[InitializeOnLoad]
public class FolderDefineChecker
{
    private const string TargetFile = "Assets/Plugins/Sirenix/Assemblies/Sirenix.OdinInspector.Attributes.xml";
    private const string DefineName = "ODIN_INSPECTOR";
    private static string[] RemoveDefines = { "ODIN_INSPECTOR", "ODIN_INSPECTOR_3", "ODIN_INSPECTOR_3_1", "ODIN_INSPECTOR_3_2", "ODIN_INSPECTOR_3_3", "ODIN_INSPECTOR_EDITOR_ONLY" };

    static FolderDefineChecker()
    {
        CheckAndSetDefine();
    }

    [MenuItem("Tools/Check Plugin Folder")]
    public static void ForceCheck()
    {
        CheckAndSetDefine();
    }

    private static void CheckAndSetDefine()
    {
        bool fileExists = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(TargetFile) != null;
        //UnityEngine.Debug.Log($"({fileExists}) Checking for file: {TargetFile}");

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);

        string defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
        string[] defineArray = defines.Split(';');

        bool hasDefine = defineArray.Contains(DefineName);

        if (fileExists && !hasDefine)
        {
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines + (defines.Length > 0 ? ";" : "") + DefineName);
            UnityEngine.Debug.Log($"<color=green>Folder found.</color> Added define: {DefineName}");
        }
        else if (!fileExists && hasDefine)
        {
            string newDefines = string.Join(";", defineArray.Where(d => !RemoveDefines.Contains(d)));
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefines);
            UnityEngine.Debug.Log($"<color=red>Folder not found.</color> Removed define: {DefineName}");
        }
    }
}
