using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		if (GameManager.Instance == null)
		{
			GameObject obj = new GameObject("GameManager");
			obj.AddComponent<GameManager>();
		}
	}
}