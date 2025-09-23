# Burst性能测试工具

这个工具用于测试SpringUtility中`[BurstCompile]`函数的性能表现，可以在构建项目中直接显示测试结果。

## 文件说明

- `BurstPerformanceUITest.cs` - 主要的性能测试脚本，包含UI逻辑和测试功能
- `BurstPerformanceUICreator.cs` - 自动创建测试UI界面的脚本
- `BurstTestUsageGuide.cs` - 使用指南和说明

## 快速开始

### 1. 创建测试UI

在Unity编辑器中：

1. 在场景中创建一个空的GameObject
2. 添加 `BurstPerformanceUICreator` 组件
3. 在Inspector中点击 **"创建Burst性能测试UI"** 按钮
4. 或者右键点击组件选择 **"创建Burst性能测试UI"**

### 2. 运行测试

#### 编辑器测试（JIT编译）
- 直接在编辑器中运行场景
- 点击 **"运行测试"** 按钮
- 查看JIT编译的性能结果

#### 构建测试（Burst编译）
1. **File → Build Settings**
2. 选择目标平台
3. 在 **Player Settings** 中确保：
   - **Scripting Backend**: IL2CPP
   - **Api Compatibility Level**: .NET Standard 2.1 或更高
4. 构建并运行项目
5. 在构建版本中运行相同的测试

## 功能特性

### 测试配置
- **迭代次数滑块**: 调整测试强度（10,000 - 1,000,000次迭代）
- **实时结果显示**: 测试过程中实时显示进度
- **详细性能数据**: 显示执行时间、操作数/秒、每操作耗时
- **使用Unity Text组件**: 避免TextMeshPro的空格后文本消失bug

### 测试函数
1. **SpringSimple** - 简易弹簧阻尼器 (float4版本)
2. **SpringElastic** - 近似弹性弹簧阻尼器 (float4版本)
3. **SpringSimpleVelocitySmoothing** - 速度平滑弹簧阻尼器 (float4版本)
4. **SpringSimpleDurationLimit** - 时间限制弹簧阻尼器 (float4版本)
5. **SpringSimpleDoubleSmoothing** - 双重平滑弹簧阻尼器 (float4版本)

### 对比测试
- **double版本测试**: 测试SpringUtility中double版本的函数性能
- **性能对比分析**: 对比double vs float4版本的性能差异
- **Burst编译效果**: 展示Burst编译 + SIMD加速的真实性能提升

### 性能指标
- **总执行时间** (毫秒)
- **操作数/秒** (ops/sec)
- **每操作耗时** (μs/op)
- **性能排名** (从快到慢)

## 预期性能提升

根据Burst文档，通常可以看到：
- **2-10倍**的性能提升（相比传统JIT编译）
- **SIMD指令**的充分利用（float4 vs double）
- **内存访问模式**的优化
- **double vs float4**: 在支持SIMD的平台上，float4通常比double快2-4倍

## 使用示例

```csharp
// 在代码中直接使用测试脚本
BurstPerformanceUITest testScript = FindFirstObjectByType<BurstPerformanceUITest>();
if (testScript != null)
{
    testScript.RunAllTests();
}
```

## 注意事项

1. **环境差异**：
   - 编辑器环境使用JIT编译，性能较低
   - 构建环境使用Burst编译，性能显著提升

2. **构建要求**：
   - 必须使用IL2CPP作为Scripting Backend
   - 需要.NET Standard 2.1或更高版本
   - 确保Burst包已正确安装

3. **测试准确性**：
   - 使用较大的迭代次数获得更准确的结果
   - 多次运行测试取平均值
   - 不同平台和硬件会有不同的性能表现

4. **性能分析**：
   - 使用Unity Profiler进行更详细的性能分析
   - 对比不同Spring函数的性能表现
   - 关注内存分配和GC压力

## 故障排除

### 常见问题

1. **UI不显示**：
   - 确保Canvas的Sorting Order足够高
   - 检查UI组件是否正确创建
   - 使用Unity Text组件而不是TextMeshPro，避免空格后文本消失的bug

2. **测试结果异常**：
   - 确保SpringUtility.cs中的函数已添加`[BurstCompile]`属性
   - 检查Unity.Mathematics.math的使用是否正确

3. **构建版本性能没有提升**：
   - 确认Scripting Backend设置为IL2CPP
   - 检查Burst包是否正确安装
   - 使用Burst Inspector查看编译状态

### 调试技巧

1. **使用Burst Inspector**：
   - Window → Analysis → Burst Inspector
   - 查看哪些函数被Burst编译

2. **性能分析**：
   - 使用Unity Profiler分析构建版本
   - 对比编辑器vs构建版本的性能差异

3. **日志输出**：
   - 在测试脚本中添加Debug.Log输出
   - 查看控制台中的详细测试信息

## 扩展功能

可以根据需要扩展测试功能：

1. **添加更多测试函数**
2. **支持不同数据类型**（float, double, int等）
3. **添加内存使用统计**
4. **支持批量测试和结果导出**
5. **添加性能趋势分析**

## 贡献

欢迎提交改进建议和bug报告！
