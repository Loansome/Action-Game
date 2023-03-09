using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputBuffer
{

    public List<InputBufferButton> buttons;
    public int lastButtonUsed;
    public void Initialize()
    {
        buttons = new List<InputBufferButton>();
        for (int i = 0; i < GameEngine.current.variablesObject.rawInputBindings.Count; i++)
        {
            buttons.Add(new InputBufferButton());
        }
    }

    public bool HoldingLastCancelButton()
    {
        if (buttons[lastButtonUsed].buffer[0].hold > 0) { return true; }
        return false;
    }
    public void Update()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].holdOK = -1; //Might be unnecessary since we can and want to check on specific frames for certain inputs (like hold RB + press A)
            buttons[i].pressOK = -1;
            for (int b = buttons[i].buffer.Count - 1; b > 0; b--)
            {
                buttons[i].buffer[b].hold = buttons[i].buffer[b - 1].hold;
                buttons[i].buffer[b].used = buttons[i].buffer[b - 1].used;
                buttons[i].buffer[b].press = buttons[i].buffer[b - 1].press;
                if (buttons[i].buffer[b].hold > 0) { buttons[i].holdOK = b; }
                if (buttons[i].buffer[b].hold == 1 && buttons[i].buffer[b].used == false) { buttons[i].pressOK = b; }
            }
            buttons[i].buffer[0].Update(i);
            if (buttons[i].buffer[0].hold > 0) { buttons[i].holdOK = 0; }
            if (buttons[i].buffer[0].hold == 1 && buttons[i].buffer[0].used == false) { buttons[i].pressOK = 0; }
        }

    }
}

[System.Serializable]
public class InputBufferButton
{

    public int pressOK;
    public int holdOK;
    public static int bufferSize = 15;
    public List<InputBufferFrameItem> buffer;

    public InputBufferButton()
    {
        buffer = new List<InputBufferFrameItem>();
        for (int i = 0; i < bufferSize; i++)
        {
            buffer.Add(new InputBufferFrameItem());
        }
    }

}

[System.Serializable]
public class InputBufferFrameItem
{
    public void Update(int b)
    {
        string nextRaw = GameEngine.current.variablesObject.rawInputBindings[b];
        if (nextRaw == "RT")
        {
            if (Input.GetAxisRaw(nextRaw) > 0.2f)
            { hold++; }
            else { hold = 0; used = false; }
        }
        else if (nextRaw == "LT")
        {
            if (Input.GetAxisRaw(nextRaw) < -0.2f)
            {
                hold++;
            }
            else { hold = 0; used = false; }
        }
        else
        {
            if (Input.GetButton(nextRaw))
            {
                hold++;
            }
            else { hold = 0; used = false; }
        }
    }
    public bool used;
    public bool press;
    public int hold;
}

[System.Serializable]
public class CommandInput
{
    public bool active;
    public int buttonIndex;
}


