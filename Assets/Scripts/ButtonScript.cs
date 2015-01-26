using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	public WindowTypes setWindow;

	public void OnPlay() {
		WindowManager.Instance().SetWindow(setWindow);
	}

}
