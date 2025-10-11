using System;
using UnityEngine;

namespace IUtil
{

	[System.AttributeUsage(AttributeTargets.Class)]
	public class IUtilableAttribute : System.Attribute { }

	#region Grouping

	/// <summary>
	/// Attribute class for [TabGroup]
	/// 
	/// - Usage
	///		[TabGroup(GroupName, TabName, HeaderFontSize, HeaderColorType)]
	///		public float m_float1;
	///		public float m_float2;
	///		...
	///		
	/// - Descript
	///		When a specific tab is selected, only the variables and functions included in that tab are displayed.
	///		It can contain [FoldoutGroup].
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field)]
	public class TabGroupAttribute : PropertyAttribute
	{
		public string GroupName { get; }
		public string TabName { get; }
		public int HeaderFontSize { get; }
		public ColorType HeaderColorType { get; }

		public TabGroupAttribute(string groupName, string tabName) : this(groupName, tabName, 14) { }
		public TabGroupAttribute(string groupName, string tabName, int headerFontSize) : this(groupName, tabName, headerFontSize, ColorType.White) { }
		
		public TabGroupAttribute(string groupName, string tabName, int headerFontSize, ColorType headerColorType)
		{
			GroupName = groupName;
			TabName = tabName;
			HeaderFontSize = headerFontSize;
			HeaderColorType = headerColorType;
		}
	}

	/// <summary>
	/// Attribute class for [FoldoutGroup]
	/// 
	/// - Usage
	///		[FoldoutGroup(HeaderName, HeaderSize, HeaderColor)]
	///		public float m_float1;
	///		public float m_float2;
	///		...
	///		
	/// - Descript
	///		When the header is collapsed, the variables are hidden; when expanded, they become visible.
	///		It can be part of [TabGroup], but it cannot contain [TabGroup].
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field)]
	public class FoldoutGroupAttribute : PropertyAttribute

	{
		public string Name { get; }
		public int FontSize { get; }
		public ColorType ColorType { get; }


		public FoldoutGroupAttribute(string name) : this(name, 14, ColorType.White) { }
		public FoldoutGroupAttribute(string name, int fontSize) : this(name, fontSize, ColorType.White) { }

		public FoldoutGroupAttribute(string name, int fontSize, ColorType type)
		{
			Name = name;
			FontSize = fontSize;
			ColorType = type;
		}
	}

	#endregion


	#region Function Utility

	/// <summary>
	/// Attribute class for [Button]
	/// 
	/// - Usage
	///		int param1, param2 ... ;
	///		
	///		[Button(nameof(param1), nameof(param2), ...)]
	///		public void Function(int param1, int param2, ...) { }
	///		
	/// - Descript
	///		You can display a button in the inspector to execute the corresponding function.
	///		It can also get parameters (by name) upto 6.
	/// 
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Method)]
	public class ButtonAttribute : Attribute
	{
		public string[] ParameterNames { get; }

		public ButtonAttribute()
		{
			ParameterNames = new string[] { };
		}
		public ButtonAttribute(string param1)
		{
			ParameterNames = new string[] { param1 };
		}
		public ButtonAttribute(string param1, string param2)
		{
			ParameterNames = new string[] { param1, param2 };
		}
		public ButtonAttribute(string param1, string param2, string param3)
		{
			ParameterNames = new string[] { param1, param2, param3 };
		}
		public ButtonAttribute(string param1, string param2, string param3, string param4)
		{
			ParameterNames = new string[] { param1, param2, param3, param4 };
		}
		public ButtonAttribute(string param1, string param2, string param3, string param4, string param5)
		{
			ParameterNames = new string[] { param1, param2, param3, param4, param5 };
		}
		public ButtonAttribute(string param1, string param2, string param3, string param4, string param5, string param6)
		{
			ParameterNames = new string[] { param1, param2, param3, param4, param5, param6 };
		}
	}

	#endregion


	#region Variable Utility

	/// <summary>
	/// Attribute class for [ShowIf]
	/// 
	/// - Usage
	///		[ShowIf(nameof(Condition))]
	///		public float m_float;
	///		
	/// - Descript
	///		It can hide or reveal variables based on a condition.
	///			true  : reveal
	///			false : hide
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field)]
	public class ShowIfAttribute : PropertyAttribute
	{
		public string Condition { get; }

		public ShowIfAttribute(string condition)
		{
			Condition = condition;
		}
	}

	/// <summary>
	/// Attribute class for [HideIf]
	/// 
	/// - Usage
	///		[HideIf(nameof(Condition))]
	///		public float m_float;
	///		
	/// - Descript
	///		It can hide or reveal variables based on a condition.
	///			true  : hide
	///			false : reveal
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field)]
	public class HideIfAttribute : PropertyAttribute
	{
		public string Condition { get; }

		public HideIfAttribute(string condition)
		{
			Condition = condition;
		}
	}

	/// <summary>
	/// Attribute class for [ReadOnlyIf]
	/// 
	/// - Usage
	///		[ReadOnlyIf(nameof(Condition))]
	///		public float m_float;
	///		
	/// - Descript
	///		It can readonly or default property based on a condition.
	///			true  : ReadOnly
	///			false : Default property
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field)]
	public class ReadOnlyIfAttribute : PropertyAttribute
	{
		public string Condition { get; }

		public ReadOnlyIfAttribute(string condition)
		{
			Condition = condition;
		}
	}

	/// <summary>
	/// Attribute class for [PopupOption]
	/// 
	/// - Usage
	///		[PopupOption(nameof(OptionArray, DefaultIndex))]
	///		public float m_float;
	///		
	/// - Descript
	///		You can set this variable using only the values from the OptionArray through the Popup.
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field)]
	public class PopupOptionAttribute : PropertyAttribute
	{
		public string ArrayName { get; }
		public int DefaultIndex { get; }

		public PopupOptionAttribute(string arrayName) : this(arrayName, 0) { }

		public PopupOptionAttribute(string arrayName, int defaultIndex)
		{
			ArrayName = arrayName;
			DefaultIndex = defaultIndex;
		}
	}

	/// <summary>
	/// Attribute class for [HelpBox]
	/// 
	/// - Usage
	///		[HelpBox(Content, MessageType)]
	///		public float m_float;
	///		
	/// - Descript
	///		It displays helpbox below variable.
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field)]
	public class HelpBoxAttribute : PropertyAttribute
	{
		public string Content { get; }
		public IUtil.MessageType MessageType { get; }

		public HelpBoxAttribute(string content) : this(content, IUtil.MessageType.None) { }

		public HelpBoxAttribute(string content, MessageType mType)
		{
			Content = content;
			MessageType = mType;
		}
	}

	/// <summary>
	/// Attribute class for [ReadOnly]
	/// 
	/// - Usage
	///		[ReadOnly(OnlyInRuntime)]
	///		public float m_float;
	///		
	/// - Descript
	///		It can prevent the variable's value from being changed in the editor inspector.
	///		If OnlyInRuntime is true, block value only in runtime.
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field)]
	public class ReadOnlyAttribute : PropertyAttribute
	{
		public bool OnlyInRuntime { get; }

		public ReadOnlyAttribute() : this(false) { }
		public ReadOnlyAttribute(bool onlyInRuntime)
		{
			OnlyInRuntime = onlyInRuntime;
		}
	}

	#endregion

}