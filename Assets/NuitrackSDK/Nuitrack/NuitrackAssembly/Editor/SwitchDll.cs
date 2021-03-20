using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

[InitializeOnLoad]
public class SwitchDll : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    static string pathIL2CPP = "Assets/NuitrackSDK/Nuitrack/NuitrackAssembly/IL2CPP/nuitrack.net.dll";
    static string pathMONO = "Assets/NuitrackSDK/Nuitrack/NuitrackAssembly/nuitrack.net.dll";
    static string pathIOS = "Assets/NuitrackSDK/Nuitrack/NuitrackAssembly/IOS/nuitrack.net.dll";

    static SwitchDll()
    {
        Check();
    }

    [MenuItem("Nuitrack/Auto switch dll")]
    public static void Check()
    {
        PluginImporter pluginIL2CPP = (PluginImporter)PluginImporter.GetAtPath(pathIL2CPP);
        PluginImporter pluginMONO = (PluginImporter)PluginImporter.GetAtPath(pathMONO);
        PluginImporter pluginIOS = (PluginImporter)PluginImporter.GetAtPath(pathIOS);

        if (pluginIL2CPP == null)
        {
            Debug.LogError("Il2cpp Dll not found: " + pathIL2CPP);
            return;
        }

        if (pluginMONO == null)
        {
            Debug.LogError("Mono Dll not found: " + pathMONO);
            return;
        }

        if (pluginIOS == null)
        {
            Debug.LogError("IOS Dll not found: " + pathIOS);
            return;
        }

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

        ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(buildTargetGroup);

        Debug.Log("Current Scripting Backend " + PlayerSettings.GetScriptingBackend(buildTargetGroup) + "  Target:" + buildTargetGroup);

        bool useStructureSensor = false;

        if (buildTargetGroup == BuildTargetGroup.iOS)
        {
#if use_structure_sensor
            useStructureSensor = true;
#else
            Debug.Log("If you need to use Structure Sensor add use_structure_sensor to Scripting Define Symbols in Player Settings...");
#endif
            Debug.Log("Used Structure Sensor: " + useStructureSensor);
        }

        if (buildTargetGroup == BuildTargetGroup.iOS)
        {
            SwitchDll.SwitchCompatibleWithPlatform(pluginMONO, false);

            if (useStructureSensor)
            {
                SwitchDll.SwitchCompatibleWithPlatform(pluginIL2CPP, false);
                SwitchDll.SwitchCompatibleWithPlatform(pluginIOS, true);
            }
            else
            {
                SwitchDll.SwitchCompatibleWithPlatform(pluginIL2CPP, true);
                SwitchDll.SwitchCompatibleWithPlatform(pluginIOS, false);
            }
        }
        else if((buildTargetGroup == BuildTargetGroup.Android || buildTargetGroup == BuildTargetGroup.Standalone) && backend == ScriptingImplementation.IL2CPP)
        {
            SwitchDll.SwitchCompatibleWithPlatform(pluginIL2CPP, true);
            SwitchDll.SwitchCompatibleWithPlatform(pluginMONO, false);
            SwitchDll.SwitchCompatibleWithPlatform(pluginIOS, false);
        }
        else
        {
            SwitchDll.SwitchCompatibleWithPlatform(pluginIL2CPP, false);
            SwitchDll.SwitchCompatibleWithPlatform(pluginMONO, true);
            SwitchDll.SwitchCompatibleWithPlatform(pluginIOS, false);
        }
    }

    public static void SwitchCompatibleWithPlatform(PluginImporter plugin, bool value)
    {
        if (value)
            Debug.Log("Platform " + EditorUserBuildSettings.activeBuildTarget + ". Switch Nuitrack dll to " + plugin.assetPath);

        plugin.SetCompatibleWithAnyPlatform(false);
        plugin.SetCompatibleWithPlatform(BuildTarget.iOS, value);
        plugin.SetCompatibleWithPlatform(BuildTarget.Android, value);
        plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, value);
        plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, value);
        plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, value);
        plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, value);
        plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, value);
        plugin.SetCompatibleWithEditor(value);
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Check();
    }
}
