using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairUI : MonoBehaviour
{
	[SerializeField] RectTransform top;
	[SerializeField] RectTransform bottom;
	[SerializeField] RectTransform left;
	[SerializeField] RectTransform right;
	[SerializeField] float ratio;
	[SerializeField] float lerpSpeed = 10f;

	WeaponHolder weaponHolder;
	float curPos;

	private void Start()
	{
		weaponHolder = FieldSceneFC.Player.GetComponent<PlayerAction>().WeaponHolder;
		curPos = 25f;
	}

	private void Update()
	{
		float targetPos = 25 + weaponHolder.Accuracy * ratio;
		//print(targetPos);
		curPos = Mathf.Lerp(curPos, targetPos, Time.deltaTime * lerpSpeed);

		top.anchoredPosition = new Vector2(top.anchoredPosition.x, curPos);
		bottom.anchoredPosition = new Vector2(bottom.anchoredPosition.x, -curPos);
		left.anchoredPosition = new Vector2(-curPos, left.anchoredPosition.y);
		right.anchoredPosition = new Vector2(curPos, right.anchoredPosition.y);
	}
}