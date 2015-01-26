using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireSpawnScript : MonoBehaviour {

	public GameObject fire;
	public Vector2 randOffset;
	public int minFireCount;
	public int maxFireCount;

	private Vector2 tempVec = Vector2.zero;

	public void StartDelay(float delay) {
		StartCoroutine(SpawnFire(delay));
	}

	IEnumerator SpawnFire(float delay) {
		while (delay > 0f) {
			yield return null;
			delay -= Time.deltaTime;
			if(GameManager.Instance().getCoroutine() == GameCoroutineType.Inactive) { yield break; }
		}
		if(GameManager.Instance().getCoroutine() != GameCoroutineType.Inactive) {
			for(int i=0; i<Random.Range(minFireCount, maxFireCount); i++) {
				tempVec.x = Random.Range(-randOffset.x, randOffset.x);
				tempVec.y = Random.Range(-randOffset.y, randOffset.y);
				GameObject obj = Instantiate(fire) as GameObject;
				obj.transform.SetParent(transform);
				((RectTransform)obj.transform).anchoredPosition = tempVec;
			}
		}
	}
}
