# AGENTS.md

## Agent 行为规则

- **内部推理使用中文**。
- **涉及 Unity 编辑器操作时**，明确告诉用户需要进行的操作（如挂组件、拖拽引用、点击按钮等）。
- **写完代码后不检查编译**，无需运行 Unity 或验证编译结果。

## 项目概览

Unity 6 (6000.0.x) 卡牌/肉鸽游戏，使用 QFramework 架构。
语言：C# (Unity)、ShaderLab。
脚本后端：Mono / IL2CPP。

### QFramework 版本说明

本项目使用完整的 QFramework.Toolkits（包含 QFramework.cs 架构 + 全部工具集）。四层架构与 CQRS 模式是项目核心设计范式。所有 System、Model 统一注册到 `Architecture<T>.Init()` 中，Utility 按类型分别在 `Init()` 或场景 MonoBehaviour 的 `Awake()` 中注册。

**QFramework 官方文档：** `Doc/QFramework v1.0.92 使用指南 .md`

### 项目目录结构

```
Assets/
├── Scripts/              # 游戏业务代码
│   ├── Core/             # 核心层（Domain/Infrastructure）
│   ├── Features/         # 功能模块（Card/Combat/Enemy/Hero...）
│   ├── Presentation/     # 表现层（UI/View/Effects）
│   ├── Services/         # 服务层（Factories...）
│   ├── Configuration/    # 配置（Excel 数据）
│   └── Main/             # GM 工具
├── Editor/               # 编辑器工具
├── QFramework/           # QFramework 框架本体
├── QFrameworkData/       # QFramework 自动生成代码（QAssets.cs）
└── GameResource/         # 游戏资源
```

## 构建 / 测试 / 代码分析命令

本项目为 Unity 项目，所有命令通过 Unity Editor 或 CLI 运行。

**运行所有 Edit Mode 测试：**
```
"C:\Program Files\Unity\Hub\Editor\6000.0.25f1\Editor\Unity.exe" -runTests -testPlatform EditMode -projectPath "E:\UnityProject\Test" -logFile - -testResults TestResults.xml
```

**运行所有 Play Mode 测试：**
```
Unity.exe -runTests -testPlatform PlayMode -projectPath "E:\UnityProject\Test" -logFile - -testResults TestResults.xml
```

**运行单个测试（按名称过滤）：**
```
Unity.exe -runTests -testPlatform EditMode -testFilter "命名空间.类名.方法名" -projectPath "E:\UnityProject\Test" -logFile - -testResults TestResults.xml
```

**构建 Windows 版本：**
```
Unity.exe -quit -batchmode -buildWindowsPlayer "Build/Game.exe" -projectPath "E:\UnityProject\Test"
```

> 注意：将 `6000.0.25f1` 替换为实际安装的 Unity 版本。测试框架使用 `com.unity.test-framework` 1.5.1 + NUnit。测试文件应放在 `Editor/` 文件夹或引用了 `nunit.framework` 的程序集中。

**代码分析：** Unity 内置分析器，或通过 `.editorconfig` 启用 Roslyn 分析器。

## 代码风格规范

### 命名空间

- `Assets/Scripts/` 下的代码使用与文件夹层级一致的 PascalCase 命名空间。
- 示例：`Core.Infrastructure.Extensions`、`Editor.Excel`。
- QFramework 代码使用 `QFramework` 命名空间。
- 新文件使用块范围命名空间（block-scoped namespace）。
- 因项目使用 C# 9.0（Unity 默认），不支持 file-scoped namespace。

### 导入（using）

- `using` 指令放在文件顶部，分组顺序为：System > Unity > 第三方 > 项目。
- 除非有明确区分，否则 `using` 组之间不空行。
- 不使用 `using static`（工具类除外）。
- 不使用 `global using`。

### 格式化

- 缩进：4 个空格（不使用 Tab）。
- 大括号：新行（Allman 风格），用于类、方法、控制块。
- 每行一条语句。
- 行宽软限制：120 字符。
- 方法定义之间空一行。
- 使用 `#region` / `#endregion` 对相关成员分组，但尽量保持简短。

### 类型与变量

| 类别 | 规范 | 示例 |
|---|---|---|
| 私有字段（非序列化） | `m` 前缀 + PascalCase | `private int mCount;` |
| 私有字段（序列化） | `[SerializeField] private` + camelCase | `[SerializeField] private float moveSpeed;` |
| 公有字段 | PascalCase，优先使用属性 | `public int Health { get; set; }` |
| 局部变量 | camelCase | `playerCount` |
| 方法参数 | camelCase | `playerData` |
| 常量 | PascalCase | `public const int MaxCount = 10;` |
| 静态只读 | PascalCase | `public static readonly string Path;` |
| `var` | 右侧类型明显时可用，原始类型不用 | `var list = new List<int>();`（不用 `var i = 0;`） |

