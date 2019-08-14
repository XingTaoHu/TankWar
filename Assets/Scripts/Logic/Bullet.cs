using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed = 100f;
    public GameObject explode;
    public float maxLiftTime = 2f;
    public float instantiateTime = 0f;

    //攻击方
    public GameObject attackTank;

    //爆炸音效
    public AudioClip explodeClip;

	// Use this for initialization
	void Start () {
        instantiateTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        //前进
        transform.position += transform.forward * speed * Time.deltaTime;
        //销毁自身
        if (Time.time - instantiateTime > maxLiftTime) {
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter(Collision collisionInfo)
    { 
        //打到自身
        if (collisionInfo.gameObject == attackTank)
            return;

        //爆炸效果
        GameObject explodeObj = (GameObject)Instantiate(explode, transform.position, transform.rotation);
        AudioSource audioSource = explodeObj.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.PlayOneShot(explodeClip);
        //销毁自身
        Destroy(gameObject);
        //击中tank
        Tank tank = collisionInfo.gameObject.GetComponent<Tank>();
        if (tank != null)
        {
            float att = GetAtt();
            tank.BeAttacked(att, attackTank);
        }
    }


    //计算攻击力
    private float GetAtt()
    {
        float att = 100 - (Time.time - instantiateTime) * 40;
        if (att < 1)
            att = 1;
        return att;
    }

}
