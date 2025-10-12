using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEditorInternal;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

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
        /// 用户输入的状态名（用于保存时使用）
        /// </summary>
        private string inputStateName;
        
        /// <summary>
        /// 当前正在编辑的动画配置名称
        /// </summary>
        private string currentEditingAnimationName;
        
        /// <summary>
        /// 主动画组件引用（编辑器专用）
        /// </summary>
        private LitMotion.Animation.LitMotionAnimation mainAnimation;
        
        /// <summary>
        /// 主动画组件的编辑器
        /// </summary>
        private Editor mainAnimationEditor;
        
        /// <summary>
        /// 根VisualElement引用，用于重新创建界面
        /// </summary>
        private VisualElement rootElement;
        
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
        
        public void OnDisable()
        {
            // 清理主动画编辑器
            if (mainAnimationEditor != null)
            {
                DestroyImmediate(mainAnimationEditor);
                mainAnimationEditor = null;
            }
        }
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            rootElement = root; // 保存引用用于重新创建
            
            // 创建动画转换按钮
            root.Add(CreateAnimToButton());
            
            // 创建主动画编辑器
            root.Add(CreateMainAnimationEditor());
            
            return root;
        }
        
        /// <summary>
        /// 创建动画转换按钮
        /// </summary>
        private VisualElement CreateAnimToButton()
        {
            var container = new VisualElement();
            container.name = "animation-matrix";
            
            // 添加标题
            var titleLabel = new Label("动画转换矩阵");
            titleLabel.style.fontSize = 14;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            container.Add(titleLabel);
            
            var stateList = aSUIStyleState.StyleStates.Select(s => s.stateName).ToList();
            if (stateList.Count < 2)
            {
                var helpBox = new HelpBox("需要至少2个状态才能创建动画转换", HelpBoxMessageType.Info);
                container.Add(helpBox);
                return container;
            }
            
            // 创建动画转换矩阵
            CreateAnimationMatrix(container, stateList);
            
            return container;
        }
        
        /// <summary>
        /// 创建动画转换矩阵
        /// </summary>
        private void CreateAnimationMatrix(VisualElement container, List<string> stateList)
        {
            var animRoot = aSUIStyleState.transform.Find("_Anim");
            var existingAnimations = new HashSet<string>();
            
            // 获取已存在的动画配置
            if (animRoot != null)
            {
                for (int i = 0; i < animRoot.childCount; i++)
                {
                    var child = animRoot.GetChild(i);
                    if (child.GetComponent<LitMotion.Animation.LitMotionAnimation>() != null)
                    {
                        existingAnimations.Add(child.name);
                    }
                }
            }
            
            // 创建矩阵表格
            var table = new VisualElement();
            table.style.flexDirection = FlexDirection.Column;
            
            // 创建表头
            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            
            var fromLabel = new Label("从\\到");
            fromLabel.style.width = 80;
            fromLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            headerRow.Add(fromLabel);
            
            foreach (var state in stateList)
            {
                var stateLabel = new Label(state);
                stateLabel.style.width = 80;
                stateLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                headerRow.Add(stateLabel);
            }
            table.Add(headerRow);
            
            // 创建矩阵内容
            for (int i = 0; i < stateList.Count; i++)
            {
                var fromState = stateList[i];
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                
                // 行标签
                var rowLabel = new Label(fromState);
                rowLabel.style.width = 80;
                rowLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                row.Add(rowLabel);
                
                // 绘制每个转换按钮
                for (int j = 0; j < stateList.Count; j++)
                {
                    var toState = stateList[j];
                    var animationName = $"{fromState}To{toState}";
                    var hasAnimation = existingAnimations.Contains(animationName);
                    var isSameState = fromState == toState;
                    var isCurrentAnimation = animationName == currentEditingAnimationName;
                    
                    if (isSameState)
                    {
                        // 相同状态，显示"-"标签，不是按钮
                        var label = new Label("-");
                        label.style.width = 80;
                        label.style.unityTextAlign = TextAnchor.MiddleCenter;
                        label.style.backgroundColor = Color.clear; // 透明背景
                        row.Add(label);
                    }
                    else
                    {
                        var button = new UnityEngine.UIElements.Button(() => {
                            if (hasAnimation)
                            {
                                // 动画已存在，检查主GameObject的动画组件是否有修改
                                if (HasMainAnimationBeenModified())
                                {
                                    if (EditorUtility.DisplayDialog("保存修改", 
                                        "当前动画组件有未保存的修改，是否保存到当前动画配置？", 
                                        "保存", "放弃"))
                                    {
                                        // 保存主GameObject的动画组件到当前正在编辑的动画配置
                                        SaveMainAnimationToTarget(currentEditingAnimationName);
                                    }
                                }
                                
                                // 切换到已存在的动画配置
                                SwitchToExistingAnimation(animationName);
                            }
                            else
                            {
                                // 动画不存在，创建新动画
                                CreateAnimationToState(fromState, toState);
                            }
                        });
                        
                        var buttonText = hasAnimation ? "✓" : "+";
                        button.text = buttonText;
                        button.style.width = 80;
                        
                        // 设置按钮颜色
                        if (hasAnimation)
                        {
                            if (isCurrentAnimation)
                            {
                                // 当前显示的动画，亮绿色
                                button.style.backgroundColor = new Color(0.2f, 0.8f, 0.2f, 1f); // 亮绿色
                            }
                            else
                            {
                                // 已创建但不是当前显示的动画，暗绿色
                                button.style.backgroundColor = new Color(0.1f, 0.5f, 0.1f, 1f); // 暗绿色
                            }
                        }
                        else
                        {
                            // 未创建，亮灰色
                            button.style.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1f); // 亮灰色
                        }
                        
                        row.Add(button);
                    }
                }
                
                table.Add(row);
            }
            
            container.Add(table);
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
                        // 切换状态时清空输入状态名
                        inputStateName = null;
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                componentMenu.ShowAsContext();
            }
            string newState = EditorGUILayout.TextField(aSUIStyleState.CurrentState);
            if (newState != aSUIStyleState.CurrentState)
            {
                // 记录用户输入的状态名，但不立即切换状态
                inputStateName = newState;
                isDirty = true;
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
                // 使用用户输入的状态名，如果没有输入则使用当前状态名
                string stateNameToSave = !string.IsNullOrEmpty(inputStateName) ? inputStateName : aSUIStyleState.CurrentState;
                
                if (string.IsNullOrEmpty(stateNameToSave))
                {
                    EditorUtility.DisplayDialog("保存样式", "没有命名样式,请命名后保存", "ok");
                }
                else
                {
                    var existingState = aSUIStyleState.GetStyleState(stateNameToSave);
                    if (existingState != null && !EditorUtility.DisplayDialog("保存样式", $"样式 {stateNameToSave} 已存在，确定覆盖样式吗？", "确定", "取消"))
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
                    
                    aSUIStyleState.AddOrUpdateState(stateNameToSave, componentStyles);
                    // 保存成功后，应用新状态并清空输入状态名
                    _ = aSUIStyleState.ApplyState(stateNameToSave);
                    inputStateName = null;
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

        /// <summary>
        /// 绘制AnimTo按钮
        /// </summary>
        private void DrawAnimToButton()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("动画转换矩阵", EditorStyles.boldLabel);
            
            var stateList = aSUIStyleState.StyleStates.Select(s => s.stateName).ToList();
            if (stateList.Count < 2)
            {
                EditorGUILayout.HelpBox("需要至少2个状态才能创建动画转换", MessageType.Info);
                return;
            }
            
            // 绘制状态转换矩阵
            DrawAnimationMatrix(stateList);
        }

        /// <summary>
        /// 绘制动画转换矩阵
        /// </summary>
        private void DrawAnimationMatrix(List<string> stateList)
        {
            var animRoot = aSUIStyleState.transform.Find("_Anim");
            var existingAnimations = new HashSet<string>();
            
            // 获取已存在的动画配置
            if (animRoot != null)
            {
                for (int i = 0; i < animRoot.childCount; i++)
                {
                    var child = animRoot.GetChild(i);
                    if (child.GetComponent<LitMotion.Animation.LitMotionAnimation>() != null)
                    {
                        existingAnimations.Add(child.name);
                    }
                }
            }
            
            // 绘制矩阵表头
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("从\\到", GUILayout.Width(60));
            foreach (var state in stateList)
            {
                EditorGUILayout.LabelField(state, GUILayout.Width(80), GUILayout.Height(20));
            }
            EditorGUILayout.EndHorizontal();
            
            // 绘制矩阵内容
            for (int i = 0; i < stateList.Count; i++)
            {
                var fromState = stateList[i];
                EditorGUILayout.BeginHorizontal();
                
                // 行标签
                EditorGUILayout.LabelField(fromState, GUILayout.Width(60), GUILayout.Height(20));
                
                // 绘制每个转换按钮
                for (int j = 0; j < stateList.Count; j++)
                {
                    var toState = stateList[j];
                    var animationName = $"{fromState}To{toState}";
                    var hasAnimation = existingAnimations.Contains(animationName);
                    var isSameState = fromState == toState;
                    
                    // 设置按钮样式
                    var originalColor = GUI.backgroundColor;
                    if (hasAnimation)
                    {
                        GUI.backgroundColor = Color.green; // 已存在动画
                    }
                    else if (isSameState)
                    {
                        GUI.backgroundColor = Color.gray; // 相同状态
                    }
                    else
                    {
                        GUI.backgroundColor = Color.white; // 可创建动画
                    }
                    
                    // 绘制按钮
                    var buttonText = isSameState ? "-" : (hasAnimation ? "✓" : "+");
                    var buttonWidth = 80;
                    
                    if (GUILayout.Button(buttonText, GUILayout.Width(buttonWidth), GUILayout.Height(20)))
                    {
                        if (!isSameState)
                        {
                            if (hasAnimation)
                            {
                                // 动画已存在，检查主GameObject的动画组件是否有修改
                                if (HasMainAnimationBeenModified())
                                {
                                    if (EditorUtility.DisplayDialog("保存修改", 
                                        "当前动画组件有未保存的修改，是否保存到当前动画配置？", 
                                        "保存", "放弃"))
                                    {
                                        // 保存主GameObject的动画组件到当前正在编辑的动画配置
                                        SaveMainAnimationToTarget(currentEditingAnimationName);
                                    }
                                }
                                
                                // 切换到已存在的动画配置
                                SwitchToExistingAnimation(animationName);
                            }
                            else
                            {
                                // 动画不存在，创建新动画
                                CreateAnimationToState(fromState, toState);
                            }
                        }
                    }
                    
                    GUI.backgroundColor = originalColor;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            // 添加图例说明
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("图例:", GUILayout.Width(40));
            
            GUI.backgroundColor = Color.green;
            EditorGUILayout.LabelField("✓", GUILayout.Width(20));
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("已创建", GUILayout.Width(50));
            
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("+", GUILayout.Width(20));
            EditorGUILayout.LabelField("可创建", GUILayout.Width(50));
            
            GUI.backgroundColor = Color.gray;
            EditorGUILayout.LabelField("-", GUILayout.Width(20));
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("相同状态", GUILayout.Width(60));
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 创建到指定状态的动画
        /// </summary>
        private void CreateAnimationToState(string fromState, string toState)
        {

            // 获取当前状态和目标状态的样式数据
            var fromStateData = aSUIStyleState.GetStateData(fromState);
            var toStateData = aSUIStyleState.GetStateData(toState);

            if (fromStateData == null || toStateData == null)
            {
                EditorUtility.DisplayDialog("创建动画", "无法获取状态数据", "确定");
                return;
            }

            // 1. 查找或创建_Anim子节点
            var animRoot = GetOrCreateAnimRoot();

            // 2. 在_Anim下创建对应的{fromState}To{toState}空对象
            var animationName = $"{fromState}To{toState}";
            var animationObject = GetOrCreateAnimationObject(animRoot, animationName);

            // 3. 在空对象上创建LitMotionAnimation组件
            var litMotionAnimation = animationObject.GetComponent<LitMotion.Animation.LitMotionAnimation>();
            if (litMotionAnimation == null)
            {
                litMotionAnimation = animationObject.AddComponent<LitMotion.Animation.LitMotionAnimation>();
            }

            // 4. 初始化components数组
            ASUIStyleState.InitializeComponentsArray(litMotionAnimation);
            EditorUtility.DisplayDialog("创建动画", $"已创建动画: {animationName}", "确定");
            SwitchToExistingAnimation(animationName);
        }
        
        /// <summary>
        /// 获取或创建_Anim子节点
        /// </summary>
        private Transform GetOrCreateAnimRoot()
        {
            var animRoot = aSUIStyleState.transform.Find("_Anim");
            if (animRoot == null)
            {
                var animObject = new GameObject("_Anim");
                animObject.transform.SetParent(aSUIStyleState.transform);
                animRoot = animObject.transform;
            }
            return animRoot;
        }
        
        /// <summary>
        /// 获取或创建动画对象
        /// </summary>
        private GameObject GetOrCreateAnimationObject(Transform animRoot, string animationName)
        {
            var existingObject = animRoot.Find(animationName);
            if (existingObject != null)
            {
                return existingObject.gameObject;
            }
            
            var animationObject = new GameObject(animationName);
            animationObject.transform.SetParent(animRoot);
            return animationObject;
        }
        
        /// <summary>
        /// 创建主动画编辑器
        /// </summary>
        private VisualElement CreateMainAnimationEditor()
        {
            var container = new VisualElement();
            container.name = "main-animation-editor";
            
            if (mainAnimation == null) return container;
            
            // 添加标题
            var titleLabel = new Label("主动画配置");
            titleLabel.style.fontSize = 14;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            container.Add(titleLabel);
            
            // 创建或更新编辑器
            if (mainAnimationEditor == null || mainAnimationEditor.target != mainAnimation)
            {
                if (mainAnimationEditor != null)
                {
                    DestroyImmediate(mainAnimationEditor);
                }
                mainAnimationEditor = Editor.CreateEditor(mainAnimation);
            }
            
            // 使用LitMotionAnimation的原生UI Toolkit编辑器
            if (mainAnimationEditor != null)
            {
                var nativeEditor = mainAnimationEditor.CreateInspectorGUI();
                if (nativeEditor != null)
                {
                    container.Add(nativeEditor);
                }
            }
            
            return container;
        }
        
        /// <summary>
        /// 重新创建主动画编辑器
        /// </summary>
        private void RefreshMainAnimationEditor()
        {
            if (rootElement == null) return;
            
            // 清除现有的主动画编辑器
            var existingEditor = rootElement.Q("main-animation-editor");
            if (existingEditor != null)
            {
                existingEditor.RemoveFromHierarchy();
            }
            
            // 创建新的主动画编辑器
            var newEditor = CreateMainAnimationEditor();
            newEditor.name = "main-animation-editor";
            
            // 添加到根元素
            rootElement.Add(newEditor);
        }
        
        /// <summary>
        /// 重新创建动画矩阵
        /// </summary>
        private void RefreshAnimationMatrix()
        {
            if (rootElement == null) return;
            
            // 清除现有的动画矩阵
            var existingMatrix = rootElement.Q("animation-matrix");
            if (existingMatrix != null)
            {
                existingMatrix.RemoveFromHierarchy();
            }
            
            // 重新创建动画矩阵
            var stateList = aSUIStyleState.StyleStates.Select(s => s.stateName).ToList();
            if (stateList.Count >= 2)
            {
                var newMatrix = CreateAnimToButton();
                // 添加到根元素
                rootElement.Add(newMatrix);
            }
        }
        

        /// <summary>
        /// 检查主GameObject的动画组件是否被修改
        /// </summary>
        private bool HasMainAnimationBeenModified()
        {
            // 简单判断：如果当前正在编辑的动画配置不为空，说明用户可能修改了
            // 这是一个简化的判断，实际项目中可能需要更复杂的逻辑
            //return !string.IsNullOrEmpty(currentEditingAnimationName);
            return false;
        }
        
        /// <summary>
        /// 保存主GameObject的动画组件到目标动画组件
        /// </summary>
        private void SaveMainAnimationToTarget(string targetAnimationName)
        {
            if (mainAnimation == null) return;
            
            var animRoot = aSUIStyleState.transform.Find("_Anim");
            var targetObject = animRoot?.Find(targetAnimationName);
            if (targetObject == null) return;
            
            var targetAnimation = targetObject.GetComponent<LitMotion.Animation.LitMotionAnimation>();
            if (targetAnimation == null) return;
            
            // 将主动画组件的参数复制到目标动画组件
            #if UNITY_EDITOR
            EditorUtility.CopySerialized(mainAnimation, targetAnimation);
            EditorUtility.SetDirty(targetAnimation);
            #endif
        }
        
        /// <summary>
        /// 切换到已存在的动画配置
        /// </summary>
        private void SwitchToExistingAnimation(string animationName)
        {
            var animRoot = aSUIStyleState.transform.Find("_Anim");
            var configObject = animRoot?.Find(animationName);
            
            if (configObject == null) return;
            
            var sourceAnimation = configObject.GetComponent<LitMotion.Animation.LitMotionAnimation>();
            if (sourceAnimation == null) return;
            
            // 停止当前动画
            if (mainAnimation != null)
            {
                mainAnimation.Stop();
            }
            
            // 设置引用到目标动画组件
            mainAnimation = sourceAnimation;
            
            // 播放新动画
            if (sourceAnimation != null)
            {
                sourceAnimation.Play();
            }
            
            // 更新当前正在编辑的动画配置名称
            currentEditingAnimationName = animationName;
            
            // 重置dirty状态，因为这是切换而不是修改
            isDirty = false;
            // 重新创建动画矩阵以更新按钮颜色
            RefreshAnimationMatrix();
            // 刷新主动画编辑器
            RefreshMainAnimationEditor();
            // 标记为已修改，触发重绘
            EditorUtility.SetDirty(aSUIStyleState);
        }
    }
}




