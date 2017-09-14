using System;
using System.IO;
using UnityEngine;

namespace AssemblyCSharp
{
	public class PlayerWriter
	{
		private StreamWriter myWriter;
		public bool isUsable = false;
		
		public PlayerWriter ()
		{
			//Let's open our file.
			myWriter = new StreamWriter(File.Open(System.Environment.CurrentDirectory + "/WorldPlaybackData/Player/" + System.DateTime.Now.Ticks + "Playerdata.egp",FileMode.OpenOrCreate));
			isUsable = true;
		}
		
		public PlayerWriter(string username, int age, double BMI, int fitness){
			//Let's open our file.
			Console.WriteLine("writing data");
			DirectoryInfo datadir = new DirectoryInfo(System.Environment.CurrentDirectory + "/WorldPlaybackData/Player/");
			myWriter = new StreamWriter(File.Open(datadir + username+"_"+age+"_"+(int)BMI+"_"+fitness+"_"+ System.DateTime.Now.Ticks + "_Playerdata.egp",FileMode.OpenOrCreate));
			isUsable = true;
		}
		
		public void WritePositions(Vector3 body, Vector3 head){
			myWriter.WriteLine(body.x + ":"+body.y+":"+body.z+":"+head.x+":"+head.y+":"+head.z); // write the force at each step.
		}
		
		public void WritePositions(Vector3 body, Vector3 head, int score){
			myWriter.WriteLine(body.x + ":"+body.y+":"+body.z+":"+head.x+":"+head.y+":"+head.z +":"+ score); // write the force at each step.
		}
		
		public void ClosePlayerWriter(){
			myWriter.Close();
			isUsable = false;
		}
		
	}
}

