# 🚀 Unity Inspector Utils

Unity Inspector Utils is a collection of powerful custom attributes designed to improve the Unity Inspector ( or Editor ) experience, making it more dynamic and user-friendly.

<p align="center">
  <image src="https://github.com/user-attachments/assets/be5e2dd6-9337-4870-bb46-e0bb3ca3fc31" width=28%></image>
  <image src="https://github.com/user-attachments/assets/5de49d1d-aa3a-409a-8e70-5b44182ebb93" width=50%></image>
</p>

### Features

- **Enhanced Readability & Organization** – Keeps the Inspector clean by grouping and hiding fields dynamically.
- **Improved Workflow Efficiency** – Reduces the need for custom editor scripts, making development faster and more intuitive.
- **Easy to Use & Integrate** – Simply add attributes to your fields without modifying the Unity Editor itself.
- **Open-Source & Free to Use** – Completely free, with full access to the source code. You can modify or extend its features as needed.

### Compatibility

| Unity Version | Compatibility |
|:-------------:|:-------------:|
| **6000.0.32f1** |:heavy_check_mark:|
| **2022.3.47f1** |:heavy_check_mark:|
| **2022.3.20f1** |:heavy_check_mark:|
| **2022.3.10f1** |:heavy_check_mark:|
| **2019.4.0f1** |:heavy_check_mark:|


## How To Use

### Import

Go to release tab (https://github.com/qweasfjbv/UnityInspectorUtils/releases)
![image](https://github.com/user-attachments/assets/a4b43bbf-bd49-458f-b1c7-6ab6f8a21f5a)

Download the `.unitypackage` file and import it into Unity.

![image](https://github.com/user-attachments/assets/97a7d06c-8be0-492e-9e06-3222ba58deaa)

## First Of All

```cs
[IUtil.IUtilable]
public class MyClass : MonoBehaviour { /* ... */ }
```
To avoid conflicts with other libraries, classes that use `TabGroup`, `FoldoutGroup`, or `Button` must have the `[IUtilable]` attribute.


### Tab / Foldout Group

```cs
[TabGroup("Main", "Tab1")]
public int mainTab1Int;
public Vector3 mainTab1Vec;

[TabGroup("Sub", "Tab1")]
public int subTab1Int;
public float subTab1Float;

[FoldoutGroup("Fold1")]
public float foldFloat1;
public float foldFloat2;

[TabGroup("Sub", "Tab1")]
public string subTab1String;

[TabGroup("Sub", "Tab2")]
public float subTab2Float;
public int subTab2Int;

[TabGroup("Main", "Tab2")]
public string mainTab2String;
public int mainTab2Int;

[TabGroup("Main", "Tab3")]
public string mainTab3String;
public int mainTab3Int;
```
<p align="center">
  <image src="https://github.com/user-attachments/assets/be5e2dd6-9337-4870-bb46-e0bb3ca3fc31"></image>
</p>

<br>


### If Attributes

```cs
public bool isReadonly;
[ReadOnlyIf(nameof(isReadonly))]
public int readonlyValue;

public bool isHide;
[HideIf(nameof(isHide))]
public float hideValue;

public bool isShow;
[ShowIf(nameof(isShow))]
public float showValue;
```

<p align="center">
  <image src="https://github.com/user-attachments/assets/5ca9ee90-650c-4440-95de-fd226dce849e" width="30%"></image>
</p>

```cs

[System.Serializable]
public class NestedClass
{
    public bool IsShow;
    [ShowIf(nameof(IsShow))] public string showString;
}

public List<NestedClass> nestedClass = new();
```

<p align="center">
  <image src="https://github.com/user-attachments/assets/90abb850-d404-48a9-91f3-d6533381be86" width="30%"></image>
</p>


### Popup Option

```cs
public int[] intOptions = new int[] { 1, 2, 3 };
public float[] floatOptions = new float[] { 5.1f, 6.3f, 7.3f };
public string[] stringOptions = new string[] { "Option1", "Option2", "Option3" };

[SerializeField, PopupOption(nameof(intOptions))]
private int selectInt;

[SerializeField, PopupOption(nameof(floatOptions), 1)]
public float selectFloat;

[SerializeField, PopupOption(nameof(stringOptions), 2)]
public string selectString;
```

<p align="center">
  <image src="https://github.com/user-attachments/assets/d99c9117-b0b0-4513-8765-6b51917ebfee" width="30%"></image>
</p>


### Button

```cs
		public string Func1Param1;
		[Space(10)]
		public int Func2Param1;
		public float Func2Param2;

		[Button]
		public void Func0()
		{
			Debug.Log($"Func0 Excuted");
		}

		[Button(nameof(Func2Param1), nameof(Func2Param2))]
		public void Func2(int param1, float param2)
		{
			Debug.Log($"Func2 Executed! \nParam1 : {param1}, Param2 : {param2}");
		}
```

<p align="center">
  <image src="https://github.com/user-attachments/assets/fe10d20b-27e0-4fcb-bf21-87057727783d" width=50%></image>
</p>

---

### Project Folder Custom

1. Open folder custom window

<p align="center">
  <image src="https://github.com/user-attachments/assets/0fd684ab-acc5-48f3-801a-bec146fb00d2" width=50%></image>
</p>

2. You can custom folder here.

<p align="center">
  <image src="https://github.com/user-attachments/assets/5de49d1d-aa3a-409a-8e70-5b44182ebb93" width=50%></image>
</p>