> **序列化字段 vs 私有字段：** `[SerializeField] private` 字段用 camelCase（无 `m` 前缀），普通 `private` 字段用 `m` + PascalCase。区分标准：Inspector 可见 vs 不可见。

### 命名规范

| 元素 | 规范 | 示例 |
|---|---|---|
| 类 / 结构体 | PascalCase | `GameManager`、`PlayerData` |
| 接口 | PascalCase，`I` 前缀 | `ISaveable`、`IController` |
| 方法 | PascalCase | `OnInit()`、`Execute()` |
| 属性 | PascalCase | `Instance`、`Health` |
| 私有字段（非序列化） | `m` + PascalCase | `mInstance`、`mDataList` |
| 私有字段（序列化） | `[SerializeField] private` + camelCase | `[SerializeField] private float moveSpeed;` |
| 局部变量 | camelCase | `playerCount` |
| 方法参数 | camelCase | `playerData` |
| 常量 | PascalCase | `MaxHealth` |
| 枚举 | PascalCase，单数 | `PlayerState`、`DamageType` |
| 枚举值 | PascalCase | `Idle`、`Walking` |
| 事件 / 委托 | PascalCase | `OnHealthChanged` |

---

## QFramework.cs 架构篇

### 一、四层架构与能力矩阵

QFramework 提供四个层级，自顶向下：

```
IController   (表现层)  ← 接收输入，更新 View
ISystem       (系统层)  ← 跨 Controller 的共享逻辑（成就、计时、商城等）
IModel        (数据层)  ← 共享数据的定义与增删改查
IUtility      (工具层)  ← 基础设施封装（存储、网络、SDK 等）
```

**各层级能力矩阵：**

| 能力 | IController | ISystem | IModel | IUtility |
|---|---|---|---|---|
| 获取 System | ✓ | ✓ | | |
| 获取 Model | ✓ | ✓ | | |
| 获取 Utility | | | ✓ | |
| 发送 Command | ✓ | | | |
| 发送 Query | ✓ | ✓ | | |
| 监听 Event | ✓ | ✓ | | |
| 发送 Event | | ✓ | ✓ | |

**接口定义：**

```csharp
// IController
public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem,
    ICanGetModel, ICanRegisterEvent, ICanSendQuery
{
}

// ISystem
public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel,
    ICanGetUtility, ICanRegisterEvent, ICanSendEvent, ICanGetSystem
{
    void Init();
}

// IModel
public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent
{
    void Init();
}

// IUtility —— 无任何框架能力，最底层
public interface IUtility
{
}
```

**基类：** `AbstractSystem`、`AbstractModel`、`AbstractCommand`、`AbstractQuery<T>`

---

### 二、核心数据流（CQRS 简化版）

```
用户输入 → IController.SendCommand<T>() → Command 修改 Model
    ↓
Model 数据变更 → 发送 Event 或 BindableProperty 自动通知
    ↓
IController 监听 Event/BindableProperty → 更新 View
```

**Command 与 Query 对比：**

| | Command（写） | Query（读） |
|---|---|---|
| 基类 | `AbstractCommand` | `AbstractQuery<T>` |
| 实现方法 | `OnExecute()` | `OnDo()` 返回 `T` |
| 职责 | 增、删、改 | 查 |
| 可获取 | System、Model | System、Model |
| 可发送 | Event、Command | Query |

**Command 规范：**
- 继承 `AbstractCommand`，覆写 `OnExecute()`
- 无参 Command 支持泛型发送：`this.SendCommand<IncreaseCountCommand>()`
- 有参 Command 通过构造函数传参：`this.SendCommand(new DecreaseCountCommand(value))`
- 修改 Model 数据后发送数据变更事件，或通过 BindableProperty 自动通知
- Command 不能持有状态字段

**Query 规范：**
- 继承 `AbstractQuery<T>`，覆写 `OnDo()` 返回 `T`
- 用于组合查询多个 Model，或查询逻辑较重时封装
- 查询逻辑不重时可不用 Query，直接在 Controller 中 `GetModel` 后读取即可

---

### 三、层级访问规则（强制）

