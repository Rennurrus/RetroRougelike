using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edgar.Unity;

public class LevelOneConnection : Connection
{
    // Whether the corresponding corridor should be locked
    public bool IsLocked;

    public override ConnectionEditorStyle GetEditorStyle(bool isFocused)
    {
        var style = base.GetEditorStyle(isFocused);

        // Use red color when locked
        if (IsLocked)
        {
            style.LineColor = Color.red;
        }

        return style;
    }
}
