using UnityEditor;
using UnityEngine;
using System;

public static class ASUIEditorExtend
{
    public static void Foldout(
    string foldName,
    Func<bool> getIsFolder,
    Action<bool> setIsFolder,
    Action drawAct = null,
    Action<bool> enableAct = null,
    bool enable = true
    )
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.label).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(20f, -2f);

        var rect = GUILayoutUtility.GetRect(16f, 22f, style);

        GUI.backgroundColor = Color.white;
        GUI.Box(rect, foldName, style);

        var e = Event.current;
        bool foldOut = getIsFolder();

        if (enableAct == null)
        {
            if (e.type == EventType.Repaint)
            {
                var arrowRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
                EditorStyles.foldout.Draw(arrowRect, false, false, foldOut, false);
            }
        }
        else
        {
            // ÓÐ„¿¥Á¥§¥Ã¥¯
            var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);
            bool sw = GUI.Toggle(toggleRect, enable, string.Empty, new GUIStyle("ShurikenCheckMark"));
            if (sw != enable)
            {
                enableAct(sw);
            }
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            foldOut = !foldOut;
            setIsFolder(foldOut);
            e.Use();
        }

        if (foldOut && drawAct != null)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                using (new EditorGUI.DisabledScope(!enable))
                {
                    drawAct();
                }
            }
        }
    }
}