| 规则 | 说明 |
|---|---|
| 上层可获取下层 | IController 可 GetModel/GetSystem；ISystem 可 GetModel；IModel 可 GetUtility |
| 下层不可获取上层 | IUtility 不可获取 Model/System/Controller；IModel 不可获取 System/Controller |
| IController 改状态必须用 Command | 不可直接调 `model.XXX = value`，必须 `this.SendCommand<XXXCommand>()` |
| ISystem/IModel 通知 IController 用 Event/BindableProperty | 不可直接调 `controller.UpdateView()` |
| 上层向下层通信用方法调用（查询）或 Command（状态变更） | |
| 下层向上层通信用 Event 或 BindableProperty | |

---

### 四、Architecture 注册规范

```csharp
public class GameMain : Architecture<GameMain>
{
    protected override void Init()
    {
        // 注册 System（接口 + 实现，支持依赖倒置）
        this.RegisterSystem<IScoreSystem>(new ScoreSystem());

        // 注册 Model（接口 + 实现）
        this.RegisterModel<IGameModel>(new GameModel());

        // 注册全局/无状态 Utility
        this.RegisterUtility<IStorage>(new PlayerPrefsStorage());
    }
}
```

| 类型 | 注册位置 | 示例 |
|---|---|---|
| System | `Init()` | 成就、计时、随机数 |
| Model | `Init()` | 玩家数据、配置数据 |
| 无状态 Utility | `Init()` | 存储(Storage)、日志(Logger) |
| 持有场景引用的 Utility | Controller 的 `Awake()` | CursorDisplay、TargetSelector、CardViewPool |

运行时注册场景 Utility：
```csharp
GameMain.Interface.RegisterUtility<ICursorDisplay>(new CursorDisplay());
```

**Architecture 即架构图：** `Init()` 方法集中展示了项目中所有模块的注册，本身就是项目的架构文档。

---

### 五、依赖倒置原则（接口设计模块，推荐）

所有模块注册和获取统一通过接口：

```csharp
// 1. 定义接口
public interface ICounterAppModel : IModel
{
    BindableProperty<int> Count { get; }
}
public interface IAchievementSystem : ISystem { }
public interface IStorage : IUtility
{
    void SaveInt(string key, int value);
    int LoadInt(string key, int defaultValue = 0);
}

// 2. 实现
public class CounterAppModel : AbstractModel, ICounterAppModel { ... }
public class AchievementSystem : AbstractSystem, IAchievementSystem { ... }
public class Storage : IStorage { ... }

// 3. 注册
this.RegisterModel<ICounterAppModel>(new CounterAppModel());
this.RegisterSystem<IAchievementSystem>(new AchievementSystem());
this.RegisterUtility<IStorage>(new Storage());

// 4. 使用
var model = this.GetModel<ICounterAppModel>();
var storage = this.GetUtility<IStorage>();
```

**好处：** 替换实现（如从 PlayerPrefs 切换到 EasySave）只需改注册处一行代码。

---

### 六、Command 拦截（中间件）

覆写 `Architecture<T>` 的 `ExecuteCommand` 方法：

```csharp
protected override void ExecuteCommand(ICommand command)
{
    Debug.Log("Before " + command.GetType().Name + " Execute");
    base.ExecuteCommand(command);
    Debug.Log("After " + command.GetType().Name + " Execute");
}
```

用途：日志、权限校验、撤销(Undo)、自动化测试。

---

### 七、EditorWindow 复用底层三层

`EditorWindow` 实现 `IController` 即可复用全部 System/Model/Utility：

```csharp
public class EditorCounterAppWindow : EditorWindow, IController
{
    public IArchitecture GetArchitecture() => CounterApp.Interface;
}
```

---

### 八、纸上设计

开发前先画两类图：
- **功能图**：Command → Model → Event → View 的数据流向
- **架构图**：每个模块所属层级的分布

不要求 UML，方块箭头即可，目的是梳理思路和团队沟通。

---

### 九、完整代码范例（CounterApp 最终形态）

