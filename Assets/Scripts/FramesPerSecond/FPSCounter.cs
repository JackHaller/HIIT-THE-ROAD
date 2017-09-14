using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public int AverageFPS { get; private set; }

    public int HighestFPS { get; private set; }
    public int LowestFPS { get; private set; }

    public int frameRange = 60;

    /**
        Now we need a buffer to store the FPS values of multiple frames, 
        plus an index so we know where to put the data of the next frame.
    */
    int[] fpsBuffer;
    int fpsBufferIndex;

    void Update()
    {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange)
        {
            InitializeBuffer();
        }
        UpdateBuffer();
        CalculateFPS();

        AverageFPS = (int)(1f / Time.unscaledDeltaTime);
    }

    /**
        When initializing this buffer, 
        make sure that frameRange is at least 1, 
        and set the index to 0.
    */
    void InitializeBuffer()
    {
        if (frameRange <= 0)
        {
            frameRange = 1;
        }
        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }

    /**
        Updating the buffer is done by storing the current 
        FPS at the current index, which is then incremented.

        But we'll quickly fill our entire buffer, then what? 
        We'll have to discard the oldest value before adding the
        new one. We could shift all values one position, 
        but the average doesn't care about the order that the 
        values are in. So we can just wrap the index back 
        to the start of the array. That way we always override the
        oldest value with the newest, once the buffer has been filled.
    */
    void UpdateBuffer()
    {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange)
        {
            fpsBufferIndex = 0;
        }
    }

    /**
        Calculating the average is a simple matter of summing 
        all values in the buffer and dividing by the number of values.
        To get a good average make sure to use a float division, 
        then cast back to an integer.
    */
    void CalculateFPS()
    {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;
        for (int i = 0; i < frameRange; i++)
        {
            int fps = fpsBuffer[i];
            sum += fps;
            if (fps > highest)
            {
                highest = fps;
            }
            if (fps < lowest)
            {
                lowest = fps;
            }
        }
        AverageFPS = (int)((float)sum / frameRange);
        HighestFPS = highest;
        LowestFPS = lowest;
    }
}
 