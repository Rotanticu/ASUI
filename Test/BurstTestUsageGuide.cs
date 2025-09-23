using UnityEngine;

namespace ASUI.Test
{
    /// <summary>
    /// Burst性能测试使用指南
    /// </summary>
    public class BurstTestUsageGuide : MonoBehaviour
    {
        [Header("使用说明")]
        [TextArea(10, 20)]
        public string usageGuide = @"
=== Burst性能测试工具使用指南 ===

1. 创建测试UI:
   - 在场景中创建一个空的GameObject
   - 添加 BurstPerformanceUICreator 组件
   - 在Inspector中点击 '创建Burst性能测试UI' 按钮
   - 或者右键点击组件选择 '创建Burst性能测试UI'

2. 运行测试:
   - 在编辑器中运行: 可以看到JIT编译的性能
   - 构建项目后运行: 可以看到Burst编译的真实性能

3. 测试配置:
   - 使用滑块调整迭代次数 (10,000 - 1,000,000)
   - 点击'运行测试'开始性能测试
   - 点击'清除结果'重置测试结果

4. 性能对比:
   - 编辑器环境: 使用JIT编译，性能较低
   - 构建环境: 使用Burst编译，性能显著提升
   - 通常可以看到2-10倍的性能提升

5. 测试函数:
   - SpringSimple: 简易弹簧阻尼器
   - SpringElastic: 近似弹性弹簧阻尼器
   - SpringSimpleVelocitySmoothing: 速度平滑弹簧阻尼器
   - SpringSimpleDurationLimit: 时间限制弹簧阻尼器
   - SpringSimpleDoubleSmoothing: 双重平滑弹簧阻尼器

6. 构建要求:
   - Scripting Backend: IL2CPP
   - Api Compatibility Level: .NET Standard 2.1+
   - 确保Burst包已安装

7. 性能分析:
   - 结果会显示每个函数的执行时间
   - 操作数/秒 (ops/sec)
   - 每操作耗时 (μs/op)
   - 性能排名

注意事项:
- 在构建版本中才能看到Burst的真实性能
- 编辑器中的性能测试仅供参考
- 建议使用较大的迭代次数获得更准确的结果
- 不同平台和硬件会有不同的性能表现
";

        [ContextMenu("显示使用指南")]
        public void ShowUsageGuide()
        {
            Debug.Log(usageGuide);
        }
        
        [ContextMenu("创建测试UI")]
        public void CreateTestUI()
        {
            BurstPerformanceUICreator creator = FindFirstObjectByType<BurstPerformanceUICreator>();
            if (creator == null)
            {
                GameObject go = new GameObject("BurstTestCreator");
                creator = go.AddComponent<BurstPerformanceUICreator>();
            }
            
            creator.CreateBurstPerformanceUI();
        }
    }
}
