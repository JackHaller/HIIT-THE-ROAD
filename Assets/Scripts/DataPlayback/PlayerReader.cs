using System;
using UnityEngine;
using System.IO;

namespace AssemblyCSharp
{
	public class PlayerReader
	{
		
		private StreamReader myReader = null;
		public bool isUsable = false;
		private char[] split;

		public PlayerReader ()
		{
			Debug.Log("Calling default constructor of PlayerReader, this shouldn't happen.");
		}
		
		public PlayerReader (string data)
		{
			//Debug.LogWarning ("Data: " + data);
			myReader = new StreamReader (data);
			isUsable = true;
			split = ":".ToCharArray ();
		}
		
		public BodyHeadScoreDATA ReadFrame()
		{
			if (isUsable){
				string temp = myReader.ReadLine ();
				if (myReader.Peek() == -1) {// No more data to read.
					ClosePlayback();
				}
				string[] vec = temp.Split (split, StringSplitOptions.RemoveEmptyEntries);
				BodyHeadScoreDATA data;
				data.body = new Vector3 (float.Parse (vec [0]), float.Parse (vec [1]), float.Parse (vec [2]));
				data.head = new Vector3 (float.Parse (vec [3]), float.Parse (vec [4]), float.Parse (vec [5]));
				data.score = float.Parse(vec[6]); // Score at this frame.
				return data;
			} else {
				return new BodyHeadScoreDATA();
			}
		}
		
		private void ClosePlayback(){
			isUsable = false;
			myReader.Close();
		}
	}

	
	public struct BodyHeadScoreDATA{
		public Vector3 body;
		public Vector3 head;
		public float score;
	}
}
