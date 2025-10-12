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
using UIElementsButton = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;

namespace ASUI
{
    [CustomEditor(typeof(ASUIStyleState))]
    public class ASUIStyleStateEditor : Editor
    {
        /// <summary>
        /// 当前正在编辑的动画配置名称
        /// </summary>
        private string currentEditingAnimationName;
        /// <summary>
        /// 主动画编辑器
        /// </summary>
        private LitMotion.Animation.Editor.LitMotionAnimationEditor mainAnimationEditor;
        /// <summary>
        /// 当前正在播放的主动画
        /// </summary>
        private LitMotion.Animation.LitMotionAnimation mainAnimation;
        /// <summary>
        /// 是否已修改
        /// </summary>
        private bool isDirty = false;
        /// <summary>
        /// 输入状态名
        /// </summary>
        private string inputStateName;
        /// <summary>
        /// 根元素引用，用于重新创建UI
        /// </summary>
        private VisualElement rootElement;
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
            
            isDirty = false;
            serializedObject.Update();

            // 按照之前的顺序创建UI
            root.Add(CreateASUIInfoListHead());
            root.Add(CreateASUIInfoReorderableList());
            root.Add(CreateDragArea());
            root.Add(CreateSwitchStateDropdownButton());
            root.Add(CreateStateStyleFolderList());
            root.Add(CreateSaveAndDeleteStateStyleButton());
            root.Add(CreateAnimToButton());
            root.Add(CreateMainAnimationEditor());

            // 如果有修改，就保存下
            if (isDirty)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(aSUIStyleState);
            }
            