```csharp
// === Model ===
public interface ICounterAppModel : IModel
{
    BindableProperty<int> Count { get; }
}
public class CounterAppModel : AbstractModel, ICounterAppModel
{
    public BindableProperty<int> Count { get; } = new BindableProperty<int>();

    protected override void OnInit()
    {
        var storage = this.GetUtility<IStorage>();
        Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));
        Count.Register(newCount => storage.SaveInt(nameof(Count), newCount));
    }
}

// === System ===
public interface IAchievementSystem : ISystem { }
public class AchievementSystem : AbstractSystem, IAchievementSystem
{
    protected override void OnInit()
    {
        this.GetModel<ICounterAppModel>().Count.Register(newCount =>
        {
            if (newCount == 10) Debug.Log("触发 点击达人 成就");
            else if (newCount == 20) Debug.Log("触发 点击专家 成就");
        });
    }
}

// === Utility ===
public interface IStorage : IUtility
{
    void SaveInt(string key, int value);
    int LoadInt(string key, int defaultValue = 0);
}
public class Storage : IStorage
{
    public void SaveInt(string key, int value) => PlayerPrefs.SetInt(key, value);
    public int LoadInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
}

// === Command ===
public class IncreaseCountCommand : AbstractCommand
{
    protected override void OnExecute() => this.GetModel<ICounterAppModel>().Count.Value++;
}
public class DecreaseCountCommand : AbstractCommand
{
    protected override void OnExecute() => this.GetModel<ICounterAppModel>().Count.Value--;
}

// === Architecture ===
public class CounterApp : Architecture<CounterApp>
{
    protected override void Init()
    {
        this.RegisterSystem<IAchievementSystem>(new AchievementSystem());
        this.RegisterModel<ICounterAppModel>(new CounterAppModel());
        this.RegisterUtility<IStorage>(new Storage());
    }
}

// === Controller ===
public class CounterAppController : MonoBehaviour, IController
{
    private ICounterAppModel mModel;

    void Start()
    {
        mModel = this.GetModel<ICounterAppModel>();

        mModel.Count.RegisterWithInitValue(_ => UpdateView())
            .UnRegisterWhenGameObjectDestroyed(gameObject);

        mBtnAdd.onClick.AddListener(() => this.SendCommand<IncreaseCountCommand>());
        mBtnSub.onClick.AddListener(() => this.SendCommand(new DecreaseCountCommand()));
    }

    void UpdateView() => mCountText.text = mModel.Count.ToString();
    public IArchitecture GetArchitecture() => CounterApp.Interface;
}
```

> **关键：** BindableProperty 替代了手动定义 `struct CountChangeEvent` + 手动 `SendEvent`。Controller 只做两件事：发送 Command + 监听 BindableProperty 更新 View。

---

## QFramework 核心工具规范

### TypeEventSystem（类型事件系统）— 跨层级通信首选

QFramework 架构内部默认使用此机制。

```csharp
// 事件体必须定义为 struct（减少 GC）
public struct CardPlayedEvent
{
    public CardData Data;
}

// 注册（返回 IUnRegister，可自动注销）
this.RegisterEvent<CardPlayedEvent>(e => HandleCard(e))
    .UnRegisterWhenGameObjectDestroyed(gameObject);

// 发送
this.SendEvent<CardPlayedEvent>();
this.SendEvent(new CardPlayedEvent { Data = data });
```

**事件继承：** 注册接口事件可接收所有实现该接口的 struct：

```csharp
public interface IEventA { }
public struct EventB : IEventA { }

TypeEventSystem.Global.Register<IEventA>(e => Debug.Log(e.GetType().Name));
TypeEventSystem.Global.Send<IEventA>(new EventB()); // 输出: EventB
```

**接口事件模式（`IOnEvent<T>`）：**

```csharp
public class MyCtrl : MonoBehaviour, IOnEvent<EventA>, IOnEvent<EventB>
{
    void Start()
    {
        this.RegisterEvent<EventA>().UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<EventB>().UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    public void OnEvent(EventA e) { }
    public void OnEvent(EventB e) { }
}
```

**手动注销：** `TypeEventSystem.Global.UnRegister<EventA>(handler)`。

**非 MonoBehaviour 自动注销：**
```csharp
public class NoneMonoScript : IUnRegisterList
{
    public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();

    void Start() => TypeEventSystem.Global.Register<EventA>(_ => { }).AddToUnregisterList(this);
    void OnDestroy() => this.UnRegisterAll();
}
```

---

### EasyEvent（轻量级事件）— 局部通信首选

性能接近 C# 委托，不需要声明事件类。

```csharp
private EasyEvent mOnClick = new EasyEvent();
private EasyEvent<int> mOnValueChanged = new EasyEvent<int>();
public class MyEvent : EasyEvent<int, int> { }
private MyEvent mMyEvent = new MyEvent();

// 注册
mOnClick.Register(() => { }).UnRegisterWhenGameObjectDestroyed(gameObject);
mOnValueChanged.Register(v => { }).UnRegisterWhenGameObjectDestroyed(gameObject);

// 触发
mOnClick.Trigger();
mOnValueChanged.Trigger(10);
```

**对比 TypeEventSystem：**

| | EasyEvent | TypeEventSystem |
|---|---|---|
| 声明事件类 | 不需要（或继承命名） | 必须定义 struct |
| 性能 | 接近委托 | 反射，稍弱 |
| 适用场景 | 局部系统内部通信 | 跨层级/跨模块通信 |
| 参数语义 | 无名称 | struct 字段有名称 |

