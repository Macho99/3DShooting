using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	//private static DataManager dataManager;
	//private static ResourceManager resourceManager;
	//private static InventoryManager inventoryManager;
	//private static UIManager uiManager;
	public static GameManager Instance { get { return instance; } }
	//public static DataManager Data { get { return dataManager; } }
	//public static ResourceManager Resource { get { return resourceManager; } }
	//public static InventoryManager Inven { get { return inventoryManager; } }
	//public static UIManager UI { get { return uiManager; } }

	[HideInInspector] public UnityEvent<bool> OnFocus;
	[HideInInspector] public UnityEvent OnFpsChange;
	
	public bool IsFocus { get; private set; }
	public int fps;
	private int frameCnt;
	private float timeCnt;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
		InitManagers();

		OnFpsChange = new UnityEvent();
		OnFocus = new UnityEvent<bool>();
		frameCnt = 0;
		timeCnt = 0f;
	}

	private void Update()
	{
		frameCnt++;
		timeCnt += Time.deltaTime;
		if (timeCnt > 1f)
		{
			timeCnt -= 1f;
			fps = frameCnt;
			frameCnt = 0;
			OnFpsChange?.Invoke();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		OnFocus?.Invoke(focus);
	}

	private void InitManagers()
	{
		//resourceManager = new GameObject("ResourceManager").AddComponent<ResourceManager>();
		//resourceManager.transform.parent = transform;
		//dataManager = new GameObject("DataManager").AddComponent<DataManager>();
		//dataManager.transform.parent = transform;
		//inventoryManager = new GameObject("InventoryManager").AddComponent<InventoryManager>();
		//inventoryManager.transform.parent = transform;
		//uiManager = new GameObject("UIManager").AddComponent<UIManager>();
		//uiManager.transform.parent = transform;
	}
}