            return root;
        }
        
        /// <summary>
        /// 刷新整个检查器
        /// </summary>
        private void RefreshInspector()
        {
            if (rootElement != null)
            {
                // 清除所有子元素
                rootElement.Clear();
                
                // 重新创建所有UI元素
                rootElement.Add(CreateASUIInfoListHead());
                rootElement.Add(CreateASUIInfoReorderableList());
                rootElement.Add(CreateDragArea());
                rootElement.Add(CreateSwitchStateDropdownButton());
                rootElement.Add(CreateStateStyleFolderList());
                rootElement.Add(CreateSaveAndDeleteStateStyleButton());
            }
        }
        
        /// <summary>
        /// 创建ASUIInfo列表表头
        /// </summary>
        private VisualElement CreateASUIInfoListHead()
        {
            var container = new VisualElement();
            container.name = "asui-info-list-head";
            
            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.marginBottom = 5;
            headerRow.style.paddingLeft = 5;
            headerRow.style.paddingRight = 5;
            headerRow.style.paddingTop = 3;
            headerRow.style.paddingBottom = 3;
            
            var nameLabel = new Label("Name");
            nameLabel.style.flexGrow = 1;
            nameLabel.style.marginRight = 5;
            nameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            headerRow.Add(nameLabel);
            
            var typeLabel = new Label("Type");
            typeLabel.style.width = 240;
            typeLabel.style.marginRight = 5;
            typeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            headerRow.Add(typeLabel);
            
            var componentLabel = new Label("Component");
            componentLabel.style.width = 240;
            componentLabel.style.marginRight = 5;
            componentLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            headerRow.Add(componentLabel);
            
            var deleteLabel = new Label("Delete");
            deleteLabel.style.width = 30;
            deleteLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            headerRow.Add(deleteLabel);
            
            container.Add(headerRow);
            return container;
        }
        
        /// <summary>
        /// 创建ASUIInfo可重排序列表
        /// </summary>
        private VisualElement CreateASUIInfoReorderableList()
        {
            var container = new VisualElement();
            container.name = "asui-info-reorderable-list";
            
            // 创建表格
            var table = new VisualElement();
            table.name = "component-table";
            
            for (int i = 0; i < aSUIStyleState.ComponentInfos.Count; i++)
            {
                int index = i; // 捕获索引用于回调
                var componentInfo = aSUIStyleState.ComponentInfos[index];
                
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.marginBottom = 5;
                row.style.paddingLeft = 5;
                row.style.paddingRight = 5;
                row.style.paddingTop = 3;
                row.style.paddingBottom = 3;
                
                // 名称输入框
                var nameField = new TextField();
                nameField.value = componentInfo.componentName;
                nameField.style.flexGrow = 1;
                nameField.style.marginRight = 5;
                nameField.RegisterValueChangedCallback(evt => {
                    var info = aSUIStyleState.ComponentInfos[index];
                    info.componentName = evt.newValue;
                    aSUIStyleState.ComponentInfos[index] = info;
                    EditorUtility.SetDirty(aSUIStyleState);
                });
                row.Add(nameField);
                
                // 组件ObjectField (只读模式)
                var componentField = new ObjectField();
                componentField.objectType = typeof(Component);
                componentField.value = componentInfo.component;
                componentField.style.width = 240;
                componentField.style.marginRight = 5;
                componentField.SetEnabled(false); // 设置为只读模式
                componentField.style.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 0.5f); // 灰色背景
                
                // 类型下拉菜单
                var typeDropdown = new DropdownField();
                var availableTypes = new List<string>();
                if (componentInfo.component != null)
                {
                    var components = componentInfo.component.GetComponents<Component>();
                    foreach (var comp in components)
                    {
                        availableTypes.Add(comp.GetType().Name);
                    }
                }
                typeDropdown.choices = availableTypes;
                typeDropdown.value = componentInfo.componentTypeName;
                typeDropdown.style.width = 240;
                typeDropdown.style.marginRight = 5;
                typeDropdown.RegisterValueChangedCallback(evt => {
                    var info = aSUIStyleState.ComponentInfos[index];
                    info.componentTypeName = evt.newValue;
                    if (componentInfo.component != null)
                    {
                        var components = componentInfo.component.GetComponents<Component>();
                        var selectedComponent = components.FirstOrDefault(c => c.GetType().Name == evt.newValue);
                        if (selectedComponent != null)
                        {
                            info.component = selectedComponent;
                            // 更新组件字段的显示
                            componentField.value = selectedComponent;
                        }
                    }
                    aSUIStyleState.ComponentInfos[index] = info;
                    EditorUtility.SetDirty(aSUIStyleState);
                });
                
                row.Add(typeDropdown);
                row.Add(componentField);
                
                // 删除按钮
                var deleteButton = new UIElementsButton(() => {
                    aSUIStyleState.ComponentInfos.RemoveAt(index);
                    EditorUtility.SetDirty(aSUIStyleState);
                    // 重新创建整个编辑器以刷新列表
                    RefreshInspector();
                });
                deleteButton.text = "×";
                deleteButton.style.width = 30;
                deleteButton.style.height = 20;
                deleteButton.style.backgroundColor = new Color(0.8f, 0.2f, 0.2f, 1f); // 红色
                deleteButton.style.color = Color.white;
                row.Add(deleteButton);
                
                table.Add(row);
            }
            
            container.Add(table);
            return container;
        }

        /// <summary>
        /// 创建拖拽区域
        /// </summary>
        private VisualElement CreateDragArea()
        {
            var container = new VisualElement();
            container.name = "drag-area";
            
            var dragArea = new VisualElement();
            dragArea.style.height = 100;
            dragArea.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            dragArea.style.borderLeftWidth = 2;
            dragArea.style.borderRightWidth = 2;
            dragArea.style.borderTopWidth = 2;
            dragArea.style.borderBottomWidth = 2;
            dragArea.style.borderLeftColor = Color.gray;
            dragArea.style.borderRightColor = Color.gray;
            dragArea.style.borderTopColor = Color.gray;
            dragArea.style.borderBottomColor = Color.gray;
            dragArea.style.justifyContent = Justify.Center;
            dragArea.style.alignItems = Align.Center;
            dragArea.style.marginBottom = 10;
            
            var dragLabel = new Label("拖拽到这里添加");
            dragLabel.style.fontSize = 18;
            dragLabel.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
            dragLabel.style.color = Color.white;
            dragArea.Add(dragLabel);
            
            // 处理拖拽事件
            dragArea.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            dragArea.RegisterCallback<DragPerformEvent>(OnDragPerform);
            
            container.Add(dragArea);
            return container;
        }
        
        /// <summary>
        /// 拖拽更新事件
        /// </summary>
        private void OnDragUpdated(DragUpdatedEvent evt)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            evt.StopPropagation();
        }
        
        /// <summary>
        /// 拖拽执行事件
        /// </summary>
        private void OnDragPerform(DragPerformEvent evt)
        {
                DragAndDrop.AcceptDrag();

            foreach (var obj in DragAndDrop.objectReferences)
                {
                if (obj is GameObject gameObject)
                    {
                    var rectTransform = gameObject.GetComponent<RectTransform>();
                    if (rectTransform != null)
                        {
                        string name = gameObject.name;
                        while (aSUIStyleState.ComponentInfos.Any(info => info.componentName == name))
                            {
                                string[] nameSplit = name.Split('_');
                                if (nameSplit.Length > 1)
                                    name = nameSplit[0] + $"_{int.Parse(nameSplit[nameSplit.Length - 1]) + 1}";
                                else
                                    name += "_1";
                            }
                        
                        // 优先选择特定类型的组件
                        var priorityTypes = new Type[] { typeof(Image), typeof(TextMeshProUGUI), typeof(RawImage), typeof(CanvasGroup) };
                        Component selectedComponent = null;
                        
                        foreach (var type in priorityTypes)
                        {
                            var component = gameObject.GetComponent(type);
                            if (component != null)
                            {
                                selectedComponent = component;
                                    break;
                                }
                            }
                        
                        if (selectedComponent == null)
                            selectedComponent = rectTransform;
                            
                        aSUIStyleState.AddComponentWithName(name, selectedComponent);
                    }
                }
            }
            
            EditorUtility.SetDirty(aSUIStyleState);
            serializedObject.ApplyModifiedProperties();
            
            // 重新创建整个编辑器以刷新列表
            RefreshInspector();
            
            evt.StopPropagation();
        }
        
        /// <summary>
        /// 创建切换状态的下拉菜单和当前状态名
        /// </summary>
        private VisualElement CreateSwitchStateDropdownButton()
        {
            var container = new VisualElement();
            container.name = "state-switch";
            
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginBottom = 5;
            
            // 状态切换下拉菜单
            var stateDropdown = new DropdownField();
                var stateList = aSUIStyleState.StyleStates.Select(s => s.stateName).ToList();
            stateDropdown.choices = stateList;
            if (stateList.Contains(aSUIStyleState.CurrentState))
                stateDropdown.value = aSUIStyleState.CurrentState;
            else if (stateList.Count > 0)
                stateDropdown.value = stateList[0];
            
            stateDropdown.RegisterValueChangedCallback(evt => {
                if (evt.newValue != aSUIStyleState.CurrentState)
                {
                    _ = aSUIStyleState.ApplyState(evt.newValue);
                        inputStateName = null;
                    EditorUtility.SetDirty(aSUIStyleState);
                    
                    // 重新创建整个编辑器以刷新样式属性
                    RefreshInspector();
                }
            });
            stateDropdown.style.flexGrow = 1;
            row.Add(stateDropdown);
            
            var space = new VisualElement();
            space.style.width = 20;
            row.Add(space);

            // 新状态输入
            var newStateLabel = new Label("新状态名:");
            newStateLabel.style.width = 60;
            row.Add(newStateLabel);
            
            var newStateField = new TextField();
            newStateField.value = inputStateName ?? "";
            newStateField.RegisterValueChangedCallback(evt => {
                inputStateName = evt.newValue;
                isDirty = true;
            });
            newStateField.style.flexGrow = 1;
            row.Add(newStateField);
            
            container.Add(row);
            
            return container;
        }
        
        /// <summary>
        /// 创建组件对应的样式文件夹
        /// </summary>
        private VisualElement CreateStateStyleFolderList()
        {
            var container = new VisualElement();
            container.name = "state-style-folders";
            
            var currentStateData = GetCurrentStateData();
            if (currentStateData == null)
            {
                var noDataLabel = new Label("没有找到当前状态的样式数据");
                noDataLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
                container.Add(noDataLabel);
                return container;
            }
            
            // 调试信息：显示当前状态和组件数量
            var debugLabel = new Label($"当前状态: {aSUIStyleState.CurrentState} (组件数量: {currentStateData.componentStyles.Count})");
            debugLabel.style.fontSize = 10;
            debugLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            container.Add(debugLabel);
            
            // 添加标题
            var titleLabel = new Label("样式编辑器");
            titleLabel.style.fontSize = 14;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 10;
            titleLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            container.Add(titleLabel);
            
            // 创建带边框的容器
            var borderedContainer = new VisualElement();
            borderedContainer.name = "style-editor-border";
            
            // 根据编辑器主题设置颜色
            bool isDarkTheme = EditorGUIUtility.isProSkin;
            Color backgroundColor = isDarkTheme ? new Color(0.2f, 0.2f, 0.2f, 1f) : new Color(0.95f, 0.95f, 0.95f, 1f);
            Color borderColor = isDarkTheme ? new Color(0.4f, 0.4f, 0.4f, 1f) : new Color(0.7f, 0.7f, 0.7f, 1f);
            
            borderedContainer.style.backgroundColor = backgroundColor;
            borderedContainer.style.borderLeftWidth = 2;
            borderedContainer.style.borderRightWidth = 2;
            borderedContainer.style.borderTopWidth = 2;
            borderedContainer.style.borderBottomWidth = 2;
            borderedContainer.style.borderLeftColor = borderColor;
            borderedContainer.style.borderRightColor = borderColor;
            borderedContainer.style.borderTopColor = borderColor;
            borderedContainer.style.borderBottomColor = borderColor;
            borderedContainer.style.borderTopLeftRadius = 5;
            borderedContainer.style.borderTopRightRadius = 5;
            borderedContainer.style.borderBottomLeftRadius = 5;
            borderedContainer.style.borderBottomRightRadius = 5;
            borderedContainer.style.paddingLeft = 10;
            borderedContainer.style.paddingRight = 10;
            borderedContainer.style.paddingTop = 10;
            borderedContainer.style.paddingBottom = 10;
            borderedContainer.style.marginBottom = 10;
                
            foreach (var componentStyle in currentStateData.componentStyles)
            {
                Component component = aSUIStyleState.GetComponentByUIName<Component>(componentStyle.componentName);
                if (component == null || componentStyle.style == null)
                    continue;

                if (!styleEditorFolderState.ContainsKey(component))
                    styleEditorFolderState.Add(component, false);

                var foldout = new Foldout();
                foldout.text = component.name;
                foldout.value = styleEditorFolderState.ContainsKey(component) ? styleEditorFolderState[component] : false;
                foldout.RegisterValueChangedCallback(evt => {
                    styleEditorFolderState[component] = evt.newValue;
                });
                
                // 为每个foldout添加美观的样式
                Color foldoutBackgroundColor = isDarkTheme ? new Color(0.3f, 0.3f, 0.3f, 0.8f) : new Color(1f, 1f, 1f, 0.8f);
                Color foldoutBorderColor = isDarkTheme ? new Color(0.5f, 0.5f, 0.5f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f);
                
                foldout.style.backgroundColor = foldoutBackgroundColor;
                foldout.style.borderLeftWidth = 1;
                foldout.style.borderRightWidth = 1;
                foldout.style.borderTopWidth = 1;
                foldout.style.borderBottomWidth = 1;
                foldout.style.borderLeftColor = foldoutBorderColor;
                foldout.style.borderRightColor = foldoutBorderColor;
                foldout.style.borderTopColor = foldoutBorderColor;
                foldout.style.borderBottomColor = foldoutBorderColor;
                foldout.style.borderTopLeftRadius = 3;
                foldout.style.borderTopRightRadius = 3;
                foldout.style.borderBottomLeftRadius = 3;
                foldout.style.borderBottomRightRadius = 3;
                foldout.style.paddingLeft = 20;
                foldout.style.paddingRight = 20;
                foldout.style.paddingTop = 5;
                foldout.style.paddingBottom = 5;
                foldout.style.marginBottom = 5;
                
                // 这里需要添加样式编辑器的内容
                // 由于UIElements的限制，可能需要使用PropertyField来显示样式属性
                var styleContainer = new VisualElement();
                styleContainer.style.marginLeft = 10;
                styleContainer.style.marginTop = 5;
                
                Color contentBackgroundColor = isDarkTheme ? new Color(0.4f, 0.4f, 0.4f, 0.5f) : new Color(1f, 1f, 1f, 0.5f);
                Color placeholderTextColor = isDarkTheme ? new Color(0.7f, 0.7f, 0.7f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f);
                
                styleContainer.style.backgroundColor = contentBackgroundColor;
                styleContainer.style.paddingLeft = 8;
                styleContainer.style.paddingRight = 8;
                styleContainer.style.paddingTop = 5;
                styleContainer.style.paddingBottom = 5;
                styleContainer.style.borderTopLeftRadius = 2;
                styleContainer.style.borderTopRightRadius = 2;
                styleContainer.style.borderBottomLeftRadius = 2;
                styleContainer.style.borderBottomRightRadius = 2;
                
                // 添加样式属性编辑
                var styleEditor = componentStyle.style.CreateUIElementsEditor(component);
                styleContainer.Add(styleEditor);
                
                foldout.Add(styleContainer);
                var space = new VisualElement();
                space.style.height = 20;
                borderedContainer.Add(foldout);
            }
            
            container.Add(borderedContainer);
            return container;
        }
        
        /// <summary>
        /// 创建保存和删除样式按钮
        /// </summary>
        private VisualElement CreateSaveAndDeleteStateStyleButton()
        {
            var container = new VisualElement();
            container.name = "save-delete-buttons";
            
            var buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Row;
            buttonRow.style.marginBottom = 10;
            
            // 保存样式按钮
            var saveButton = new UIElementsButton(() => {
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
                    _ = aSUIStyleState.ApplyState(stateNameToSave);
                    inputStateName = null;
                    isDirty = true;
                }
            });
            saveButton.text = "保存样式";
            saveButton.style.flexGrow = 1;
            saveButton.style.marginRight = 5;
            buttonRow.Add(saveButton);
            
            // 删除样式按钮
            var deleteButton = new UIElementsButton(() => {
                bool result = EditorUtility.DisplayDialog("删除样式", $"确定要删除样式 {aSUIStyleState.CurrentState} 吗？", "确定", "取消");
                if (result)
                {
                    aSUIStyleState.StyleStates.RemoveAll(s => s.stateName == aSUIStyleState.CurrentState);
                    isDirty = true;
                }
            });
            deleteButton.text = "删除样式";
            deleteButton.style.flexGrow = 1;
            buttonRow.Add(deleteButton);
            
            container.Add(buttonRow);
            return container;
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
                mainAnimationEditor = Editor.CreateEditor(mainAnimation) as LitMotion.Animation.Editor.LitMotionAnimationEditor;
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
            
            // 根据组件值是否相等决定是否添加动画组件
            SetAnimationValues(litMotionAnimation, fromState, toState);
            
            EditorUtility.DisplayDialog("创建动画", $"已创建动画: {animationName}", "确定");
            isDirty = true;
            SwitchToExistingAnimation(animationName);
        }
        
        /// <summary>
        /// 设置动画的起始值和结束值
        /// </summary>
        private void SetAnimationValues(LitMotion.Animation.LitMotionAnimation animation, string fromState, string toState)
        {
            var fromStateData = aSUIStyleState.GetStateData(fromState);
            var toStateData = aSUIStyleState.GetStateData(toState);
            
            if (fromStateData == null || toStateData == null) return;
            
            // 遍历所有组件，检查是否需要添加动画
            foreach (var componentInfo in aSUIStyleState.ComponentInfos)
            {
                if (componentInfo.component == null) continue;
                
                var fromStyle = fromStateData.FirstOrDefault(s => s.componentName == componentInfo.componentName);
                var toStyle = toStateData.FirstOrDefault(s => s.componentName == componentInfo.componentName);
                
                if (fromStyle?.style != null && toStyle?.style != null)
                {
                    // 使用起始样式的AddAnimationComponents方法，传入目标样式
                    fromStyle.style.AddAnimationComponents(animation, componentInfo.component, toStyle.style);
                }
            }
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