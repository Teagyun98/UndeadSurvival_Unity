using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // inputSystem Assest 사용을 위해 추가

public class Player : MonoBehaviour
{

    public Vector2 inputVec;
    public float speed;
	public Scanner scanner;
	public Hand[] hands;
	public RuntimeAnimatorController[] animCon;

	SpriteRenderer spriter;
    Rigidbody2D rigid;
	Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
		spriter = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		scanner = GetComponent<Scanner>();
		hands = GetComponentsInChildren<Hand>(true); // 비활성화된 오브젝트도 초기화 가능
	}
	private void OnEnable()
	{
		speed *= Character.Speed;
		anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
	}

	// Update is called once per frame
	private void OnMove(InputValue value)
	{
        inputVec = value.Get<Vector2>();
    }

	private void FixedUpdate()
	{
		if (!GameManager.instance.isLive)
			return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
	}

	private void LateUpdate()
	{
		if (!GameManager.instance.isLive)
			return;

		anim.SetFloat("Speed", inputVec.magnitude);

		if(inputVec.x != 0)
		{
			spriter.flipX = inputVec.x < 0;
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (!GameManager.instance.isLive)
			return;

		GameManager.instance.health -= Time.deltaTime * 10;

		if (GameManager.instance.health < 0)
		{
			for (int index = 2; index < transform.childCount; index++)
			{
				transform.GetChild(index).gameObject.SetActive(false);
			}
			anim.SetTrigger("Dead");
			GameManager.instance.GameOver();
		}
	}
}
