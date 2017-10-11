//Sends values to the Arduino to trigger button presses on the bike

using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.IO;

public class ResistanceController : MonoBehaviour {

	public GlobalSettings globalSettings;

	private SerialPort port = null;
	
	private int baud = 9600;
	private string portName = "COM4";
	
	public BikeController bikeController;
	
	private bool requiresReset = false;

    public int Resistance { get; private set; }

    public int FanSpeed { get; private set; }
	
	// Use this for initialization
	void Start () {
        Resistance = 5;
		portName = globalSettings.ResistancePort;
		Initialise();

	}
	
	void Update() {
        if (requiresReset) {
			Reset ();
		}
	}

    void OnApplicationQuit()
    {
        Close();
    }
	
	public void Initialise() {
		try {
			port = new SerialPort(portName, baud);
			port.Open();
			requiresReset = true;
		}
		catch (IOException e) {
			Debug.Log ("Error initialising connection to Arduino for resistance: " + e.ToString());	
		}
	}
	
	public void Close() {
        SetFanSpeed(0);
		port.Close();	
		port = null;
	}
	
	public void SetResistance(int resistance) 
    {
        //If the bike is not in the appropriate state, don't fiddle with the resistance
        if (!(bikeController.status == 0x09 || bikeController.status == 0x19 || bikeController.status == 0x89))
        {
            //Debug.Log (string.Format("Cannot set resistance, bike in wrong state: {0}", bikeController.status));	
            return;
        }
        if (port != null && port.IsOpen)
        {
            //Only tell the Arduino to change the resistance if we are not already at the desired resistance
            if (Resistance != resistance)
            {
                Debug.Log (string.Format ("Setting resistance to: {0}", resistance));
                port.Write(new byte[] { (byte)resistance }, 0, 1);
                Resistance = resistance;
            }
        }
        else
        {
            Debug.Log("Cannot set resistance, port is not open.");
        }
	}
	
	public void Reset() 
    {
        //If the bike is not in the appropriate state, don't fiddle with the resistance
        if (!(bikeController.status == 0x09 || bikeController.status == 0x19 || bikeController.status == 0x89))
        {
            //Debug.Log (string.Format("Cannot reset resistance, bike in wrong state: {0}", bikeController.status));	
            return;
        }
        if (port != null && port.IsOpen)
        {
            //Debug.Log("Resetting resistance.");
            port.Write(new byte[] { (byte)7 }, 0, 1);
            Resistance = 7;
            requiresReset = false;
        }
        else
        {
            Debug.Log("Cannot reset, resistance Port is not open.");
        }
	}

    public void SetFanSpeed(int speed)
    {
        if (globalSettings.EnableFanFeedback)
        {
            if (speed < 0 || speed > 100)
            {
                Debug.Log("Trying to set an invalid fan speed");
                return;
            }
            if (port != null && port.IsOpen)
            {
                port.Write(new byte[] { (byte)(100 + speed) }, 0, 1);
            }
        }
    }
}