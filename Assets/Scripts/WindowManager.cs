using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour {

	//Singleton set-up
	private static WindowManager instance;
	public static WindowManager Instance() {
		return instance;
	}
	
	[SerializeField]
	private List<Window> WindowList = new List<Window>();
	private WindowTypes CurrentWindow;

	//Ensure singleton on creation
	private void Awake() {
		if(instance != null) { Destroy(gameObject); return; }
		instance = this;
	}

	//Sets the current window to WindowType
	public void SetWindow(WindowTypes windowType) { 
		if (CurrentWindow == windowType) { return; }
		foreach(Window curWindow in WindowList) { 
			curWindow.gameObject.SetActive(curWindow.windowType == windowType);
		}
		CurrentWindow = windowType; 
	}

	//Gets currently active window
	public WindowTypes GetActiveWindow() { return CurrentWindow; }

}