**推荐：** 局部系统内部用 EasyEvent，跨层级/跨模块用 TypeEventSystem。

---

### BindableProperty（可绑定属性）— Model 暴露数据的首选

`数据 + 数据变更事件` 的一体封装。

```csharp
// 声明
public BindableProperty<int> Health { get; } = new BindableProperty<int>();

// 设置初始值（不触发事件）
Health.SetValueWithoutEvent(initialValue);

// 修改值（在 Command 中操作 .Value，自动触发通知）
model.Health.Value--;

// 监听变更（不复用初始值）
model.Health.Register(newValue => UpdateView())
    .UnRegisterWhenGameObjectDestroyed(gameObject);

// 监听 + 立即回调当前值（适合初始化显示）
model.Health.RegisterWithInitValue(newValue => UpdateView())
    .UnRegisterWhenGameObjectDestroyed(gameObject);
```

**对比手动 Event 模式：**

| | 手动 Event（CountChangeEvent） | BindableProperty |
|---|---|---|
| 需声明事件 struct | 是 | 否 |
| Command 需手动 SendEvent | 是 | 否（`.Value` 自动触发） |
| Controller 手动 UpdateView | 是 | 否（Register 自动回调） |
| 初始值显示 | 需手动调用 | RegisterWithInitValue |

**推荐：** 单值数据（血量、金币、分数）优先用 BindableProperty。集合数据（List/Dictionary 增删）用 EasyEvent。

---

### IOCContainer（控制反转容器）

本质 `Dictionary<Type, object>`。Architecture 的 RegisterModel/GetModel 底层即 IOCContainer。开发者一般无需直接使用。

```csharp
var container = new IOCContainer();
container.Register<INetworkService>(new NetworkService());
container.Get<INetworkService>().Connect();
```

---

## QFramework.Toolkits 工具集规范

### ResKit（资源管理）

**开发流程：**
1. 确保模拟模式勾选（Ctrl+E 面板）
2. 对资源文件夹/文件右键 → `@ResKit-AssetBundle Mark`
3. 代码中使用 `ResLoader`

```csharp
public class MyPanel : UIPanel
{
    private ResLoader mResLoader;

    protected override void OnInit(IUIData uiData = null)
    {
        // 项目启动时调用一次（在 GameRoot 或首个 UIPanel 中执行）
        ResKit.Init();

        // 每个脚本独立申请一个 ResLoader
        mResLoader = ResLoader.Allocate();

        // 同步加载：只传资源名
        var prefab = mResLoader.LoadSync<GameObject>("AssetObj");

        // 精确加载：传 AssetBundle 名 + 资源名
        var prefab2 = mResLoader.LoadSync<GameObject>("assetobj_prefab", "AssetObj");

        // 异步加载
        mResLoader.Add2Load("AssetObj", (success, res) => { })
                  .LoadAsync();
    }

    protected override void OnClose()
    {
        mResLoader.Recycle2Cache();
        mResLoader = null;
    }
}
```

**关键 API：**

| API | 用途 |
|---|---|
| `ResKit.Init()` | 项目启动时调用一次 |
| `ResLoader.Allocate()` | 每个脚本申请一个 |
| `mResLoader.LoadSync<T>(assetName)` | 同步（仅资源名） |
| `mResLoader.LoadSync<T>(abName, assetName)` | 同步（指定 AB 包） |
| `mResLoader.Add2Load(name, callback)` + `LoadAsync()` | 异步 |
| `mResLoader.Recycle2Cache()` | 释放引用（引用计数归零才真正卸载） |
| 资源名代码生成（QAssets.cs） | 避免拼写错误 |

---

### UIKit（界面管理）

**UIPanel 生命周期：** `OnInit` → `OnOpen` → `OnShow` → `OnHide` → `OnClose`

**常用 API：**

| API | 说明 |
|---|---|
| `UIKit.OpenPanel<T>(uiData)` | 同步打开 |
| `UIKit.OpenPanelAsync<T>(uiData)` | 异步打开（WebGL 必须） |
| `UIKit.ClosePanel<T>()` | 关闭 |
| `this.CloseSelf()` | 面板关闭自身 |
| `UIKit.HidePanel<T>()` / `ShowPanel<T>()` | 隐藏/显示 |
| `UIKit.GetPanel<T>()` | 获取实例 |
| `UIKit.Stack.Push()` / `UIPanel.Back()` | 界面堆栈 |

**UI 层级：** `UILevel.Common`（默认）、`UILevel.Forward`、`UILevel.UITop`、`UILevel.Guide`

