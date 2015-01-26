using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundScript : MonoBehaviour {

	public float normScrollRate;
	public float slowScrollRate;

	private float totalRate = 0f;
	private RawImage bkgSprite;
	private float scrollRate;
	private Rect tempRect = new Rect(0,0,1,1);
	private bool canScroll = true;

	// Use this for initialization
	void Start () {
		bkgSprite = gameObject.GetComponent<RawImage>();
		scrollRate = normScrollRate;
	}
	
	// Update is called once per frame
	void Update () {
		if(canScroll) {
			totalRate += Time.deltaTime * scrollRate;
			tempRect.x = totalRate;
			bkgSprite.uvRect = tempRect;
		}
	}

	public void SetScrollRate(float lerp) {
		scrollRate = Mathf.Lerp(normScrollRate, slowScrollRate, lerp);
	}

	public void CanScroll(bool b) { canScroll = b; }
}
