using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    Rigidbody2D rigid;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	public void Init(float damage, int per, Vector3 dir)
	{
        this.damage = damage;
        this.per = per;

		if(per >= 0)
		{
			//velocity = 속도
			rigid.velocity = dir * 15f;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Enemy") || per == -100)
			return;

		per--;

		if(per < 0)
		{
			rigid.velocity = Vector2.zero; // 재활용을 대비하여 초기화
			gameObject.SetActive(false);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!collision.CompareTag("Area") || per == -100)
			return;

		gameObject.SetActive(false);
	}
}
