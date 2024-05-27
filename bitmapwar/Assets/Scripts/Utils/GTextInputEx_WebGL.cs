using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

/*
public class GTextInputEx_WebGL
{
    private GTextInput gTextInput;
    private bool isMultiLine = true;
    private int fontsize = 20;
    private string inputID;

    public GTextInputEx_WebGL(GTextInput input)
    {
        gTextInput = input;
        isMultiLine = !gTextInput.singleLine;
        fontsize = gTextInput.textFormat.size;
        inputID = gTextInput.inputTextField.gameObject.GetInstanceID().ToString();
        //InputWebGLManage.Instance.Register(gTextInput.inputTextField.gameObject.GetInstanceID(), this);
        input.onFocusIn.Add(OnFocusIn);
        input.onFocusOut.Add(OnFocusOut);
    }

    private void OnFocusOut(EventContext context)
    {

    }

    private void OnFocusIn(EventContext context)
    {
        string text = gTextInput.text;
        string indexStr = gTextInput.inputTextField.selectionBeginIndex + "|" +
                          gTextInput.inputTextField.selectionEndIndex;

        Vector2 screenPos = gTextInput.LocalToGlobal(Vector2.zero);
        string inputRectStr = screenPos.x + "|" + screenPos.y + "|" + gTextInput.width + "|" + gTextInput.height;

        //Debug.LogError(inputID+"  "+text+"  "+fontsize+"   "+indexStr+"  "+inputRectStr);
        InputWebGLManage.Instance.InputShow( inputID, text, fontsize.ToString(), indexStr, inputRectStr);
    }

    #region WebGL回调
    public void OnInputText(string text, string selectStartIndexStr, string selectEndIndexStr)
    {
        //Debug.LogError("OnInputText  "+text);
        gTextInput.text = text;


    }
    public void OnInputEnd()
    {
        WebGLInput.captureAllKeyboardInput = true;
        //inputField.DeactivateInputField();
    }

    public void SelectAll()
    {
        gTextInput.SetSelection(0,gTextInput.text.Length);
    }

    #endregion
}
*/