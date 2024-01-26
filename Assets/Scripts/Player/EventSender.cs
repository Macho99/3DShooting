using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSender : MonoBehaviour
{
	PlayerAction playerAction;
	WeaponHolder weaponHolder;

	private void Start()
	{
		playerAction = GetComponentInParent<PlayerAction>();
		weaponHolder = playerAction.WeaponHolder;
	}

	public void HandUp()
	{
		weaponHolder.HandUp();
	}

	public void DrawGun()
	{
		weaponHolder.DrawGun();
	}

	public void TossGrenade()
	{
		weaponHolder.TossGrenade();
	}
}
