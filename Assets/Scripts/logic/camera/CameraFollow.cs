using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	//距离
	private float distance = 15f;
	//距离范围
	private float maxDistance = 22f;
	private float minDistance = 8f;
	//距离变化速度
	private float zoomSpeed = 0.5f;

	//横向角度
	private float rot = 45f;
	//横向旋转速度
	private float rotSpeed = 15f;

	//纵向角度
	private float roll = 30f;
	//纵向角度范围
	private float maxRoll = 50f;
	private float minRoll = -10f;
	//纵向旋转速度
	private float rollSpeed = 15f;

	//目标物体
	private GameObject target;


	// Use this for initialization
	void Start () {
        //找到目标物体
        target = GameObject.Find("tank_dark");
        if(target != null)
		    SetTarget(target);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetTarget(GameObject target)
	{
		if (target.transform.Find("cameraPoint") != null)
		{
			this.target = target.transform.Find("cameraPoint").gameObject;
		}
		else
		{
			this.target = target;
		}
	}

	private void LateUpdate()
	{
		//目标跟随
		if (target == null)
			return;
		if (Camera.main == null)
			return;

        FollowTarget();

        Rotate();
        Roll();
        Zoom();
	}

    //跟随目标物体
    void FollowTarget()
    {
        //目标坐标
        Vector3 targetPos = target.transform.position;
        //用三角函数计算相机位置
        Vector3 cameraPos;
        float d = distance * Mathf.Cos(revertAngel(roll));
        float height = distance * Mathf.Sin(revertAngel(roll));
        cameraPos.x = targetPos.x + d * Mathf.Sin(revertAngel(rot));
        cameraPos.y = targetPos.y + height;
        cameraPos.z = targetPos.z - d * Mathf.Cos(revertAngel(rot));
        Camera.main.transform.position = cameraPos;
        //对准目标
        Camera.main.transform.LookAt(target.transform);
    }
    //计算角度
	private float revertAngel(float angle)
	{
		return angle * Mathf.PI * 2 / 360;
	}

    //相机横向旋转
	void Rotate()
	{
		float w = Input.GetAxis("Mouse X") * rotSpeed;
		rot -= w;
	}

    //相机纵向旋转
	void Roll()
	{
		float w = Input.GetAxis("Mouse Y") * rollSpeed;
		roll -= w;
		if (roll > maxRoll)
			roll = maxRoll;
		if (roll < minRoll)
			roll = minRoll;
	}

    //滚轮距离
	void Zoom()
	{
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (distance > minDistance)
				distance -= zoomSpeed;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (distance < maxDistance)
				distance += zoomSpeed;
		}
	}

}