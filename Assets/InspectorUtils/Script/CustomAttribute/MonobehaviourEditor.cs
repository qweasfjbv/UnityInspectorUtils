#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IUtil.Utils
{
	[CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : Editor
    {
        #region Internal Classes 
        private class PropertyData
        {
            public SerializedProperty Property;
            public TabGroupAttribute TabAttr;
            public FoldoutGroupAttribute FoldAttr;
        }

        private class TabGroupInfo
        {
            public List<string> Tabs = new List<string>();
            public string ParentGroup;
            public string ParentTab;
        }

        private class TabGroupContext
        {
            public string GroupName;
            public string TabName;
        }
        #endregion

        #region Local Variables
        /** Every variables to draw in monobehavioru editor. **/
        private List<PropertyData> propertyDataList = new List<PropertyData>();

        /** Dictionary to save tab/foldout states. **/
        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
        private Dictionary<string, string> activeTabs = new Dictionary<string, string>();
        
        /** Variables to trace properties' parent group to decide whether draw or not. **/
        private Stack<TabGroupContext> contextStack = new Stack<TabGroupContext>();
        private Dictionary<string, TabGroupInfo> tabGroups = new Dictionary<string, TabGroupInfo>();
        private Dictionary<string, TabGroupContext> foldoutGroups = new Dictionary<string, TabGroupContext>();
        
        private TabGroupAttribute curTabAttr = null;
        private FoldoutGroupAttribute curFoldAttr = null;
		private bool isTargetCustomInspectable;

		#endregion

		private void OnEnable()
		{
			var targetType = target.GetType();
			isTargetCustomInspectable = System.Attribute.IsDefined(targetType, typeof(IUtilableAttribute));
		}

		public override void OnInspectorGUI()
        {
            if (!isTargetCustomInspectable)
            {
                DrawDefaultInspector();
                return;
            }

			serializedObject.Update();
            CollectProperties();
            DrawProperties();
            DrawFunctionButton();
			serializedObject.ApplyModifiedProperties();
        }

        private void Init()
        {
            propertyDataList.Clear();
            tabGroups.Clear();
            foldoutGroups.Clear();
			contextStack.Clear();
            curTabAttr = null;
            curFoldAttr = null;
		}

		/// <summary>
		/// Collect properties to draw
		/// </summary>
		private void CollectProperties()
        {
            Init();

            SerializedProperty prop = serializedObject.GetIterator();
            prop.NextVisible(true);
                
            do
            {
                var tabAttr = GetTabGroupAttribute(prop);
                var foldAttr = GetFoldoutGroupAttribute(prop);

                RegisterGroup(tabAttr);
                RegisterGroup(foldAttr);

                if (tabAttr != null)
                {
                    UpdateTabContext(tabAttr);
                    curTabAttr = tabAttr;
                    curFoldAttr = null;
                }

                if (foldAttr != null)
                {
                    curFoldAttr = foldAttr;
                }

                propertyDataList.Add(new PropertyData
                {
                    Property = prop.Copy(),
                    FoldAttr = curFoldAttr,
                    TabAttr = curTabAttr,
                });
            } while (prop.NextVisible(false));

		}

        private void UpdateTabContext(TabGroupAttribute tabAttr)
        {
            contextStack.Push(new TabGroupContext
            {
                GroupName = tabAttr.GroupName,
                TabName = tabAttr.TabName
            });
        }

		/// <summary>
		/// Register TabGroup on tabGroups
		/// </summary>
		private void RegisterGroup(TabGroupAttribute tabAttr)
        {
            if (tabAttr == null) return;
            if (!tabGroups.ContainsKey(tabAttr.GroupName))
			{
				if (contextStack.Count > 0)
				{
                    var parent = contextStack.Peek();
					tabGroups[tabAttr.GroupName] = new TabGroupInfo
					{
						ParentGroup = parent.GroupName,
						ParentTab = parent.TabName,
					};
				}
                else
				{
					tabGroups[tabAttr.GroupName] = new TabGroupInfo
					{
						ParentGroup = null,
						ParentTab = null
					};
				}
            }

            if (!tabGroups[tabAttr.GroupName].Tabs.Contains(tabAttr.TabName))
            {
                tabGroups[tabAttr.GroupName].Tabs.Add(tabAttr.TabName);
            }
        }

        /// <summary>
        /// Register FoldoutGroup on foldoutGroups
        /// </summary>
        private void RegisterGroup(FoldoutGroupAttribute foldAttr)
        {
            if (foldAttr == null) return;

            if (!foldoutGroups.ContainsKey(foldAttr.Name))
			{
				if (contextStack.Count > 0)
				{
					var parent = contextStack.Peek();
					foldoutGroups[foldAttr.Name] = new TabGroupContext
					{
						GroupName = parent.GroupName,
						TabName = parent.TabName
					};
				}
            }
        }

        /// <summary>
        /// Draw Properties or Groups
        /// </summary>
        private void DrawProperties()
        {
            HashSet<string> processedGroups = new HashSet<string>();
            HashSet<string> processedFoldouts = new HashSet<string>();

            foreach (var data in propertyDataList)
			{
				if (data.Property.name == "m_Script")
				{
					GUI.enabled = false;
					EditorGUILayout.PropertyField(data.Property, true);
					GUI.enabled = true;
					continue;
				}

				TabGroupAttribute tabAttr = data.TabAttr;
				FoldoutGroupAttribute foldAttr = data.FoldAttr;

				if (foldAttr != null && !foldoutStates.ContainsKey(foldAttr.Name))
					foldoutStates[foldAttr.Name] = false;

                if (tabAttr != null && !activeTabs.ContainsKey(tabAttr.GroupName))
                    activeTabs[tabAttr.GroupName] = tabGroups[tabAttr.GroupName].Tabs[0];

                // Except unable property
				if (!IsAbleToDrawGroup(data.TabAttr)) continue;
				if (!IsAbleToDrawGroup(data.FoldAttr)) continue;

				// Draw Header
				if (tabAttr != null && !processedGroups.Contains(tabAttr.GroupName))
				{
					DrawTabHeader(tabAttr.GroupName);
					processedGroups.Add(tabAttr.GroupName);
				}

				if (foldAttr != null)
				{
                    if (!processedFoldouts.Contains(foldAttr.Name))
					{
						DrawFoldoutGroup(foldAttr);
						processedFoldouts.Add(foldAttr.Name);
					}
					bool isActive = IsFoldActive(foldAttr.Name);
					if (!isActive) continue;
				}

				EditorGUILayout.PropertyField(data.Property, true);
            }
        }

		private bool IsAbleToDrawGroup(FoldoutGroupAttribute attr)
		{
			if (attr == null) return true;

			return IsFoldGroupActive(attr.Name);
		}
		private bool IsAbleToDrawGroup(TabGroupAttribute attr)
        {
            if (attr == null) return true;

			return IsTabGroupActive(attr.GroupName, attr.TabName);
        }

        private bool IsFoldGroupActive(string groupName)
		{
			if ((groupName == null || !foldoutGroups.TryGetValue(groupName, out var groupInfo)))
				return true;

			if (!string.IsNullOrEmpty(groupInfo.GroupName))
			{
				return true;
			}

			return IsTabGroupActive(groupInfo.GroupName, groupInfo.TabName);
		}
        private bool IsFoldActive(string groupName)
        {
            if (!foldoutStates.TryGetValue(groupName, out var groupInfo)) return false;
            return groupInfo;
        }
		private bool IsTabGroupActive(string parentGroup, string parentTab)
        {
            if ((string.IsNullOrEmpty(parentGroup) || !tabGroups.TryGetValue(parentGroup, out var groupInfo)))
                return true;

            bool isActive = IsTabActive(parentGroup, parentTab);
            if (!isActive) return false;

			if (string.IsNullOrEmpty(groupInfo.ParentGroup))
            {
                return true;
            }

			return IsTabGroupActive(groupInfo.ParentGroup, groupInfo.ParentTab);
		}
		private bool IsTabActive(string group, string tab)
		{
            activeTabs.TryGetValue(group, out string activeT);
			return activeTabs.TryGetValue(group, out string activeTab) && activeTab == tab;
		}

        /// <summary>
        /// Draw Tab Header
        /// </summary>
        private void DrawTabHeader(string groupName)
        {
            if (!tabGroups.TryGetValue(groupName, out var groupInfo)) return;

            if (!activeTabs.ContainsKey(groupName))
                activeTabs[groupName] = groupInfo.Tabs.FirstOrDefault();

            int tmpFontSize = 16;

			GUIStyle labelStyle = EditorStyles.boldLabel;
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.fontSize = tmpFontSize;
            labelStyle.normal.textColor = IUtil.Utils.Constants.PALLETE[(int)IUtil.ColorType.White];

            GUILayout.Space(tmpFontSize / 4);
			GUILayout.Label(groupName, labelStyle);
            GUILayout.Space(tmpFontSize / 4);

			GUILayout.BeginHorizontal();
            foreach (string tab in groupInfo.Tabs)
            {
                bool isActive = activeTabs[groupName] == tab;
                if (GUILayout.Toggle(isActive, tab, EditorStyles.toolbarButton))
                {
                    if (!isActive) activeTabs[groupName] = tab;
                }
            }
            GUILayout.EndHorizontal();
        }

		/// <summary>
		/// Draw Foldout Header
		/// </summary>
		private bool DrawFoldoutGroup(FoldoutGroupAttribute attr)
        {
            if (!foldoutStates.ContainsKey(attr.Name))
                foldoutStates[attr.Name] = false;

            GUIStyleState onState = new GUIStyleState
            {
                textColor = Constants.PALLETE[(int)attr.ColorType]
            };

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = attr.FontSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Constants.PALLETE[(int)attr.ColorType] },
                alignment = TextAnchor.MiddleLeft,
                onFocused = onState,
                onNormal = onState
            };

            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, attr.FontSize * 1.5f);
            foldoutStates[attr.Name] = EditorGUI.Foldout(rect, foldoutStates[attr.Name], attr.Name, true, foldoutStyle);
            return foldoutStates[attr.Name];
        }

		private FoldoutGroupAttribute GetFoldoutGroupAttribute(SerializedProperty prop)
		{
			var targetObj = prop.serializedObject.targetObject;
			var field = targetObj.GetType().GetField(prop.propertyPath,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			return field?.GetCustomAttribute<FoldoutGroupAttribute>();
		}
		private TabGroupAttribute GetTabGroupAttribute(SerializedProperty prop)
		{
			var targetObj = prop.serializedObject.targetObject;
			var field = targetObj.GetType().GetField(prop.propertyPath,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			return field?.GetCustomAttribute<TabGroupAttribute>();
		}

        private void DrawFunctionButton()
		{
			MonoBehaviour targetScript = (MonoBehaviour)target;
			MethodInfo[] methods = targetScript.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MethodInfo method in methods)
			{
				ButtonAttribute buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
				if (buttonAttr == null) continue;

				string buttonLabel = method.Name;
				if (GUILayout.Button(buttonLabel))
				{
					TryInvokeMethod(targetScript, method, buttonAttr);
				}
			}
		}
		private void TryInvokeMethod(MonoBehaviour targetScript, MethodInfo method, ButtonAttribute buttonAttr)
		{
			ParameterInfo[] parameters = method.GetParameters();
			object[] args = new object[parameters.Length];

			if (parameters.Length != buttonAttr.ParameterNames.Length)
			{
                IUtilDebug.ParameterCountError("Button", method.Name);
				return;
			}

			for (int i = 0; i < parameters.Length; i++)
			{
				string paramName = buttonAttr.ParameterNames[i];
				FieldInfo field = targetScript.GetType().GetField(paramName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (field == null)
				{
                    IUtilDebug.NoFieldError("Button", paramName);
					return;
				}

				if (field.FieldType != parameters[i].ParameterType)
				{
					IUtilDebug.TypeError("Button", field.FieldType.ToString());
					return;
				}

				args[i] = field.GetValue(targetScript);
			}

			method.Invoke(targetScript, args);
		}
	}
}
#endif