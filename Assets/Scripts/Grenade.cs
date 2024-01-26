using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
	Rigidbody rb;
	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}
	public void Init(Vector3 vel)
	{
		transform.parent = null;
		rb.isKinematic = false;
		rb.velocity = vel;
	}
}
