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
		//ĳ���� ���� ��ġ, ���� ������, ĳ���� ����, ĳ���� ����, ��� ���̾�
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
			float curDiff = Vector3.Distance(mypos, targetPos); // �� �Ÿ��� ����

			if(curDiff < diff) // ���� 100������ ���� Ÿ�� �� ���� ����� Ÿ���� ��ȯ
			{
				diff = curDiff;
				result = target.transform;
			}
		}

		return result;
	}
}
