using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceBoxWithLine : MonoBehaviour
{
	public GameObject lineFXObj;
	public BoxCollider box;
	public float lineDuration;

	public void DrawLineFX()
	{
		//I'm so sorry
		Vector3 p1 = box.transform.position + box.center + box.transform.right / 2f * box.size.x * box.transform.localScale.x + Vector3.down * box.size.y / 2f * box.transform.localScale.y + box.transform.forward * box.size.z / 2f * box.transform.localScale.z;
		Vector3 p2 = box.transform.position + box.center + box.transform.right / 2f * box.size.x * box.transform.localScale.x + Vector3.down * box.size.y / 2f * box.transform.localScale.y - box.transform.forward * box.size.z / 2f * box.transform.localScale.z;
		Vector3 p3 = box.transform.position + box.center - box.transform.right / 2f * box.size.x * box.transform.localScale.x + Vector3.down * box.size.y / 2f * box.transform.localScale.y - box.transform.forward * box.size.z / 2f * box.transform.localScale.z;
		Vector3 p4 = box.transform.position + box.center - box.transform.right / 2f * box.size.x * box.transform.localScale.x + Vector3.down * box.size.y / 2f * box.transform.localScale.y + box.transform.forward * box.size.z / 2f * box.transform.localScale.z;
		GameObject lineFXObjInst = GameObject.Instantiate(lineFXObj, transform.position, Quaternion.identity);
		LineRenderer lineFX = lineFXObjInst.GetComponent<LineRenderer>();
		Vector3[] points = { p1, p2, p3, p4 };
		lineFX.SetPositions(points);
		Destroy(lineFXObjInst, lineDuration);
	}
}
