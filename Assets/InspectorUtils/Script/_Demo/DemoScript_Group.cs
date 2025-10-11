using UnityEngine;

namespace IUtil._Demo
{
	[IUtil.IUtilable]
	public class DemoScript_Group : MonoBehaviour
	{

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

	}
}