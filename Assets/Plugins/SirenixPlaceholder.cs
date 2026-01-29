#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

[InitializeOnLoad]
public class FolderDefineChecker
{
    private const string TargetFolder = "Assets/Plugins/Sirenix";
    private const string DefineName = "ODIN_INSPECTOR";

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
        bool folderExists = AssetDatabase.IsValidFolder(TargetFolder);

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);

        string defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
        string[] defineArray = defines.Split(';');

        bool hasDefine = defineArray.Contains(DefineName);

        if (folderExists && !hasDefine)
        {
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines + (defines.Length > 0 ? ";" : "") + DefineName);
            Debug.Log($"<color=green>Folder found.</color> Added define: {DefineName}");
        }
        else if (!folderExists && hasDefine)
        {
            string newDefines = string.Join(";", defineArray.Where(d => d != DefineName));
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefines);
            Debug.Log($"<color=red>Folder not found.</color> Removed define: {DefineName}");
        }
    }
}
#endif

#if !ODIN_INSPECTOR
using System;

namespace Sirenix.OdinInspector
{
    public class HideIfAttribute : Attribute
    {
        public HideIfAttribute(string memberName) { }
    }

    public class ShowIfAttribute : Attribute
    {
        public ShowIfAttribute(string memberName) { }
    }

    public class InlineEditorAttribute : Attribute
    {
        public InlineEditorAttribute() { }
    }

    public class TableListAttribute : Attribute
    {
        public TableListAttribute() { }
    }

    public class ValueDropdownAttribute : Attribute
    {
        public ValueDropdownAttribute(string memberName) { }
    }

    public class ReadOnlyAttribute : Attribute
    {
        public ReadOnlyAttribute() { }
    }

    public class TitleAttribute : Attribute
    {
        public TitleAttribute(string title) { }
    }

    public class InfoBoxAttribute : Attribute
    {
        public InfoBoxAttribute(string message) { }
    }

    public class PropertySpaceAttribute : Attribute
    {
        public PropertySpaceAttribute(float space) { }
    }

    public class EnumToggleButtonsAttribute : Attribute
    {
        public EnumToggleButtonsAttribute() { }
    }

    public class FoldoutGroupAttribute : Attribute
    {
        public FoldoutGroupAttribute(string groupName) { }
    }

    public class SuffixLabelAttribute : Attribute
    {
        public SuffixLabelAttribute(string label) { }
    }

    public class MinMaxSliderAttribute : Attribute
    {
        public MinMaxSliderAttribute(float min, float max) { }
    }

    public class ColorPaletteAttribute : Attribute
    {
        public ColorPaletteAttribute(string paletteName) { }
    }

    public class HideLabelAttribute : Attribute
    {
        public HideLabelAttribute() { }
    }

    public class MultiLinePropertyAttribute : Attribute
    {
        public MultiLinePropertyAttribute(int lines) { }
    }

    public class RequiredAttribute : Attribute
    {
        public RequiredAttribute() { }
    }

    public class GUIColorAttribute : Attribute
    {
        public GUIColorAttribute(float r, float g, float b) { }
    }

    public class ButtonSizes
    {
        public const int Small = 0;
        public const int Medium = 1;
        public const int Large = 2;
    }

    public class ButtonStyle
    {
        public const int Default = 0;
        public const int Box = 1;
        public const int Foldout = 2;
    }

    public enum IconAlignment
    {
        Left,
        Right,
        Center
    }

    public enum SdfIconType
    {
        None,
        Plus,
        Minus,
        Refresh,
        Play,
        Stop,
        Gear,
        Trash,
        Folder,
        Search,
        Eye,
        EyeOff
    }

    public class ButtonAttribute : Attribute
    {
        public string Name;
        public ButtonStyle Style;
        public bool Expanded;
        public bool DisplayParameters = true;
        public bool DirtyOnClick = true;
        public SdfIconType Icon;
        private int buttonHeight;
        private bool drawResult;
        private bool drawResultIsSet;
        private bool stretch;
        private IconAlignment buttonIconAlignment;
        private float buttonAlignment;

        public ButtonAttribute() {}
        public ButtonAttribute(ButtonSizes size) { }
        public ButtonAttribute(int buttonSize) { }
        public ButtonAttribute(string name) { }
        public ButtonAttribute(string name, ButtonSizes buttonSize) { }
        public ButtonAttribute(string name, int buttonSize) { }
        public ButtonAttribute(ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(int buttonSize, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(ButtonSizes size, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(string name, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(string name, ButtonSizes buttonSize, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(string name, int buttonSize, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(SdfIconType icon, IconAlignment iconAlignment) { }
        public ButtonAttribute(SdfIconType icon) { }
        public ButtonAttribute(SdfIconType icon, string name) { }
    }
}
#endif