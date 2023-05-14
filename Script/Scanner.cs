using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

	private void FixedUpdate()
	{
		//캐스팅 시작 위치, 원의 반지름, 캐스팅 바향, 캐스팅 길이, 대상 레이어
		targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
		nearestTarget = GetNearest();
	}

	Transform GetNearest()
	{
		Transform result = null;
		float diff = 100;

		foreach(RaycastHit2D target in targets)
		{
			Vector3 mypos = transform.position;
			Vector3 targetPos = target.transform.position;
			float curDiff = Vector3.Distance(mypos, targetPos); // 두 거리의 차이

			if(curDiff < diff) // 범위 100안으로 들어온 타겟 중 가장 가까운 타겟을 반환
			{
				diff = curDiff;
				result = target.transform;
			}
		}

		return result;
	}
}
