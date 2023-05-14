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
        this.damage = damage * Character.Damage; // this.�� �޾ƿ� ���� �ƴ� �� ��ũ��Ʈ ���� ������ �����ϱ� ���� ���
        this.count += count;

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
	{
        //�ʱ� ����
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

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); //������Ʈ ���� ��� ApplyGear�Լ� ȣ��, ���߿� �߰��Ǵ� ���⵵ ��� ��� ������ ���� �ǵ���
                                                                                      //SendMessageOptions.DontRequireReceiver : ȣ�� ������ ������Ʈ�� ��� �������.
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
                bullet.parent = transform; // bullet�� �θ� weapon.transform���� ����
            }
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity; //Rotation�� �ʱ�ȭ��

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
        dir = dir.normalized; // ����ȭ

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);//dir �������� ȸ��
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
