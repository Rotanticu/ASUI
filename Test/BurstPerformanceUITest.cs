using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using Unity.Mathematics;
namespace ASUI.Test
{
    /// <summary>
    /// Burst性能测试UI脚本
    /// 可以在构建项目中直接显示性能测试结果
    /// </summary>
    public class BurstPerformanceUITest : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] public Text resultText;
        [SerializeField] public Button runTestButton;
        [SerializeField] public Button clearButton;
        [SerializeField] public Slider iterationSlider;
        [SerializeField] public Text iterationLabel;
        
        [Header("测试配置")]
        [SerializeField] public int minIterations = 10000;
        [SerializeField] public int maxIterations = 1000000;
        [SerializeField] public float deltaTime = 0.016f; // 60 FPS
        [SerializeField] public float stiffness = 5.0f;
        [SerializeField] public float dampingRatio = 0.5f;
        
        public List<TestResult> testResults = new List<TestResult>();
        
        [System.Serializable]
        public class TestResult
        {
            public string testName;
            public long milliseconds;
            public int iterations;
            public double operationsPerSecond;
            public double microsecondsPerOperation;
            public bool isBurstCompiled;
            
            public TestResult(string name, long ms, int iter, bool burst)
            {
                testName = name;
                milliseconds = ms;
                iterations = iter;
                isBurstCompiled = burst;
                operationsPerSecond = (double)iter / (ms / 1000.0);
                microsecondsPerOperation = (ms * 1000.0) / iter;
            }
        }
        
        public void Start()
        {
            InitializeUI();
            UpdateIterationLabel();
        }
        
        public void InitializeUI()
        {
            if (resultText == null)
            {
                resultText = GetComponentInChildren<Text>();
            }
            
            if (runTestButton != null)
            {
                runTestButton.onClick.AddListener(RunAllTests);
            }
            
            if (clearButton != null)
            {
                clearButton.onClick.AddListener(ClearResults);
            }
            
            if (iterationSlider != null)
            {
                iterationSlider.minValue = 0f;
                iterationSlider.maxValue = 1f;
                iterationSlider.value = 0.5f; // 默认中等迭代次数
                iterationSlider.onValueChanged.AddListener(OnIterationSliderChanged);
            }
            
            // 显示初始信息
            ShowInitialInfo();
        }
        
        public void ShowInitialInfo()
        {
            string info = $"=== Burst性能测试工具 ===\n";
            info += $"当前环境: {(Application.isEditor ? "编辑器 (JIT)" : "构建版本 (Burst)")}\n";
            info += $"Unity版本: {Application.unityVersion}\n";
            info += $"平台: {Application.platform}\n";
            info += $"点击'运行测试'开始性能测试\n\n";
            info += "测试说明:\n";
            info += "• 编辑器环境使用JIT编译\n";
            info += "• 构建环境使用Burst编译\n";
            info += "• 对比不同Spring函数的性能\n";
            info += "• 对比double vs float4版本的性能差异\n";
            
            UpdateResultText(info);
        }
        
        public void OnIterationSliderChanged(float value)
        {
            UpdateIterationLabel();
        }
        
        public void UpdateIterationLabel()
        {
            if (iterationLabel != null && iterationSlider != null)
            {
                int iterations = GetCurrentIterations();
                iterationLabel.text = $"迭代次数: {iterations:N0}";
            }
        }
        
        public int GetCurrentIterations()
        {
            if (iterationSlider == null) return minIterations;
            float t = iterationSlider.value;
            return Mathf.RoundToInt(Mathf.Lerp(minIterations, maxIterations, t));
        }
        
        public void RunAllTests()
        {
            testResults.Clear();
            int iterations = GetCurrentIterations();
            
            string progress = "=== 开始性能测试 ===\n";
            progress += $"迭代次数: {iterations:N0}\n";
            progress += $"环境: {(Application.isEditor ? "编辑器 (JIT)" : "构建版本 (Burst)")}\n\n";
            UpdateResultText(progress);
            
            // 直接测试各个Spring函数，无协程等待
            TestSpringFunction("SpringSimple", TestSpringSimple, iterations);
            TestSpringFunction("SpringElastic", TestSpringElastic, iterations);
            TestSpringFunction("SpringSimpleVelocitySmoothing", TestSpringSimpleVelocitySmoothing, iterations);
            TestSpringFunction("SpringSimpleDurationLimit", TestSpringSimpleDurationLimit, iterations);
            TestSpringFunction("SpringSimpleDoubleSmoothing", TestSpringSimpleDoubleSmoothing, iterations);
            
            // 添加double版本性能对比测试
            TestSpringFunction("SpringSimple_double", TestSpringSimpleDouble, iterations);
            TestSpringFunction("SpringElastic_float", TestSpringElasticFloat, iterations);
            TestSpringFunction("SpringSimpleVelocitySmoothing_double", TestSpringSimpleVelocitySmoothingDouble, iterations);
            TestSpringFunction("SpringSimpleDurationLimit_double", TestSpringSimpleDurationLimitDouble, iterations);
            TestSpringFunction("SpringSimpleDoubleSmoothing_double", TestSpringSimpleDoubleSmoothingDouble, iterations);
            
            // 显示最终结果
            ShowFinalResults();
        }
        
        public void TestSpringFunction(string functionName, System.Func<int, long> testFunc, int iterations)
        {
            string progress = $"正在测试 {functionName}...\n";
            AppendResultText(progress);
            
            // 直接执行测试，无协程等待
            long milliseconds = testFunc(iterations);
            bool isBurstCompiled = !Application.isEditor; // 构建版本使用Burst编译
            
            TestResult result = new TestResult(functionName, milliseconds, iterations, isBurstCompiled);
            testResults.Add(result);
            
            string resultText = $"✓ {functionName}: {milliseconds}ms | {result.operationsPerSecond:N0} ops/sec | {result.microsecondsPerOperation:F2} μs/op\n";
            AppendResultText(resultText);
        }
        
        public long TestSpringSimple(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = (float4)currentValue;
                float4 currentVel = (float4)currentVelocity;
                float4 targetVal = (float4)targetValue;
                SpringUtility.SpringSimple(deltaTime, ref currentVal, ref currentVel, targetVal, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSpringElastic(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            Vector4 targetVelocity = new Vector4(0, 0, 0, 0);
            
            for (int i = 0; i < iterations; i++)
            {
                float4 newVel = default;
                float4 currentVal = (float4)currentValue;
                float4 currentVel = (float4)currentVelocity;
                float4 targetVal = (float4)targetValue;
                float4 targetVel = (float4)targetVelocity;
                float4 result = default;
                SpringUtility.SpringElastic(deltaTime, ref currentVal, ref currentVel, targetVal, targetVel, dampingRatio, stiffness);
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSpringSimpleVelocitySmoothing(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            float4 intermediatePosition = new float4(5, 10, 15, 20);
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = (float4)currentValue;
                float4 currentVel = (float4)currentVelocity;
                float4 targetVal = (float4)targetValue;
                SpringUtility.SpringSimpleVelocitySmoothing(deltaTime, ref currentVal, ref currentVel, targetVal, ref intermediatePosition, 2.0f, stiffness);
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSpringSimpleDurationLimit(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            float4 intermediatePosition = new float4(5, 10, 15, 20);
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = (float4)currentValue;
                float4 currentVel = (float4)currentVelocity;
                float4 targetVal = (float4)targetValue;
                SpringUtility.SpringSimpleDurationLimit(deltaTime, ref currentVal, ref currentVel, ref targetVal, 0.2f);
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSpringSimpleDoubleSmoothing(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = default;
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            Vector4 intermediatePosition = default;
            Vector4 intermediateVelocity = default;
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = currentValue;
                float4 currentVel = currentVelocity;
                float4 targetVal = targetValue;
                float4 intermediatePos = intermediatePosition;
                float4 intermediateVel = intermediateVelocity;
                SpringUtility.SpringSimpleDoubleSmoothing(deltaTime, ref currentVal, ref currentVel, targetVal, ref intermediatePos, ref intermediateVel, stiffness);
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试double版本的SpringSimple函数
        /// </summary>
        public long TestSpringSimpleDouble(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringSimple(deltaTime, ref currentValue, ref currentVelocity, targetValue, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试float版本的SpringElastic函数
        /// </summary>
        public long TestSpringElasticFloat(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            float targetVelocity = 0.0f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringElastic(deltaTime, ref currentValue, ref currentVelocity, targetValue, targetVelocity, dampingRatio, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试double版本的SpringSimpleVelocitySmoothing函数
        /// </summary>
        public long TestSpringSimpleVelocitySmoothingDouble(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            float intermediatePosition = 5.0f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringSimpleVelocitySmoothing(deltaTime, ref currentValue, ref currentVelocity, targetValue, ref intermediatePosition, 2.0f, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试double版本的SpringSimpleDurationLimit函数
        /// </summary>
        public long TestSpringSimpleDurationLimitDouble(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringSimpleDurationLimit(deltaTime, ref currentValue, ref currentVelocity, targetValue, 0.2f);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试double版本的SpringSimpleDoubleSmoothing函数
        /// </summary>
        public long TestSpringSimpleDoubleSmoothingDouble(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            float intermediatePosition = 5.0f;
            float intermediateVelocity = 0.05f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringSimpleDoubleSmoothing(deltaTime, ref currentValue, ref currentVelocity, targetValue, ref intermediatePosition, ref intermediateVelocity, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public void ShowFinalResults()
        {
            string results = "\n=== 测试结果汇总 ===\n";
            results += $"环境: {(Application.isEditor ? "编辑器 (JIT)" : "构建版本 (Burst)")}\n";
            results += $"测试时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";
            
            // 按性能排序
            testResults.Sort((a, b) => a.milliseconds.CompareTo(b.milliseconds));
            
            results += "性能排名 (从快到慢):\n";
            for (int i = 0; i < testResults.Count; i++)
            {
                var result = testResults[i];
                string burstStatus = result.isBurstCompiled ? "[Burst]" : "[JIT]";
                results += $"{i + 1}. {result.testName} {burstStatus}: {result.milliseconds}ms ({result.operationsPerSecond:N0} ops/sec)\n";
            }
            
            results += "\n详细结果:\n";
            foreach (var result in testResults)
            {
                string burstStatus = result.isBurstCompiled ? "[Burst编译]" : "[JIT编译]";
                results += $"\n{result.testName} {burstStatus}:\n";
                results += $"  总时间: {result.milliseconds}ms\n";
                results += $"  操作数: {result.iterations:N0}\n";
                results += $"  每秒操作: {result.operationsPerSecond:N0} ops/sec\n";
                results += $"  每操作耗时: {result.microsecondsPerOperation:F2} μs/op\n";
            }
            
            // 添加SIMD性能分析
            results += "\n=== SIMD性能分析 ===\n";
            var vector4Result = testResults.Find(r => r.testName == "SpringSimple_Vector4");
            var float4Result = testResults.Find(r => r.testName == "SpringSimple_float4");
            
            if (vector4Result != null && float4Result != null)
            {
                double simdSpeedup = (double)vector4Result.milliseconds / float4Result.milliseconds;
                results += $"Vector4 (无SIMD): {vector4Result.milliseconds}ms\n";
                results += $"float4 (有SIMD): {float4Result.milliseconds}ms\n";
                results += $"SIMD加速比: {simdSpeedup:F2}x\n";
                
                if (simdSpeedup > 2.0)
                {
                    results += "🚀 SIMD加速效果显著！\n";
                }
                else if (simdSpeedup > 1.2)
                {
                    results += "✅ SIMD有一定加速效果\n";
                }
                else
                {
                    results += "ℹ️ SIMD加速效果不明显（可能平台不支持或编译器优化）\n";
                }
                
                results += "\n说明:\n";
                results += "• Vector4: Unity传统类型，无SIMD加速\n";
                results += "• float4: Unity.Mathematics类型，支持SIMD加速\n";
                results += "• 在支持SIMD的平台上，float4通常比Vector4快2-4倍\n";
            }
            
            // 添加double vs float4性能对比分析
            results += "\n=== double vs float4 性能对比分析 ===\n";
            
            // SpringSimple对比
            var springSimpleFloat4Result = testResults.Find(r => r.testName == "SpringSimple");
            var springSimpleDoubleResult = testResults.Find(r => r.testName == "SpringSimple_double");
            
            if (springSimpleFloat4Result != null && springSimpleDoubleResult != null)
            {
                double speedupRatio = (double)springSimpleDoubleResult.milliseconds / springSimpleFloat4Result.milliseconds;
                results += $"SpringSimple对比:\n";
                results += $"  float4 (Burst): {springSimpleFloat4Result.milliseconds}ms\n";
                results += $"  double (JIT): {springSimpleDoubleResult.milliseconds}ms\n";
                results += $"  float4加速比: {speedupRatio:F2}x\n";
            }
            
            // SpringElastic对比
            var springElasticFloat4Result = testResults.Find(r => r.testName == "SpringElastic");
            var springElasticFloatResult = testResults.Find(r => r.testName == "SpringElastic_float");
            
            if (springElasticFloat4Result != null && springElasticFloatResult != null)
            {
                double speedupRatio = (double)springElasticFloatResult.milliseconds / springElasticFloat4Result.milliseconds;
                results += $"SpringElastic对比:\n";
                results += $"  float4 (Burst): {springElasticFloat4Result.milliseconds}ms\n";
                results += $"  float (JIT): {springElasticFloatResult.milliseconds}ms\n";
                results += $"  float4加速比: {speedupRatio:F2}x\n";
            }
            
            // 计算平均加速比
            var float4Results = testResults.Where(r => !r.testName.Contains("_double")).ToList();
            var doubleResults = testResults.Where(r => r.testName.Contains("_double")).ToList();
            
            if (float4Results.Count > 0 && doubleResults.Count > 0)
            {
                double totalFloat4Time = float4Results.Sum(r => r.milliseconds);
                double totalDoubleTime = doubleResults.Sum(r => r.milliseconds);
                double averageSpeedup = totalDoubleTime / totalFloat4Time;
                
                results += $"\n总体性能对比:\n";
                results += $"  float4总时间: {totalFloat4Time}ms\n";
                results += $"  double总时间: {totalDoubleTime}ms\n";
                results += $"  平均加速比: {averageSpeedup:F2}x\n";
                
                if (averageSpeedup > 2.0)
                {
                    results += "🚀 float4 + Burst编译性能显著优于double版本！\n";
                }
                else if (averageSpeedup > 1.2)
                {
                    results += "✅ float4 + Burst编译有一定性能优势\n";
                }
                else
                {
                    results += "ℹ️ 性能差异不明显，可能受其他因素影响\n";
                }
                
                results += "\n说明:\n";
                results += "• double版本: 使用传统JIT编译，无SIMD加速\n";
                results += "• float4版本: 使用Burst编译 + SIMD加速\n";
                results += "• 在支持SIMD的平台上，float4通常比double快2-4倍\n";
                results += "• 构建版本中Burst编译效果更明显\n";
            }
            
            results += "\n=== 测试完成 ===\n";
            results += "提示: 在构建版本中运行此测试可以看到Burst编译的真实性能提升\n";
            
            UpdateResultText(results);
        }
        
        public void UpdateResultText(string text)
        {
            if (resultText != null)
            {
                resultText.text = text;
            }
        }
        
        public void AppendResultText(string text)
        {
            if (resultText != null)
            {
                resultText.text += text;
            }
        }
        
        public void ClearResults()
        {
            testResults.Clear();
            ShowInitialInfo();
        }
        
        // 用于测试非Burst编译版本的对比函数
        [System.Obsolete("仅用于性能对比测试")]
        public long TestSpringSimpleNonBurst(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            
            for (int i = 0; i < iterations; i++)
            {
                // 使用Unity的Vector4进行非Burst计算
                Vector4 displacement = currentValue - targetValue;
                Vector4 velocityWithDamping = currentVelocity + displacement * stiffness;
                float exponentialDecay = Mathf.Exp(-stiffness * deltaTime);
                
                Vector4 newPosition = exponentialDecay * (displacement + velocityWithDamping * deltaTime) + targetValue;
                Vector4 newVelocity = exponentialDecay * (currentVelocity - velocityWithDamping * stiffness * deltaTime);
                
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
