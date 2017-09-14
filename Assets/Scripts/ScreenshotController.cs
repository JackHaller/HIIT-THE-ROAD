using UnityEngine;
using System.Collections;

public class ScreenshotController : MonoBehaviour {

	private int ScreenShotInt = 1;
	
	void Update() {
		if(Input.GetKeyDown(KeyCode.F2)) {
			ScreenCapture.CaptureScreenshot("Screenshot" + ScreenShotInt + ".png");
			ScreenShotInt++;
		}
	}
}
