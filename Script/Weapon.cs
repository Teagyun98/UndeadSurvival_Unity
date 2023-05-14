using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    float timer;
    Player player;

	private void Awake()
	{
        player = GameManager.instance.player;
	}

	void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
		{
            case 0:
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;
                if(timer > speed)
				{
                    timer = 0f;
                    Fire();
				}
                break;
		}

        if (Input.GetButtonDown("Jump"))
            LevelUp(20, 5);
    }

    public void LevelUp(float damage, int count)
	{
        this.damage = damage * Character.Damage; // this.는 받아온 값이 아닌 이 스크립트 안의 변수를 지정하기 위해 사용
        this.count += count;

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
	{
        //초기 세팅
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for(int i = 0; i < GameManager.instance.pool.prefabs.Length; i++)
		{
            if(data.Projectile == GameManager.instance.pool.prefabs[i])
			{
                prefabId = i;
                break;
			}
		}

        switch(id)
		{
            case 0:
                speed = -150 * Character.WeaponSpeed;
                Batch();
                break;
            default:
                speed = 0.5f * Character.WeaponRate;
                break;
		}

        //Hand set
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); //오브젝트 내에 모든 ApplyGear함수 호출, 나중에 추가되는 무기도 기어 기어 레벨이 적용 되도록
                                                                                      //SendMessageOptions.DontRequireReceiver : 호출 가능한 오브젝트가 없어도 상관없다.
    }

    void Batch()
	{
        for(int index = 0; index < count; index++)
		{
            Transform bullet;
            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
			else
			{
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform; // bullet의 부모 weapon.transform으로 변경
            }
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity; //Rotation의 초기화값

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 is Infinity per
		}
	}

    void Fire()
	{
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized; // 정규화

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);//dir 방향으로 회전
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