**UIElement：** 子控件，Bind 设置 `标记类型=Element` 自动生成。

**推荐：** 每个界面一个独立测试场景，用 `UIPanelTester` 运行。

---

### CodeGenKit（代码生成）

**操作流程：**
1. 根节点挂 `ViewController`（快捷键 Alt+V），设命名空间和生成目录
2. 子节点挂 `Bind`（快捷键 Alt+B），选择要绑定的组件类型
3. 点击"生成代码"

**生成文件：**
- `XXX.cs`：手动逻辑文件，只生成一次，开发者在此写业务
- `XXX.Designer.cs`：自动生成文件，每次覆盖，包含绑定字段引用

**支持特性：**
- 嵌套 ViewController（子节点再挂 ViewController）
- 类型选择（Transform、SpriteRenderer、Button 等）
- 生成 Prefab（勾选 ViewController 上的生成 Prefab）
- 默认命名空间和生成目录在 Ctrl+E 面板的 CodeGenKit 设置中配置
- Pipeline 配置文件存储在 `Assets/QFrameworkData/CodeGenKit/`

---

### ActionKit（时序动作系统）

将动画、延时、资源加载、Tween、网络请求等时序任务统一管理。

```csharp
// 延时
ActionKit.Delay(1.0f, () => { }).Start(this);
ActionKit.DelayFrame(1, () => { }).Start(this);
ActionKit.NextFrame(() => { }).Start(this);

// 顺序执行
ActionKit.Sequence()
    .Callback(() => { })
    .Delay(1.0f)
    .Callback(() => { })
    .Start(this, () => Debug.Log("finish"));

// 并行执行
ActionKit.Parallel()
    .Delay(1.0f, () => { })
    .Delay(2.0f, () => { })
    .Start(this);

// 条件等待
ActionKit.Sequence()
    .Condition(() => Input.GetMouseButtonDown(0))
    .Callback(() => { })
    .Start(this);

// 重复
ActionKit.Repeat(5)
    .Condition(() => Input.GetMouseButtonDown(0))
    .Callback(() => { })
    .Start(this);
ActionKit.Repeat() // 无限重复
    .Callback(() => { })
    .Start(this);

// 协程
ActionKit.Coroutine(SomeCoroutine).Start(this);
SomeCoroutine().ToAction().Start(this);

// 自定义动作
ActionKit.Custom(a => a
    .OnStart(() => { })
    .OnExecute(dt => a.Finish())
    .OnFinish(() => { })
).Start(this);

// 嵌套组合
ActionKit.Sequence()
    .Parallel(p => p.Delay(1f, cb1).Delay(2f, cb2))
    .Sequence(s => s.Condition(cond).Callback(cb))
    .Start(this);
```

**全局 Mono 生命周期**（无需继承 MonoBehaviour）：

```csharp
ActionKit.OnUpdate.Register(() => { }).UnRegisterWhenGameObjectDestroyed(gameObject);
ActionKit.OnFixedUpdate.Register(() => { });
ActionKit.OnLateUpdate.Register(() => { });
ActionKit.OnGUI.Register(() => GUILayout.Label("hi"));
ActionKit.OnApplicationFocus.Register(focus => { });
ActionKit.OnApplicationPause.Register(pause => { });
ActionKit.OnApplicationQuit.Register(() => { });
```

**DOTween 集成**（需提前安装 DOTween）：

```csharp
ActionKit.Custom(c =>
{
    c.OnStart(() => transform.DOLocalMove(Vector3.one, 0.5f).OnComplete(c.Finish));
}).Start(this);

ActionKit.Sequence()
    .DOTween(() => transform.DOScale(Vector3.one, 0.5f))
    .Start(this);

DOVirtual.DelayedCall(2.0f, () => { }).ToAction().Start(this);
```

**UniRx 集成**（需提前安装 UniRx）：

```csharp
ActionKit.Custom(c =>
{
    c.OnStart(() => Observable.Timer(TimeSpan.FromSeconds(1f))
        .Subscribe(_ => c.Finish()));
}).Start(this);

Observable.Timer(TimeSpan.FromSeconds(2f)).ToAction().Start(this);

ActionKit.Sequence()
    .UniRx(() => Observable.Timer(TimeSpan.FromSeconds(3f)))
    .Start(this);
```

---

### AudioKit（音频管理）

```csharp
AudioKit.PlayMusic("bgm_name");
AudioKit.PlaySound("sfx_name");
AudioKit.PlayVoice("voice_name");

AudioKit.Settings.IsMusicOn = true;
AudioKit.Settings.MusicVolume = 0.8f;
AudioKit.Settings.IsSoundOn = true;
AudioKit.Settings.SoundVolume = 1.0f;
AudioKit.Settings.IsVoiceOn = true;
AudioKit.Settings.VoiceVolume = 1.0f;
```

