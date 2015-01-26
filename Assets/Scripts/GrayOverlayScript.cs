using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrayOverlayScript : MonoBehaviour {

	public float minAlpha;
	public float maxAlpha;

	private Image img;

	// Use this for initialization
	void Start () {
		img = gameObject.GetComponent<Image>();
	}
	
	public void SetAlpha(float lerp) {
		img.color = new Color(0, 0, 0, Mathf.Lerp(minAlpha, maxAlpha, lerp));
	}
}
