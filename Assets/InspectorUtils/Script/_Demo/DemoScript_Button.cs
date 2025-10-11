using UnityEngine;

namespace IUtil._Demo
{
	[IUtil.IUtilable]
    public class DemoScript_Button : MonoBehaviour
	{
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

	}
}