using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCSpawnScript : MonoBehaviour {

	public GameObject NPC;
	public Vector2 randOffset;
	public int minNPCCount;
	public int maxNPCCount;

	private Vector2 tempVec = Vector2.zero;

	public void StartDelay(float t) {
		StartCoroutine(SpawnNPC(t));
	}

	IEnumerator SpawnNPC(float t) {
		while (t > 0f) {
			yield return null;
			t -= Time.deltaTime;
			if(GameManager.Instance().getCoroutine() == GameCoroutineType.Inactive) { yield break; }
		}
		if(GameManager.Instance().getCoroutine() != GameCoroutineType.Inactive) {
			float tLimit = Random.Range(GameManager.Instance().minNPCDropTime, GameManager.Instance().maxNPCDropTime);
			for(int i=0;i < Random.Range(minNPCCount, maxNPCCount);i++) {
				tempVec.x = Random.Range(-randOffset.x, randOffset.x);
				tempVec.y = Random.Range(-randOffset.y, randOffset.y);
				GameObject obj = Instantiate(NPC) as GameObject;
				obj.transform.SetParent(transform);
				((RectTransform)obj.transform).anchoredPosition = tempVec;
				obj.GetComponent<NPCScript>().InitNPC(i, tLimit);
			}

			float timeTillSlow = Random.Range(GameManager.Instance().minSlowTime, GameManager.Instance().maxSlowTime);
			t = 0;
			while (t < timeTillSlow) {
				t += Time.deltaTime;
				tLimit -= Time.deltaTime;
				yield return null;
			}
			GameManager.Instance().StartTransition();
		}
	}
}