默认资源名以 `resources://` 开头，默认使用 ResKit 管理音频资源。

---

### SingletonKit（单例套件）

| 类型 | 说明 | 用法 |
|---|---|---|
| `Singleton<T>` | C# 单例 | 继承 + 私有构造 + `Instance` |
| `MonoSingleton<T>` | Mono 单例，自动创建 GameObject | 继承 |
| `PersistentMonoSingleton<T>` | 跨场景不销毁，先创建者保留 | 继承 |
| `ReplaceableMonoSingleton<T>` | 跨场景不销毁，后创建者替换 | 继承 |
| `MonoSingletonProperty<T>` | 属性形式 Mono 单例 | `get => MonoSingletonProperty<T>.Instance` |
| `SingletonProperty<T>` | 属性形式 C# 单例 | `get => SingletonProperty<T>.Instance` |
| `[MonoSingletonPath]` | 自定义 Mono 单例层级路径 | `[MonoSingletonPath("[Audio]/AudioManager")]` |

```csharp
// 属性式单例
public class GameManager : MonoBehaviour, ISingleton
{
    public static GameManager Instance => MonoSingletonProperty<GameManager>.Instance;
    public void Dispose() => MonoSingletonProperty<GameManager>.Dispose();
    public void OnSingletonInit() { }
}

// MonoSingletonPath
[MonoSingletonPath("[Example]/MyManager")]
class MyManager : MonoSingleton<MyManager> { }
```

---

### FSMKit（状态机）

**链式**（快速开发，状态少）：

```csharp
public FSM<States> FSM = new FSM<States>();

void Start()
{
    FSM.State(States.A)
        .OnCondition(() => FSM.CurrentStateId == States.B)
        .OnEnter(() => { })
        .OnUpdate(() => { })
        .OnGUI(() => { if (GUILayout.Button("To B")) FSM.ChangeState(States.B); })
        .OnExit(() => { });

    FSM.StartState(States.A);
}

void Update() => FSM.Update();
void FixedUpdate() => FSM.FixedUpdate();
void OnGUI() => FSM.OnGUI();
void OnDestroy() => FSM.Clear();
```

**类模式**（状态多、逻辑重）：

```csharp
public class StateA : AbstractState<States, MyClass>
{
    public StateA(FSM<States> fsm, MyClass target) : base(fsm, target) { }
    protected override bool OnCondition() => mFSM.CurrentStateId == States.B;
}

FSM.AddState(States.A, new StateA(FSM, this));
FSM.StartState(States.A);
```

链式和类模式可混用。

---

### PoolKit（对象池）

```csharp
// SimpleObjectPool
var pool = new SimpleObjectPool<Fish>(() => new Fish(), initCount: 50);
var fish = pool.Allocate();
pool.Recycle(fish);

// SafeObjectPool（要求实现 IPoolable + IPoolType）
class Bullet : IPoolable, IPoolType
{
    public bool IsRecycled { get; set; }
    public void OnRecycled() { }
    public static Bullet Allocate() => SafeObjectPool<Bullet>.Instance.Allocate();
    public void Recycle2Cache() => SafeObjectPool<Bullet>.Instance.Recycle(this);
}
SafeObjectPool<Bullet>.Instance.Init(50, 25);
SafeObjectPool<Bullet>.Instance.SetFactoryMethod(() => new Bullet());

// ListPool / DictionaryPool
var names = ListPool<string>.Get();
names.Add("Hello");
names.Release2Pool();
```

---

### FluentAPI（链式 API）

对 Unity/C# 常用 API 的链式封装：

```csharp
Resources.Load<GameObject>("prefab")
    .Instantiate()
    .transform
    .Parent(null)
    .LocalRotationIdentity()
    .LocalScaleIdentity();

// 与 ResKit 配合
mResLoader.LoadSync<GameObject>("obj")
    .InstantiateWithParent(parent)
    .transform
    .LocalIdentity()
    .Name("MyObj")
    .Show();
```

---

### TableKit（表数据结构）

为 `List<T>` 提供多索引支持，兼顾查询性能：

```csharp
public class Student { public string Name; public int Age; public int Level; }

public class School : Table<Student>
{
    public TableIndex<int, Student> AgeIndex = new TableIndex<int, Student>(s => s.Age);
    public TableIndex<int, Student> LevelIndex = new TableIndex<int, Student>(s => s.Level);

    protected override void OnAdd(Student item) { AgeIndex.Add(item); LevelIndex.Add(item); }
    protected override void OnRemove(Student item) { AgeIndex.Remove(item); LevelIndex.Remove(item); }
    protected override void OnClear() { AgeIndex.Clear(); LevelIndex.Clear(); }
}

// 联合查询
foreach (var s in school.LevelIndex.Get(2).Where(s => s.Age < 3))
    Debug.Log(s.Name);
```

