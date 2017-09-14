using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class LogController : MonoBehaviour {
	
	private string logFileName = null;
	private StreamWriter logFile = null;
	private float time = 0.0f;
	
	public BikeController bikeController;
	public ResistanceController resistanceController;
	public Generator generator;
	
	// Use this for initialization
	void Start () {
		time = 0.0f;
		//Create the log file
		logFileName = DetermineLogFileName();
		logFile = new StreamWriter(File.Create(Environment.CurrentDirectory + "/Logs/" + logFileName));
		WriteHeader();
		InvokeRepeating("WriteData", 0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void WriteData () {
		if (logFile != null) {
			time += 0.5f;
			int speed = bikeController.RPM;
			int hr = bikeController.heartRate;
			int resistance = resistanceController.Resistance;
			string block = generator.GetBlockInfo();
			logFile.WriteLine(string.Format("{0},{1},{2},{3},{4}", time, speed, hr, resistance, block));
		}
	}
			
	void WriteHeader() {
		logFile.WriteLine("Elapsed Time (s),Speed,Heart Rate,Resistance,Block");
	}
	
	string DetermineLogFileName() {
		return string.Format("Game Log {0}.csv", Guid.NewGuid());
	}
		
	public void Finish() {
		CancelInvoke();
		logFile.Close();
		logFile = null;
	}
}
