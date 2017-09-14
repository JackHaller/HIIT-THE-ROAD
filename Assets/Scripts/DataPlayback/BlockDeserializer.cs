using System;
using System.IO;
using UnityEngine;


namespace Exergame
{
	public class BlockDeserializer
	{
		public string PlaybackFile = "";
		private StreamReader myReader = null;
		public bool isUsable = false;
		
		public BlockDeserializer (string file)
		{
			PlaybackFile = file;
			myReader = new StreamReader(PlaybackFile);
			Debug.Log("Worlddata loaded from file:" + PlaybackFile);
			Debug.Log("Timestamped at: " + myReader.ReadLine()); // Letś just output the timestamp so we can be sure....
			isUsable = true;
			
			//for(int i =0; i < 10; i++){
				//GetNextBlock();	
			//}
			
		}
		
		private string ReadBlock(){
			int debugcount = 0; // Due to the nature of reading from file. We need to make sure we dont hit inf loop and crash unity.
			//Just a safety switch :3
			//In order to read an entire block, we have to read until the end tag. Which is just </ + the rest of the first tag.
			string blockdata = string.Empty;
			string blockidend = myReader.ReadLine() + Environment.NewLine; // Get the block identifier.
			blockdata += blockidend; // add it to the blockdata. It has the info string.
			//Ok now to create the block end tag.
			blockidend = "</"+blockidend.Substring(1); // add the end tag and finish off the tag. Now we have the end tag.
			
			while(!blockdata.Contains(blockidend)){ // While we don't have the end tag, keep reading.
				blockdata += myReader.ReadLine() + Environment.NewLine; //Read in all the block data.
				debugcount++;
				if(debugcount == 1000){
					Debug.LogError("INFINITE LOOP IN READBLOCK" + blockdata);
				break;
			}
				
			}
			//Because all the tags MUST BE SYM WE ALWAYS NEED A CLOSING TAG. HOWEVER WE MAY FIT ENDOFFILE DUE TO WRITELINE
			//Kill the stream when out of data. Set isUsable to false so we revert to generated world data.
			if(myReader.EndOfStream || myReader.Peek().Equals(string.Empty)){
				myReader.Close();
				isUsable = false;
				Debug.LogWarning("End of playback file read. No more world data to be loaded. Revert to procedual after this block");
			}
			//Debug.Log(blockdata);
			blockdata = blockdata.Trim(); // Trim the final endline char to stop getting an extra string when we split
			//Now we have all the block data for this block.
			return blockdata;
			
		}
		
		//Creates the next block to be loaded in from the saved data. Should return either a Block or a TwinnedBlock Object.
		//This gets added directly to the BlockList used for rendering the game.
		public string[] GetNextBlock(){
			//Get the next block.
			string block = ReadBlock();
			string[] blockdata = block.Split(Environment.NewLine.ToCharArray(),StringSplitOptions.RemoveEmptyEntries); // Split all the lines by endofline char.
			//foreach(string s in blockdata){
				//Debug.Log("block data: " + s); //lets check our data.
			//}
			//array makeup:
			//0: block info. Contains tags + pit input values that were used by the BlockBuilder to make the pits.
			//1: blocktype. Use this generate the blocktype.
			//The next values can change depending on if the objects are present on this particular block.
			//General order in best case is
			//Cannon info
			//Powerup info
			//BlockXYZ in the world.
			//Have to determine which each line is but that shouldnt be hard. Check if contain 'can'/'power'/'block'?
			
			return blockdata;
		}
				
		public void KillPlayback(){
			myReader.Close();
			isUsable = false;
			Debug.LogWarning("World playback killed. Probably Player Death/Gameoverstate? - KillPlayback()");
		}
	}
}

