using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : MonoBehaviour
{
	[SerializeField] Transform target;

	private void Awake()
	{
		Update();
	}

	private void Update()
	{
		transform.position = target.position;
		transform.rotation = target.rotation;
	}
}
