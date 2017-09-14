using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FPSCounter))]
public class FPSDisplay : MonoBehaviour
{

    /**
        As FPSDisplay in the only thing that will use this structure, 
        we put the struct definition directly inside that class and make it private 
        so that it won't show up in the global namespace. 
        Make it serializable so that it can be exposed by the Unity editor.
    */
    [System.Serializable]
    private struct FPSColor
    {
        public Color color;
        public int minimumFPS;
    }

    public Text highestFPSLabel, averageFPSLabel, lowestFPSLabel;

    FPSCounter fpsCounter;

    /**
        Now add an array of these struct so we can configure the coloring of the FPS labels. 
        We'd typically add a public field for that, but we can't do that because the struct 
        itself is private. So make the array private as well and give it the SerializeField 
        attribute so Unity exposes it in the editor and saves it.
    */
    [SerializeField]
    private FPSColor[] coloring;

    void Awake()
    {
        fpsCounter = GetComponent<FPSCounter>();
    }

    /**

        Everything appears to work find now, but there is a subtle problem.
        We are now creating a new string object each update, 
        which is discarded the next update. 
        This pollutes the managed memory, which will trigger the garbage collector. 
        While this isn't a big deal for desktop apps, 
        it is more troublesome for devices with little memory to spare. 
        It also pollutes our profiler data, which is annoying when you're hunting for allocations.

        Can we get rid of these temporary strings? 
        The value that we are displaying can be any integer between 0 and 99. 
        That's 100 different strings. Why not create all these strings once and reuse them, 
        instead of recreating the same content all the time?

        By using a fixed array of string representations of every number that we might need, 
        we have eliminated all temporary string allocations!

    */
    static string[] stringsFrom00To99 = {
        "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
        "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
        "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
        "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
        "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
        "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
        "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
        "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
        "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
        "90", "91", "92", "93", "94", "95", "96", "97", "98", "99"
    };

    void Update()
    {
        Display(highestFPSLabel, fpsCounter.HighestFPS);
        Display(averageFPSLabel, fpsCounter.AverageFPS);
        Display(lowestFPSLabel, fpsCounter.LowestFPS);
    }

    /**
        The correct color can be found by looping through the array until the
        minimum FPS for a color is met. Then set the color and break out of the loop.
    */
    void Display(Text label, int fps)
    {
        label.text = stringsFrom00To99[Mathf.Clamp(fps, 0, 99)];
        for (int i = 0; i < coloring.Length; i++)
        {
            if (fps >= coloring[i].minimumFPS)
            {
                label.color = coloring[i].color;
                break;
            }
        }
    }
}
 