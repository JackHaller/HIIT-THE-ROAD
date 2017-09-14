using UnityEngine;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System;

public class PipeSystemController : MonoBehaviour {

    public GlobalSettings globalSettings;

	private NamedPipeServer PServer = null;
	private Thread sideThread = null;
	private bool wantToStop = false;

	public string ClientFileName;
	public bool EnableCamera = true;
	public Vector2 PositionOffset { get; private set; }

	public bool CameraInitialised = false;

	//these variables define the centre pixel of the depth camera stream (used to calculate how far away the sent coordinates are from the centre)
	private int midX = 160;
	private int midY = 120;

	// Use this for initialization
	void Start () {
        EnableCamera = globalSettings.EnableCamera;
		PositionOffset = new Vector3 (0.0f, 0.0f, 0.0f);
		if (EnableCamera) {
			sideThread = new Thread (SideThreadMethod);
			sideThread.Start ();
		}
	}

	void OnApplicationQuit() {
		if (sideThread != null && sideThread.IsAlive) {
			wantToStop = true;
		}
	}

	void SideThreadMethod() {
		//Start the servers
		PServer = new NamedPipeServer(@"\\.\pipe\ExergameNamedPipe1007992",0);
		PServer.Start();
		//delay for half a second to allow the servers to finish initialising before trying to start the client process
		Thread.Sleep (500);

		//Start the client executable then restore focus to the game
		System.Diagnostics.Process clientProcess = new System.Diagnostics.Process ();
		clientProcess.StartInfo.FileName = ClientFileName;
		clientProcess.StartInfo.UseShellExecute = false;
		clientProcess.StartInfo.CreateNoWindow = true;
		clientProcess.Start ();

		while (!wantToStop) {
			Thread.Sleep(15);	//Throw in a sleep because we really don't need to check this a thousand times a second
			ParseMessage(PServer.LastMessage);
		}

		Debug.Log ("Closing Pipe System.");
		PServer.StopServer ();
		clientProcess.Kill ();
		clientProcess.Close ();
	}

	void ParseMessage(string message) {
		if (message != null && message != string.Empty) {
			//our expected message format is number,number, in a range of 0-319, 0-239 (incl)
			//e.g: 125,60
			int separatorIndex = message.IndexOf(',');
			int readX = int.Parse(message.Substring(0, separatorIndex));
			int readY = int.Parse(message.Substring(separatorIndex + 1, message.Length - (separatorIndex + 1)));
			int offsetX = midX - readX;
			int offsetY = midY - readY;
			float x = offsetX == 0 ? 0.0f : (float)offsetX / (float)midX;
			float y = offsetY == 0 ? 0.0f : (float)offsetY / (float)midY;
			PositionOffset = new Vector2(x, y);
			CameraInitialised = true;		//We only need to set this once, but the cost of checking it every frame vs setting it every frame is pretty much the same
		}
	}
}
