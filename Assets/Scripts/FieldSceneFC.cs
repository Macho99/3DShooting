using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSceneFC : MonoBehaviour
{
	private static FieldSceneFC instance;
	private static GameObject player;
	public static FieldSceneFC Instance { get { return instance; } }
	public static GameObject Player
	{
		get
		{
			if(player == null)
			{
				player = GameObject.FindGameObjectWithTag("Player");
			}
			return player;
		}
	}

	private void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
		}
		instance = this;
	}

	private void OnDestroy()
	{
		if(instance == this)
		{
			instance = null;
			player = null;
		}
	}
}