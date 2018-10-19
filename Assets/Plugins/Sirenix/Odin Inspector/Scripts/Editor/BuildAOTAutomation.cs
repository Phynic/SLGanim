#if UNITY_5_6_OR_NEWER

namespace Sirenix.Serialization.Internal
{
    using Sirenix.Serialization;
    using UnityEditor;
    using UnityEditor.Build;
    using System.IO;

    public class PreBuildAOTAutomation : IPreprocessBuild
    {
        public int callbackOrder
        {
            get
            {
                return -1000;
            }
        }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (AOTGenerationConfig.Instance.AutomateBeforeBuilds
                && AOTGenerationConfig.Instance.AutomateForPlatforms != null
                && AOTGenerationConfig.Instance.AutomateForPlatforms.Contains(target))
            {
                AOTGenerationConfig.Instance.ScanProject();
                AOTGenerationConfig.Instance.GenerateDLL();
            }
        }
    }

    public class PostBuildAOTAutomation : IPostprocessBuild
    {
        public int callbackOrder
        {
            get
            {
                return -1000;
            }
        }

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (AOTGenerationConfig.Instance.AutomateBeforeBuilds
                && AOTGenerationConfig.Instance.AutomateForPlatforms != null
                && AOTGenerationConfig.Instance.AutomateForPlatforms.Contains(target)
                && AOTGenerationConfig.Instance.DeleteDllAfterBuilds)
            {
                Directory.Delete(AOTGenerationConfig.Instance.AOTFolderPath, true);
                File.Delete(AOTGenerationConfig.Instance.AOTFolderPath.TrimEnd('/', '\\') + ".meta");
                AssetDatabase.Refresh();
            }
        }
    }
}

#endif