---

### LiveCodingKit（热重载）

Play Mode 下修改代码，等待编译后自动重新加载场景。

**使用：** Ctrl+E 面板开启 LiveCodingKit，选择编译后操作（重新加载当前场景 / 重启游戏）。适合调整数值和写 OnGUI 调试代码。

---

### GridKit（二维格子数据结构）

```csharp
var grid = new EasyGrid<string>(4, 4);
grid.Fill("Empty");
grid[2, 3] = "Hello";
grid.ForEach((x, y, content) => Debug.Log($"({x},{y}):{content}"));
grid.Clear();
```

适用：消除类游戏、俄罗斯方块、棋类、Tilemap 地块数据。

---

### 其他事件工具

**EnumEventSystem：** 枚举作为事件 ID，适合网络 protobuf 消息 id 通信。

```csharp
public enum TestEvent { Start, TestOne, End }
EnumEventSystem.Global.Register(TestEvent.TestOne, (key, obj) => Debug.Log(obj[0]));
EnumEventSystem.Global.Send(TestEvent.TestOne, "Hello");
EnumEventSystem.Global.UnRegister(TestEvent.TestOne, handler);
```

**StringEventSystem：** 字符串作为事件 ID，适合跨脚本层通信（Lua、ILRuntime、PlayMaker）。

```csharp
StringEventSystem.Global.Register("TEST_ONE", () => Debug.Log("ok"))
    .UnRegisterWhenGameObjectDestroyed(gameObject);
StringEventSystem.Global.Register<int>("TEST_TWO", count => Debug.Log(count));
StringEventSystem.Global.Send("TEST_ONE");
StringEventSystem.Global.Send("TEST_TWO", 10);
```

---

### 事件系统选择指南

| 事件系统 | 性能 | 适用场景 |
|---|---|---|
| **TypeEventSystem**（推荐） | 反射，CPU 稍弱 | 跨层级/跨模块通信，框架架构内部 |
| **EasyEvent**（推荐） | 接近委托，性能好 | 局部系统内部，原型快速迭代 |
| EnumEventSystem | 性能好 | 网络通信（protobuf 消息 id） |
| StringEventSystem | 一般 | 跨脚本层（Lua、ILRuntime） |

**默认推荐 TypeEventSystem + EasyEvent 组合。**

---

## 错误处理

- `try-catch` 仅用于外部 I/O 操作（文件、网络、第三方 SDK）。
- 不捕获宽泛的 `Exception`，捕获具体异常类型。
- 使用 `Debug.LogError()` 或 QFramework `LogKit` 记录错误。
- 开发期不变量检查使用 `Debug.Assert()`（仅 Development Build 生效）。
- 对于可能失败的操作，返回 `bool` 结果或使用 `TryGet` 模式（如 `TryGetValue`）。

## 异步 / 协程

- 异步操作优先使用 **UniTask**，而非 Unity 协程。
- 支持取消操作时使用 `CancellationToken`。
- 仅 Unity 生命周期相关场景使用 `StartCoroutine`。
- 避免 `async void`，改用 `async UniTask` 或 `async UniTaskVoid`。

### 编辑器代码

- 编辑器脚本放在 `Assets/Editor/` 文件夹下。
- 使用 `MenuItem` 属性添加自定义菜单项（沿用现有中文菜单名惯例）。
- 使用 `[CustomEditor]` 或 `PropertyDrawer` 编写自定义 Inspector。
- 适当使用 Odin Inspector 特性（`[Button]`、`[FoldoutGroup]` 等）。

### 通用实践

- UTF-8 编码，不带 BOM（延续现有惯例）。
- 使用现代 C# 特性：模式匹配、元组解构、null 条件运算符（`?.`）、null 合并运算符（`??`）、`new()`、switch 表达式。
- 公共 API 添加 XML 文档注释（`/// <summary>`），私有方法可省略。
- 不写重复代码意图的注释——用自描述命名代替。
- 小型数据容器优先使用 `readonly struct`。
- 引用成员名称时使用 `nameof()` 而非字符串字面量。
- 循环中拼接字符串使用 `StringBuilder`。
- 方法保持简短（不超过 30 行），超过时提取辅助方法。
- 多值匹配时使用 switch 表达式代替 `if-else` 链。