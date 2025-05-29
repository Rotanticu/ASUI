using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEditorInternal;
using System.Reflection;

namespace ASUI
{
    [CustomEditor(typeof(ASUIStyleState))]
    public class ASUIStyleStateEditor : Editor
    {
        //����ѡ����Щ���
        public static List<Type> PrioritySelectionComponentTypeList = new List<Type>()
        {
            typeof(TextMeshProUGUI),
            typeof(Button),
            typeof(Image),
            typeof(CanvasGroup),
            typeof(RawImage),
        };
        /// <summary>
        /// �Ƿ����ֵ�ı�
        /// </summary>
        private bool isDirty;
        /// <summary>
        /// ÿһ��ASUIInfo��ǰѡ����������
        /// </summary>
        private Dictionary<GameObject, Type> selectedComponent = new Dictionary<GameObject, Type>();
        /// <summary>
        /// ÿ�������Ӧ��ASUIStyle�����ļ����Ƿ�չ��
        /// </summary>
        private readonly Dictionary<Component, bool> styleEditorFolderState = new Dictionary<Component, bool>();
        /// <summary>
        /// Unity����Ͽ��Ը���˳���List
        /// </summary>
        private ReorderableList reorderableList;
        /// <summary>
        /// �༭��Ŀ�����
        /// </summary>
        private ASUIStyleState aSUIStyleState;
        /// <summary>
        /// ���治ͬ״̬���������ʽ���ֵ�
        /// </summary>
        private StringToDictionaryIASUIStyleSerializedDictionary StateStyleDictionary => aSUIStyleState.StateStyleDictionary;
        public void OnEnable()
        {
            SerializedProperty componentList = serializedObject.FindProperty("ASUIInfoList");
            reorderableList = new ReorderableList(serializedObject, componentList, true, true, true, true);
            aSUIStyleState = (serializedObject.targetObject as ASUIStyleState);
        }
        public override void OnInspectorGUI()
        {
            isDirty = false;
            serializedObject.Update();
            DrawASUIInfoListHead();
            DrawASUIInfoReorderableList();
            DrawDragArea();
            DrawSwitchStateDropdownButton();
            DrawStateStyleFolderList();
            DrawSaveAndDeleteStateStyleButton();
            EditorGUILayout.BeginHorizontal();
            //������޸ģ��ͱ�����
            if (isDirty)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// ����ASUIInfo��ͷ
        /// </summary>
        private void DrawASUIInfoListHead()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("Type", GUILayout.Width(130));
            EditorGUILayout.LabelField("Component", GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// ����ASUIInfo�б�
        /// </summary>
        private void DrawASUIInfoReorderableList()
        {
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "��Ҫ����״̬��UI�б�");
            };
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                // ��ȡ��index��element
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                Component oldComponent = element.FindPropertyRelative("Component").objectReferenceValue as Component;

                EditorGUILayout.BeginHorizontal();
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3, rect.height), element.FindPropertyRelative("UIName"), GUIContent.none);
                bool folder = EditorGUI.DropdownButton(new Rect(rect.x * 2 + rect.width / 3, rect.y, rect.width / 3, rect.height), new GUIContent(oldComponent == null ? "" : oldComponent.GetType().Name), FocusType.Keyboard, EditorStyles.popup);
                if (folder && oldComponent != null)
                {
                    GenericMenu componentMenu = new GenericMenu();
                    Component[] components = oldComponent.GetComponents<Component>();
                    for (int i = 0; i < components.Length; i++)
                    {
                        Component menuComponent = components[i];
                        componentMenu.AddItem(new GUIContent(components[i].GetType().Name), selectedComponent.ContainsKey(menuComponent.gameObject) && selectedComponent[menuComponent.gameObject] == menuComponent.GetType(), () =>
                        {
                            selectedComponent.Set(menuComponent.gameObject, menuComponent.GetType());
                            ASUIInfo aSUIInfo = aSUIStyleState.ASUIInfoList[index];
                            aSUIInfo.Component = menuComponent;
                            aSUIStyleState.ASUIInfoList[index] = aSUIInfo;
                            EditorUtility.SetDirty(serializedObject.targetObject);
                        });
                    }
                    componentMenu.ShowAsContext();
                }
                EditorGUI.PropertyField(new Rect(rect.x * 3 + rect.width / 3 * 2, rect.y, rect.width / 3, rect.height), element.FindPropertyRelative("Component"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
            };
            reorderableList.DoLayoutList();
        }
        /// <summary>
        /// ������ק��ʾ����
        /// </summary>
        private void DrawDragArea()
        {
            //����ק���������ӡ�����ʾ����
            GUIStyle helpStyle = new GUIStyle(EditorStyles.helpBox);
            helpStyle.alignment = TextAnchor.MiddleCenter;
            helpStyle.normal.textColor = Color.white;
            helpStyle.fontSize = 18;
            helpStyle.fontStyle = FontStyle.BoldAndItalic;
            helpStyle.border = GUI.skin.window.border;
            EditorGUILayout.LabelField(GUIContent.none, new GUIContent("�� ק �� �� �� �� ��"), helpStyle, GUILayout.Height(100), GUILayout.ExpandWidth(true));
            //��ק������ʱ
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                Event.current.Use();
            }
            //��ק�������ɿ�ʱ�����ֵ���������
            else if (Event.current.type == EventType.DragPerform)
            {
                // To consume drag data.
                DragAndDrop.AcceptDrag();

                // GameObjects from hierarchy.
                if (DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0)
                {
                    isDirty = true;
                    foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                    {
                        GameObject dragObject = (GameObject)obj;
                        RectTransform rectTransform = dragObject.gameObject.GetComponent<RectTransform>();
                        if (dragObject != null && rectTransform != null)
                        {
                            string name = dragObject.name;
                            while (aSUIStyleState.ASUIInfoList.Where((ASUIInfo) => ASUIInfo.UIName == name).Count() > 0)
                            {
                                string[] nameSplit = name.Split('_');
                                if (nameSplit.Length > 1)
                                    name = nameSplit[0] + $"_{int.Parse(nameSplit[nameSplit.Length - 1]) + 1}";
                                else
                                    name += "_1";
                            }
                            bool isExistSuitableComponent = false;
                            foreach (Type componentType in PrioritySelectionComponentTypeList)
                            {
                                if (dragObject.GetComponent(componentType) != null)
                                {
                                    aSUIStyleState.ASUIInfoList.Add(new ASUIInfo() { UIName = name, Component = dragObject.GetComponent(componentType) });
                                    isExistSuitableComponent = true;
                                    break;
                                }
                            }
                            if (!isExistSuitableComponent)
                                aSUIStyleState.ASUIInfoList.Add(new ASUIInfo() { UIName = name, Component = dragObject.GetComponent<RectTransform>() });
                        }
                    }
                }
            }
        }
        /// <summary>
        /// �����л�״̬�������˵��͵�ǰ״̬��
        /// </summary>
        private void DrawSwitchStateDropdownButton()
        {
            EditorGUILayout.BeginHorizontal();
            //ָʾ��ǰ��ʽ״̬
            if (EditorGUILayout.DropdownButton(new GUIContent(aSUIStyleState.State), FocusType.Keyboard))
            {
                GenericMenu componentMenu = new GenericMenu();
                var stateList = StateStyleDictionary.Keys.ToList();
                for (int i = 0; i < stateList.Count; i++)
                {
                    int index = i;
                    var x = stateList[index];
                    componentMenu.AddItem(new GUIContent(stateList[index]), aSUIStyleState.State == stateList[index], () =>
                    {
                        aSUIStyleState.State = stateList[index];
                        foreach (var kvp in StateStyleDictionary[aSUIStyleState.State])
                        {
                            kvp.Value.ApplyStyle(kvp.Key);
                        }
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        serializedObject.ApplyModifiedProperties();
                });
                }
                componentMenu.ShowAsContext();
            }
            aSUIStyleState.State = EditorGUILayout.TextField(aSUIStyleState.State);
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// ���������Ӧ����ʽ�ļ���
        /// </summary>
        private void DrawStateStyleFolderList()
        {
            if (StateStyleDictionary == null || !StateStyleDictionary.ContainsKey(aSUIStyleState.State))
                return;
            var cpmponentStyleDictionary = StateStyleDictionary[aSUIStyleState.State];
            foreach (var keyValuePair in cpmponentStyleDictionary)
            {
                Component component = keyValuePair.Key;
                IASUIStyle aSUIStyle = keyValuePair.Value;

                if (!styleEditorFolderState.ContainsKey(component))
                    styleEditorFolderState.Add(component, false);

                ASUIEditorExtend.Foldout(component.name,
                    () => styleEditorFolderState[component],
                    (isFolder) => styleEditorFolderState[component] = isFolder,
                    () =>
                    {
                        EditorGUILayout.Space();
                        aSUIStyle.DrawInEditorFoldout();
                    });
            }
        }
        /// <summary>
        /// ���Ʊ����ɾ����ʽ��ť
        /// </summary>
        private void DrawSaveAndDeleteStateStyleButton()
        {
            //���浱ǰ�������ʽ
            if (GUILayout.Button(new GUIContent("������ʽ")))
            {
                if (string.IsNullOrEmpty(aSUIStyleState.State))
                {
                    EditorUtility.DisplayDialog("������ʽ", "û��������ʽ,�������󱣴�", "ok");
                }
                else
                {
                    if (!StateStyleDictionary.ContainsKey(aSUIStyleState.State) || EditorUtility.DisplayDialog("������ʽ", $"��ʽ {aSUIStyleState.State} �Ѵ��ڣ�ȷ��������ʽ��", "ȷ��", "ȡ��"))
                    {
                        if (!StateStyleDictionary.ContainsKey(aSUIStyleState.State))
                            StateStyleDictionary.Add(aSUIStyleState.State, new ComponentToIASUIStyleSerializedDictionary());
                        foreach (var ASUIInfo in aSUIStyleState.ASUIInfoList)
                        {
                            if (ASUIInfo.Component == null)
                                continue;
                            var type = ASUIInfo.Component.GetType();
                            var styleType = Type.GetType($"ASUI.{type.Name}Style");
                            if (styleType == null)
                                continue;
                            var styleTypeAttribute = styleType.GetCustomAttribute<ASUIStyleAttribute>();
                            if (styleTypeAttribute != null && styleTypeAttribute.ComponentType == type)
                            {
                                var asuiStyle = Activator.CreateInstance(styleType) as IASUIStyle;
                                asuiStyle.SaveStyle(ASUIInfo.Component);
                                StateStyleDictionary[aSUIStyleState.State].Set(ASUIInfo.Component, asuiStyle);
                            }
                        }
                        isDirty = true;
                    }
                }
            }
            if (GUILayout.Button(new GUIContent("ɾ����ʽ")))
            {
                bool result = EditorUtility.DisplayDialog("ɾ����ʽ", $"ȷ��Ҫɾ����ʽ {aSUIStyleState.State} ��", "ȷ��", "ȡ��");
                if (result)
                {
                    if (StateStyleDictionary != null && StateStyleDictionary.ContainsKey(aSUIStyleState.State))
                    {
                        StateStyleDictionary.Remove(aSUIStyleState.State);
                        if (StateStyleDictionary.Count > 0)
                            aSUIStyleState.State = StateStyleDictionary.FirstOrDefault().Key;
                        else
                            aSUIStyleState.State = "normal";
                    }
                    isDirty = true;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}


