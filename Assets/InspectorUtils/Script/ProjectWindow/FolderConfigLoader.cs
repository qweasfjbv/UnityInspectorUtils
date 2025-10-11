using System.Collections.Generic;
using UnityEditor;
using IUtil.SO;
using UnityEngine;
using IUtil.Utils;
using System;
using System.Linq;

namespace IUtil.ProjectWindow
{

#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class FolderConfigLoader
    {
        public static Dictionary<string, FolderConfigElement> ConfigDict { get; private set; } = new();
        public static Dictionary<FolderColorType, Texture2D> ColoredFolders { get; private set; } = new();
        public static Dictionary<FolderIconType, Texture2D> Icons { get; private set; } = new();
        private static FolderConfig configSO = null;

		static FolderConfigLoader()
        {
            LoadAll();

            EditorApplication.projectChanged -= LoadAll;
            EditorApplication.projectChanged += LoadAll;
        }

        public static void SaveAll()
        {
            if (configSO == null) return;
			ConfigDict = ConfigDict
	            .Where(kv => AssetDatabase.IsValidFolder(kv.Key))
	            .ToDictionary(kv => kv.Key, kv => kv.Value);

			configSO.Elements = ConfigDict.Values.ToList();

			EditorUtility.SetDirty(configSO);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
        public static void LoadAll()
        {
			RefreshConfigs();
            LoadFolderColorTextures();
            LoadIconTextures();
        }

        private static void RefreshConfigs()
        {
            ConfigDict.Clear();
			configSO = AssetDatabase.LoadAssetAtPath<FolderConfig>(Constants.PATH_FOLDER_CONFIG);

            if (configSO == null)
            {
                IUtilDebug.ConfigurationNullError("Folder Config", Constants.PATH_FOLDER_CONFIG);
                return;
            }
			foreach (var entry in configSO.Elements)
			{
                if (!AssetDatabase.IsValidFolder(entry.Path)) continue;
				if (entry != null && !string.IsNullOrEmpty(entry.Path))
                {
                    ConfigDict[entry.Path] = entry;
                }
			}
		}
        private static void LoadFolderColorTextures()
        {
            ColoredFolders.Clear();

            foreach (FolderColorType colorType in System.Enum.GetValues(typeof(FolderColorType)))
            {
                if (colorType == FolderColorType.None)
                    continue;

                string fileName = colorType.ToString();
                string[] guids = AssetDatabase.FindAssets($"{fileName} t:Texture2D", new[] { Constants.PATH_FOLDER_TEXTURE });

                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                    if (texture != null)
                    {
                        ColoredFolders[colorType] = texture;
                    }
                }
            }
        }
        private static void LoadIconTextures()
        {
            Icons.Clear();

            foreach (FolderIconType iconType in System.Enum.GetValues(typeof(FolderIconType)))
            {
                if (iconType < 0)
                    continue;

                Icons[iconType] = EditorGUIUtility.IconContent(iconType.GetIconName()).image as Texture2D;
            }
        }

        /// <summary>
        /// Save path info in Scriptable Object,
        ///     and Re-Load all.
        /// </summary>
        public static void SetCustomFolderConfig<T>(string path, int idx) where T : Enum
        {
            if (!ConfigDict.ContainsKey(path)) ConfigDict[path] = new FolderConfigElement(path);
            if (typeof(T) == typeof(FolderIconType)) ConfigDict[path].IconType = (FolderIconType)idx;
            if (typeof(T) == typeof(FolderColorType)) ConfigDict[path].ColorType = (FolderColorType)idx;

            SaveAll();
            LoadAll();
		}

        /// <summary>
        /// Save folder color in Scriptable Object,
        ///     and Re-Load all.
        /// </summary>
        public static void SetCustomFolderColor(string path, Color color)
        {
            if (!ConfigDict.ContainsKey(path)) ConfigDict[path] = new FolderConfigElement(path);
            ConfigDict[path].CustomFolderColor = color;

            SaveAll();
            LoadAll();
        }

        /// <summary>
        /// Get FolderConfigElement by folder path
        /// </summary>
        public static FolderConfigElement GetFolderConfigElement(string path)
        {
            if (!ConfigDict.ContainsKey(path)) return null;
            return ConfigDict[path];
        }

        /// <summary>
        /// Remove folder config element by folder path
        /// </summary>
        public static void ResetCustom(string path)
        {
            if (!ConfigDict.ContainsKey(path)) return;
            ConfigDict.Remove(path);

			SaveAll();
			LoadAll();
		}
	}
#endif
}
