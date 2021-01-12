using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasNav : MonoBehaviour
{
	public List<InputField> inputs;
	public int inputIndex;

	void Start()
	{
		//Check if there are input fields in the canvas
		if (inputs != null)
		{
			//If there are, select the first one
			inputs[0].Select();
			inputIndex = 0;
		}
	}

	void Update()
	{
		//Check if the tab key is being pressed and if there are more than one input fields in the list
		if (Input.GetKeyDown(KeyCode.Tab) && inputs.Count > 1)
		{
			//If there are, check if either shift key is being pressed
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				//If shift is pressed, move up on the list - or, if at the top of the list, move to the bottom
				if (inputIndex <= 0)
				{
					inputIndex = inputs.Count;
				}
				inputIndex--;
				inputs[inputIndex].Select();
			}
			else
			{
				//if shift is not pressed, move down on the list - or, if at the bottom, move to the top
				if (inputs.Count <= inputIndex + 1)

				{
					inputIndex = -1;
				}
				inputIndex++;
				inputs[inputIndex].Select();
			}
		}
	}
}