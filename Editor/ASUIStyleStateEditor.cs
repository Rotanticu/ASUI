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
        //优先选中哪些组件
        public static List<Type> PrioritySelectionComponentTypeList = new List<Type>()
        {
            typeof(TextMeshProUGUI),
            typeof(Button),
            typeof(Image),
            typeof(CanvasGroup),
            typeof(RawImage),
        };
        /// <summary>
        /// 是否组件值改变
        /// </summary>
        private bool isDirty;
        /// <summary>
        /// 每一个ASUIInfo当前选择的组件类型
        /// </summary>
        private Dictionary<GameObject, Type> selectedComponent = new Dictionary<GameObject, Type>();
        /// <summary>
        /// 每个组件对应的ASUIStyle伸缩文件夹是否展开
        /// </summary>
        private readonly Dictionary<Component, bool> styleEditorFolderState = new Dictionary<Component, bool>();
        /// <summary>
        /// Unity面板上可以更改顺序的List
        /// </summary>
        private ReorderableList reorderableList;
        /// <summary>
        /// 编辑器目标对象
        /// </summary>
        private ASUIStyleState aSUIStyleState;
        /// <summary>
        /// 获取当前状态的样式数据
        /// </summary>
        private ASUIStyleStateData GetCurrentStateData()
        {
            return aSUIStyleState.GetStyleState(aSUIStyleState.CurrentState);
        }
        public void OnEnable()
        {
            SerializedProperty componentList = serializedObject.FindProperty("componentInfos");
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
            //如果有修改，就保存下
            if (isDirty)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 绘制ASUIInfo表头
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
        /// 绘制ASUIInfo列表
        /// </summary>
        private void DrawASUIInfoReorderableList()
        {
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "需要配置状态的UI列表");
            };
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                // 获取第index个element
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                if (element == null) return;
                
                SerializedProperty componentProperty = element.FindPropertyRelative("component");
                Component oldComponent = componentProperty?.objectReferenceValue as Component;

                EditorGUILayout.BeginHorizontal();
                
                // 计算每个字段的宽度，留出间隔
                float spacing = 10f;
                float fieldWidth = (rect.width - spacing * 2) / 3f;
                
                SerializedProperty nameProperty = element.FindPropertyRelative("componentName");
                if (nameProperty != null)
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, fieldWidth, rect.height), nameProperty, GUIContent.none);
                    
                bool folder = EditorGUI.DropdownButton(new Rect(rect.x + fieldWidth + spacing, rect.y + 2, fieldWidth, rect.height), new GUIContent(oldComponent == null ? "" : oldComponent.GetType().Name), FocusType.Keyboard, EditorStyles.popup);
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
                            var componentInfo = aSUIStyleState.ComponentInfos[index];
                            componentInfo.component = menuComponent;
                            componentInfo.componentTypeName = menuComponent.GetType().FullName;
                            aSUIStyleState.ComponentInfos[index] = componentInfo;
                            EditorUtility.SetDirty(serializedObject.targetObject);
                        });
                    }
                    componentMenu.ShowAsContext();
                }
                if (componentProperty != null)
                    EditorGUI.PropertyField(new Rect(rect.x + (fieldWidth + spacing) * 2, rect.y, fieldWidth, rect.height), componentProperty, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            };
            reorderableList.DoLayoutList();
        }
        /// <summary>
        /// 绘制拖拽提示区域
        /// </summary>
        private void DrawDragArea()
        {
            //“拖拽到这里添加”，提示区域
            GUIStyle helpStyle = new GUIStyle(EditorStyles.helpBox);
            helpStyle.alignment = TextAnchor.MiddleCenter;
            helpStyle.normal.textColor = Color.white;
            helpStyle.fontSize = 18;
            helpStyle.fontStyle = FontStyle.BoldAndItalic;
            helpStyle.border = GUI.skin.window.border;
            EditorGUILayout.LabelField(GUIContent.none, new GUIContent("拖 拽 到 这 里 添 加"), helpStyle, GUILayout.Height(100), GUILayout.ExpandWidth(true));
            //拖拽到区域时
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                Event.current.Use();
            }
            //拖拽到区域松开时，往字典里新增项
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
                            while (aSUIStyleState.ComponentInfos.Where(info => info.componentName == name).Count() > 0)
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
                                    aSUIStyleState.AddComponentWithName(name, dragObject.GetComponent(componentType));
                                    isExistSuitableComponent = true;
                                    break;
                                }
                            }
                            if (!isExistSuitableComponent)
                                aSUIStyleState.AddComponentWithName(name, dragObject.GetComponent<RectTransform>());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 绘制切换状态的下拉菜单和当前状态名
        /// </summary>
        private void DrawSwitchStateDropdownButton()
        {
            EditorGUILayout.BeginHorizontal();
            //指示当前样式状态
            if (EditorGUILayout.DropdownButton(new GUIContent(aSUIStyleState.CurrentState), FocusType.Keyboard))
            {
                GenericMenu componentMenu = new GenericMenu();
                var stateList = aSUIStyleState.StyleStates.Select(s => s.stateName).ToList();
                for (int i = 0; i < stateList.Count; i++)
                {
                    int index = i;
                    var stateName = stateList[index];
                    componentMenu.AddItem(new GUIContent(stateName), aSUIStyleState.CurrentState == stateName, () =>
                    {
                        _ = aSUIStyleState.ApplyState(stateName);
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                componentMenu.ShowAsContext();
            }
            string newState = EditorGUILayout.TextField(aSUIStyleState.CurrentState);
            if (newState != aSUIStyleState.CurrentState)
            {
                _ = aSUIStyleState.ApplyState(newState);
            }
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制组件对应的样式文件夹
        /// </summary>
        private void DrawStateStyleFolderList()
        {
            var currentStateData = GetCurrentStateData();
            if (currentStateData == null)
                return;
                
            foreach (var componentStyle in currentStateData.componentStyles)
            {
                Component component = aSUIStyleState.GetComponentByUIName<Component>(componentStyle.componentName);
                if (component == null || componentStyle.style == null)
                    continue;

                if (!styleEditorFolderState.ContainsKey(component))
                    styleEditorFolderState.Add(component, false);

                ASUIEditorExtend.Foldout(component.name,
                    () => styleEditorFolderState[component],
                    (isFolder) => styleEditorFolderState[component] = isFolder,
                    () =>
                    {
                        EditorGUILayout.Space();
                        componentStyle.style.DrawInEditorFoldout(component);
                    });
            }
        }
        /// <summary>
        ///  绘制保存和删除样式按钮
        /// </summary>
        private void DrawSaveAndDeleteStateStyleButton()
        {
            //保存当前组件的样式
            if (GUILayout.Button(new GUIContent("保存样式")))
            {
                if (string.IsNullOrEmpty(aSUIStyleState.CurrentState))
                {
                    EditorUtility.DisplayDialog("保存样式", "没有命名样式,请命名后保存", "ok");
                }
                else
                {
                    var existingState = aSUIStyleState.GetStyleState(aSUIStyleState.CurrentState);
                    if (existingState != null && !EditorUtility.DisplayDialog("保存样式", $"样式 {aSUIStyleState.CurrentState} 已存在，确定覆盖样式吗？", "确定", "取消"))
                    {
                        return;
                    }
                    
                    var componentStyles = new List<ASUIComponentStyleData>();
                    foreach (var componentInfo in aSUIStyleState.ComponentInfos)
                    {
                        if (componentInfo.component == null)
                            continue;
                            
                        var type = componentInfo.component.GetType();
                        Type styleType = null;
                        var asuiAssembly = System.Reflection.Assembly.Load("ASUI");
                        if (asuiAssembly != null)
                        {
                            var styleTypeName = $"ASUI.{type.Name}Style";
                            styleType = asuiAssembly.GetType(styleTypeName);
                        }
                        if (styleType == null)
                            continue;
                            
                        var styleTypeAttribute = styleType.GetCustomAttribute<ASUIStyleAttribute>();
                        if (styleTypeAttribute != null && styleTypeAttribute.ComponentType == type)
                        {
                            var asuiStyle = Activator.CreateInstance(styleType) as IASUIStyle;
                            asuiStyle.SaveStyle(componentInfo.component);
                            
                            componentStyles.Add(new ASUIComponentStyleData
                            {
                                componentName = componentInfo.componentName,
                                componentTypeName = componentInfo.componentTypeName,
                                style = asuiStyle
                            });
                        }
                    }
                    
                    aSUIStyleState.AddOrUpdateState(aSUIStyleState.CurrentState, componentStyles);
                    isDirty = true;
                }
            }
            if (GUILayout.Button(new GUIContent("删除样式")))
            {
                bool result = EditorUtility.DisplayDialog("删除样式", $"确定要删除样式 {aSUIStyleState.CurrentState} 吗？", "确定", "取消");
                if (result)
                {
                    var stateToRemove = aSUIStyleState.StyleStates.FirstOrDefault(s => s.stateName == aSUIStyleState.CurrentState);
                    if (stateToRemove != null)
                    {
                        aSUIStyleState.StyleStates.Remove(stateToRemove);
                        if (aSUIStyleState.StyleStates.Count > 0)
                            _ = aSUIStyleState.ApplyState(aSUIStyleState.StyleStates.First().stateName);
                        else
                            _ = aSUIStyleState.ApplyState("normal");
                    }
                    isDirty = true;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}


