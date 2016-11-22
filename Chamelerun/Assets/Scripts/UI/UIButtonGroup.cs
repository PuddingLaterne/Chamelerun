using UnityEngine;
using System.Collections;

public class UIButtonGroup : MonoBehaviour 
{
    public UIButton[] Buttons;
    public float RepeatDelay = 0.1f;

    private bool isLocked;
    private int highlightedIndex;

    public void OnEnable()
    {
        isLocked = false;
        HighlightButton(0);
    }

    public void Update()
    {
        if (!isLocked)
        {
            float input = InputHelper.VerticalInput;
            if (input < 0)
            {
                HighlightButton(highlightedIndex < Buttons.Length - 1 ? highlightedIndex + 1 : 0);
            }
            if(input > 0)
            {
                HighlightButton(highlightedIndex > 0 ? highlightedIndex - 1 : Buttons.Length - 1);
            }
        }

        if (InputHelper.ConfirmPressed)
        {
            Buttons[highlightedIndex].Select();
        }
    }

    private void HighlightButton(int index)
    {
        highlightedIndex = index;
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].Highlight(i == index); 
        }
        StartCoroutine(WaitForRepeatDelay());
    }

    private IEnumerator WaitForRepeatDelay()
    {
        isLocked = true;
        yield return new WaitForSeconds(RepeatDelay);
        isLocked = false;
    }
}
