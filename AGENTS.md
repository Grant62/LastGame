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

本项目使用 QFramework.cs 架构层（四层架构 + CQRS 模式）。**QFramework.Toolkits 工具集（UIKit/ResKit 等）已废弃不用**，资源加载统一使用 Unity Addressables，UI 面板自管理。所有 System、Model 统一注册到 `Architecture<T>.Init()` 中，Utility 按类型分别在 `Init()` 或场景 MonoBehaviour 的 `Awake()` 中注册。

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

#### 每个 Feature 模块的 QF 层级子目录约定

每个模块按 QF 四层架构组织子目录，以 `Card/` 为例：

```
Features/Card/
├── Command/      # AbstractCommand 实现
├── System/       # ISystem 接口 + AbstractSystem 实现
├── Model/        # IModel 接口 + AbstractModel 实现
├── Utility/      # IUtility 接口 + 实现（统一放在此目录，不再散落各处）
├── Event/        # 跨层级事件 struct
├── Interfaces/   # 非 QF 层级的接口（如 ICardHoverDisplay 等 IUtility 接口也可放在这里）
├── View/         # ViewController / MonoBehaviour + IController（运行时视图）
├── UI/           # 由 CodeGenKit 生成的 ViewController（UI 面板）
├── Effects/      # Effect 子类（纯数据+行为，无架构感知）
├── Data/         # 纯数据类（CardData 等）
└── Define/       # struct 定义（CardDefine 等）
```

> **关键规则：** IUtility 接口和实现统一放在 `Utility/` 目录下。System 接口和实现统一放在 `System/` 目录下。不要用业务领域名（如 `Targeting/`、`Interaction/`、`Pool/`）来替代 QF 层级目录。

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

- `using` 指令放在文件顶部，按需导入即可，不强求分组顺序。
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
| 获取 Utility | ✓ | ✓ | ✓ | |
| 发送 Command | ✓ | | | |
| 发送 Query | ✓ | ✓ | | |
| 监听 Event | ✓ | ✓ | | |
| 发送 Event | | ✓ | ✓ | |

**接口定义：**

```csharp
// IController
public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem,
    ICanGetModel, ICanRegisterEvent, ICanSendQuery, ICanGetUtility
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
| 持有场景引用的 Utility | Controller 的 `Awake()` | ArrowDisplay、CursorDisplay、CardViewPool、CardHoverDisplay |

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

> **注意：UIKit 和 ResKit 已废弃不用。** 本章节仅供了解框架原始能力，项目实际使用 Addressables + 自管理面板。以下工具仍在使用：ActionKit、PoolKit、FSMKit、SingletonKit、CodeGenKit。

### ResKit（资源管理）— 已废弃

> 本项目使用 Unity Addressables 替代 ResKit。本节仅供参考。

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
- **禁止防御式判空**：架构保证非空的引用（`GetComponent`/`GetSystem`/`GetModel`/`GetUtility` 在已初始化的上下文中）不写 `if (x == null) return;` 或 `if (x != null)` 包裹。让异常暴露问题，不隐藏 bug。

## UI 渲染架构

### 分层总览

所有 UI 层是 `GameRoot` 的 `UIRoot` Transform 下的兄弟 Canvas 节点，按创建顺序（即 sibling index）排列：

```
GameRoot (DontDestroyOnLoad)
  └── UIRoot (Transform, 纯容器)
       ├── BgLayer       (Canvas, sort=0)   — 背景
       ├── SceneContainer (Canvas, sort=5)  — 场景 Prefab 挂载点
       ├── CommonLayer   (Canvas, sort=10)  — 跨房间全局 UI
       ├── CombatOverlay (Canvas, sort=15)  — 战斗内悬浮物
       └── PopUILayer    (Canvas, sort=20)  — 弹窗/面板
```

> **关键规则：** 每个场景 Prefab（LogoSceneRoot、CombatRoomRoot 等）**可以包含 Canvas 组件**。Sort Order 由各预制体内部在 Editor 中预设，与上述 5 层 Canvas 的 sort order 配合管理整体渲染顺序。

### 各层职责

| 层 | 内容 | 说明 |
|---|---|---|
| BgLayer | 背景图、装饰 | 通过 SceneContainer 管理的场景自行渲染 |
| SceneContainer | LogoSceneRoot / RunSceneRoot / MainMenuSceneRoot | `SceneContainer` 组件挂在这，Addressables 加载的场景 Prefab 放这里 |
| CommonLayer | TopBarPanel | 跨房间不变的全局 UI，运行期间加载一次 |
| CombatOverlay | hoverCard、ArrowView、CursorView、DamageText | 战斗内悬浮层，CombatController.RegisterUtilities() 注册时 Instantiate 到本层 |
| PopUILayer | GmPanel、PileGridPanel、DiscardSelectPanel | 弹出面板层 |

**渲染顺序：** 依赖 Canvas.sortingOrder（0→5→10→15→20），不使用 sibling 顺序。

**Canvas 规范：** 每层 Canvas 统一 `ScaleWithScreenSize` + `1920x1080` + `MatchWidthOrHeight=1`，代码中 `SetupCanvas()` 方法统一创建。

### UI 面板管理

**已废弃 UIKit。** 所有面板均为 `MonoBehaviour` 子类（非 `UIPanel`），通过 `Addressables.InstantiateAsync("PanelName", GameRoot.XXXLayer)` 直接加载并挂载到对应 Canvas 层。

| 面板 | 加载位置 | 加载时机 |
|---|---|---|
| TopBarPanel | `RunScene.OnSceneEnter()` → `GameRoot.CommonLayer` | 进入 Run 时 |
| BattleBottomPanel | `CombatController.Start()` → `CombatController.transform`（CombatRoom 内） | 进入战斗时，随房间销毁 |
| GmPanel | `GmSystem.OnInit()` → `GameRoot.PopUILayer` | 首次按 backtick 时 |
| PileGridPanel | `PileGridPanel.ShowOrCreate(pile)` → `GameRoot.PopUILayer` | 点击牌堆按钮或 Tab 键（全局单例 + `ToggleDrawPile`） |
| DiscardSelectPanel | `CombatController.OnHandDiscardRequest()` → `GameRoot.PopUILayer` | 需要弃牌时 |

### 弹窗栈管理

弹出面板（PopUILayer）通过 `IPopupStackSystem`（ISystem）管理，支持 ESC 逆序关闭：

```csharp
// Features/Combat/System/IPopupStackSystem.cs
public interface IPopupStackSystem : ISystem
{
    void Push(GameObject panel);       // 面板打开 → 入栈
    void Remove(GameObject panel);     // 面板自己关闭 → 出栈
    bool HandleEsc();                  // ESC → 关栈顶，return true=已处理
}

// Features/Combat/System/PopupStackSystem.cs
public class PopupStackSystem : AbstractSystem, IPopupStackSystem
{
    private readonly Stack<GameObject> mStack = new();

    public void Push(GameObject panel) => mStack.Push(panel);

    public void Remove(GameObject panel)
    {
        if (mStack.Count > 0 && mStack.Peek() == panel)
            mStack.Pop();
    }

    public bool HandleEsc()
    {
        if (mStack.Count == 0) return false;
        mStack.Pop().SetActive(false);
        return true;
    }
}
```

**ESC 入口：** `CombatController.Update()` 最前面检查 ESC → `HandleEsc()`。

**各面板接入：** `GmPanel.Open()/Close()`、`PileGridPanel.Show()/OnClose()`、`DiscardSelectPanel.Open()/Close()` 分别调 `Push`/`Remove`。

## SPA 场景管理系统

### 设计目标

整个游戏 **只有 1 个 Unity Scene**，通过容器组件交换 Prefab 来实现"场景切换"。

### 根架构

`GameRoot`（`PersistentMonoSingleton`）在 `BeforeSceneLoad` 时自动创建，`BuildRootCanvas()` 代码动态构建上述 5 层 Canvas 结构。`GameRoot` 暴露静态属性供外部访问：

```csharp
GameRoot.CommonLayer   // Transform
GameRoot.CombatOverlay // Transform
GameRoot.PopUILayer    // Transform
```

### 核心类

#### SceneBase — 场景基类

```csharp
public abstract class SceneBase : MonoBehaviour
{
    public abstract string SceneId { get; }
    public abstract SceneContainerType ContainerType { get; }
    public virtual UniTask OnSceneEnter(SceneLoadContext ctx) => UniTask.CompletedTask;
    public virtual UniTask OnSceneExit() => UniTask.CompletedTask;
    public virtual void OnScenePause() { }
    public virtual void OnSceneResume() { }
}

public enum SceneContainerType { Main, Room, Overlay }
```

#### SceneContainer — 容器组件（纯 MonoBehaviour，非 IController）

```csharp
public class SceneContainer : MonoBehaviour
{
    public SceneBase CurrentScene { get; private set; }
    public async UniTask SetCurrentScene(SceneBase newScene, SceneLoadContext ctx = null) { ... }
    public void Clear() { ... }
}
```

#### ISceneManager — 场景调度 System

```csharp
public interface ISceneManager : ISystem
{
    SceneBase CurrentMainScene { get; }
    SceneBase CurrentRoomScene { get; }
    void SetRoomContainer(SceneContainer roomContainer);
    UniTask LoadMainScene(string sceneId, SceneLoadContext ctx = null);
    UniTask LoadRoomScene(string sceneId, SceneLoadContext ctx = null);
    UniTask PreloadScene(string sceneId);
}
```

**已删除：** `ShowOverlay`/`HideOverlay`（未使用）、`ISceneTransition`/`SceneTransition`（废弃）、OverlayContainer。

### CombatRoom 生命周期

每场战斗结束，`CombatRoom.OnSceneExit()` 执行统一清理：

```csharp
public override UniTask OnSceneExit()
{
    this.SendCommand(new ShuffleAllToDrawPileCommand());  // 手牌/弃牌堆/消耗堆洗回抽牌堆
    foreach (Transform child in GameRoot.CombatOverlay)    // 清 hoverCard/arrow/cursor
        Object.Destroy(child.gameObject);
    GameMain.Interface.GetUtility<IEnemyViewPool>().Dispose();       // 清敌人池
    GameMain.Interface.GetUtility<IDamageTextSpawner>().Dispose();   // 清伤害文字池
    return UniTask.CompletedTask;
}
```

### 交互输入架构

```
UGUI EventSystem（全 UGUI，无 Physics）
─────────────────────────────────────────
卡牌拖拽：IBeginDragHandler / IDragHandler / IEndDragHandler
目标检测：EventSystem.RaycastAll → IEnemyTarget / ISlotTarget
卡牌释放/点击：IPointerDownHandler / IPointerUpHandler
```

不再使用任何 Physics.Raycast / Physics2D.OverlapPoint。所有交互通过 UGUI EventSystem 的 `RaycastAll` 接口实现。

### 初始化时序与 GameReadyEvent

System 的 `OnInit()` 在架构注册期间（`GameMain.Init()`）执行，此时场景未加载、持有场景引用的 Utility 尚未注册。需依赖 Scene Utility 的 System 不应在 `OnInit()` 中缓存，应订阅 `GameReadyEvent`：

```csharp
// 1. 定义事件（Features/Combat/Event/GameReadyEvent.cs）
public struct GameReadyEvent { }

// 2. 在 System.OnInit() 中注册监听，延迟缓存
protected override void OnInit()
{
    this.RegisterEvent<GameReadyEvent>(_ =>
    {
        mArrow = this.GetUtility<IArrowDisplay>();
        mCursor = this.GetUtility<ICursorDisplay>();
    });
}

// 3. 所有 Scene Utility 注册完毕后发送
// 在 CombatController.RegisterUtilities() 末尾：
GameMain.Interface.SendEvent<GameReadyEvent>();
```

### 卡牌系统三堆架构

```
牌库（Library）— 战斗开始时的初始牌组，不可变
    ↓ StartBattleDraw()
抽牌堆（DrawPile）— 待抽的牌，回合开始从里抽
    ↓ DrawCards()
手牌（HandPile）— 玩家手中的牌，可打出
    ↓ PlayCardCommand（消耗能量）→ AddToDiscard()
弃牌堆（DiscardPile）— 已使用或丢弃的牌
    ↓ 抽牌堆耗尽时洗回抽牌堆
```

三堆均为 `List<CardData>`，变更时通过 `EasyEvent` 通知：
- `OnLibraryChanged`
- `OnDrawPileChanged`
- `OnHandPileChanged`
- `OnDiscardPileChanged`

### UGUI 卡牌组件

`CardView`（`Features/Card/View/CardView.cs`）是 UGUI 版的卡牌显示组件，挂载在 `Image` + `TMP_Text` 子控件构成的预制体上。通过 `ICardViewPool` 对象池管理生命周期。

- 手牌：`HandPanel`（`ViewController`）监听 `ICardModel.OnHandPileChanged`，通过 `ICardViewPool` 同步视图
- 拖拽：`HandDragHandler`（`IBeginDragHandler`/`IDragHandler`/`IEndDragHandler`）处理拖拽手势，创建拖拽幽灵卡牌，释放时走 `PlayCardCommand`
- 网格查看器：`PileGridPanel`（`ViewController` + `ScrollRect` + `GridLayoutGroup`）展示抽牌堆/弃牌堆/牌库

### 卡牌定义（CardDefine → CardData）

`CardDefine`（Excel 驱动）→ `CreateCardData()` → `CardData`（运行时实例）。`CardData` 的 `ManualTargetEffect` 决定卡牌是否需要瞄准敌人。

---

## 效果系统（Effect System）

### 架构

```
CardEffectSystem (ISystem)
  ├── 创建 EffectContext（聚合 IHeroModel/ISwordModel/ICardSystem 等依赖）
  ├── effect.Ctx = context（依赖注入）
  └── effect.Execute(targets, caster)

Effect 子类（纯数据+行为，无架构感知）
  ├── 通过 Ctx.Xxx 访问 Model/System/Utility
  ├── 由 CardEffectFactory.PopulateEffects() 组装
  └── 不持有状态，不注册事件
```

### 核心约定

| 规则 | 说明 |
|---|---|
| Effect 不访问 `GameMain.Interface` | 依赖通过 `EffectContext` 注入 |
| Effect 不注册事件 | 改为设 Model 标记，由 System 监听处理 |
| 场景对象通过 `IBoardAccess` Utility 获取 | 不使用 `FindObjectOfType` |
| 效果组合优于继承 | 每张卡 = `ManualTargetEffect[]` + `OtherEffects[]` |


## 战斗钩子系统（Combat Hook System）

### 设计目标

借鉴 STS2 的 Hook 系统，为战斗中的属性修改、伤害计算、状态触发提供**可插拔的钩子点**。遗物、能力（Power）、状态效果（Status）通过实现钩子接口来影响战斗逻辑，替代硬编码的 if-else 判断链。

### 架构层级

钩子系统以 **ISystem（CombatHookSystem）** 形式存在，位于 QFramework 系统层：

```
IController → SendCommand (PlayCard / Attack / Damage)
    ↓
Command.OnExecute() 中调用 CombatHookSystem 的分发方法
    ↓
CombatHookSystem 遍历已注册的 ICombatHook 列表
    ↓
每个 ICombatHook 根据自己的条件决定是否介入
    ↓
返回修改后的值 / 触发副作用 → Event → View 更新
```

### 钩子接口定义

所有钩子统一通过 `ICombatHook` 接口注册，放在 `Features/Combat/Hook/` 目录下：

```csharp
// Features/Combat/Hook/ICombatHook.cs
namespace Features.Combat.Hook
{
    /// <summary>战斗钩子优先级：数值越小越先执行</summary>
    public enum CombatHookPriority
    {
        StatusEffect = 0,   // 状态效果（虚弱、易伤）
        Power = 100,        // 能力（力量、敏捷）
        Relic = 200,        // 遗物
        CardEffect = 300,   // 卡牌自身效果
    }

    /// <summary>所有战斗钩子的基接口</summary>
    public interface ICombatHook
    {
        /// <summary>是否激活（死亡/被移除后返回 false）</summary>
        bool IsActive { get; }

        /// <summary>执行优先级</summary>
        CombatHookPriority Priority { get; }
    }

    // ----- 三类钩子 -----

    /// <summary>Before/After 型：观察副作用，无返回值</summary>
    public interface ICombatHook_OnEvent : ICombatHook
    {
    }

    /// <summary>Modify 型：链式修改值，返回修改后的值</summary>
    public interface ICombatHook_Modify : ICombatHook
    {
    }

    /// <summary>Should 型：布尔断言，返回 false 阻止操作</summary>
    public interface ICombatHook_Should : ICombatHook
    {
    }
}
```

### 具体钩子接口示例

每种战斗事件定义一个接口，继承上述三类之一。放在 `Features/Combat/Hook/` 下：

```csharp
// 示例：伤害相关钩子
public interface IBeforeAttackHook : ICombatHook_OnEvent
{
    void BeforeAttack(ITargetable target, ref int damage, DamageType type);
}

public interface IAfterAttackHook : ICombatHook_OnEvent
{
    void AfterAttack(ITargetable target, int damage, DamageType type);
}

public interface IModifyDamageDealtHook : ICombatHook_Modify
{
    /// <summary>链式修改造成的伤害（如力量+3，虚弱×0.75）</summary>
    int ModifyDamageDealt(ITargetable target, int baseDamage, DamageType type);
}

public interface IModifyDamageReceivedHook : ICombatHook_Modify
{
    int ModifyDamageReceived(ITargetable self, int incomingDamage, DamageType type);
}

public interface IShouldDieHook : ICombatHook_Should
{
    bool ShouldDie(ITargetable creature);
}

// 示例：格挡相关钩子
public interface IBeforeBlockGainedHook : ICombatHook_OnEvent
{
    void BeforeBlockGained(ITargetable target, ref int amount);
}

public interface IModifyBlockGainedHook : ICombatHook_Modify
{
    int ModifyBlockGained(ITargetable target, int baseAmount);
}

// 示例：抽牌相关钩子
public interface IModifyDrawCountHook : ICombatHook_Modify
{
    int ModifyDrawCount(int baseCount);
}
```

### CombatHookSystem 实现

```csharp
// Features/Combat/Hook/CombatHookSystem.cs
namespace Features.Combat.Hook
{
    public interface ICombatHookSystem : ISystem
    {
        void RegisterHook(ICombatHook hook);
        void UnregisterHook(ICombatHook hook);

        // 分发方法
        int ModifyDamageDealt(ITargetable target, int baseDamage, DamageType type);
        int ModifyDamageReceived(ITargetable self, int incomingDamage, DamageType type);
        int ModifyBlockGained(ITargetable target, int baseAmount);
        int ModifyDrawCount(int baseCount);
    }

    public class CombatHookSystem : AbstractSystem, ICombatHookSystem
    {
        private List<ICombatHook> mHooks = new List<ICombatHook>();

        protected override void OnInit()
        {
            // 战斗重置时清空钩子
            this.RegisterEvent<BattleStartEvent>(_ => mHooks.Clear());
        }

        public void RegisterHook(ICombatHook hook)
        {
            mHooks.Add(hook);
            mHooks.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void UnregisterHook(ICombatHook hook) => mHooks.Remove(hook);

        public int ModifyDamageDealt(ITargetable target, int baseDamage, DamageType type)
        {
            int result = baseDamage;
            foreach (var hook in mHooks)
            {
                if (hook is IModifyDamageDealtHook h && h.IsActive)
                    result = h.ModifyDamageDealt(target, result, type);
            }
            return result;
        }

        // ... 其他分发方法同理
    }
}
```

### 与现有 Effect 系统的关系

| 系统 | 职责 | 层级 |
|---|---|---|
| **Effect**（现有） | 卡牌打出时执行的具体行为（扣血、加盾、抽牌） | Command 层 |
| **CombatHook**（新增） | Effect 执行过程中，外部因素对数值的修正 | System 层 |

**调用顺序：**
```
PlayCardCommand.OnExecute()
  → CardEffectSystem.OnCardPlayed()
    → Effect.Execute() 内部：
      1. 计算基础伤害值 baseDamage
      2. CombatHookSystem.ModifyDamageDealt(target, baseDamage)  // Hook 链修正
      3. CombatHookSystem.ModifyDamageReceived(target, modifiedDamage)  // 目标端修正
      4. 应用最终伤害 → IDamageable.TakeDamage()
```

### 注册钩子的时机

| 钩子来源 | 注册方式 | 注册时机 |
|---|---|---|
| 遗物 | `CombatHookSystem.RegisterHook(relic)` | 进入战斗时（`AfterRoomEntered`） |
| 能力（Power） | `CombatHookSystem.RegisterHook(power)` | Power 被施加时（`AfterPowerReceived`） |
| 状态效果 | `CombatHookSystem.RegisterHook(status)` | 状态被施加时 |
| 卡牌自身 | 不需要注册 | Effect 内部直接计算 |

### 设计原则

- **链式不可变**：Modify 型钩子接收上一步的结果，返回新的值，不修改传入参数
- **优先级排序**：StatusEffect(0) → Power(100) → Relic(200)，确保虚弱/易伤最先计算、力量最后
- **IsActive 门控**：死亡的生物、被移除的遗物自动跳过
- **无架构感知**：ICombatHook 实现类放在 `Features/Combat/Hook/` 下，通过接口访问 Model/System，不直接依赖 `GameMain.Interface`
- **先有后优**：初期可只实现 Modify 型钩子（覆盖 80% 场景），Before/After 和 Should 型按需追加

### 演化时机（何时引入）

| 阶段 | 当前状态 | 触发条件 | 引入动作 |
|---|---|---|---|
| **不要现在做** | 只有硬编码 Effect，无遗物/能力/状态系统 | — | 把这页当参考文档 |
| **P0 阶段引入** | 开始写状态效果（虚弱/易伤），`DealDamageEffect` 里 if-else 超过 3 种 | 发现自己在 Effect 里写了 `if (hasWeak) damage *= 0.75f; if (hasVulnerable) damage *= 1.5f;` | 创建 `CombatHookSystem`，只实现 Modify 型钩子 |
| **P1 阶段扩展** | 已添加遗物或能力的原型 | 某个遗物需要"出牌时触发额外效果"或"格挡获得量翻倍" | 追加 Before/After 型钩子 |
| **P2 阶段完整** | 遗物/能力系统成型，有反制/阻止类效果 | 某个效果需要"阻止死亡一次"或"阻止格挡被清除" | 追加 Should 型钩子 |

> **关键信号：** 当你在 `DealDamageEffect` 或 `GainBlockEffect` 里写第 4 个 `if` 判断状态修正时，立即停止，引入 Hook 系统。不要等到 if-else 嵌套到不可维护才重构。

---

## Canonical/Mutable 数据模式

### 设计目标

借鉴 STS2 的 Canonical/Mutable 模式，将游戏实体的**模板数据**与**运行时可变数据**分离。解决卡牌升级后无法回滚、存档序列化臃肿、多实例共享引用被意外修改等问题。

### 核心概念

| 概念 | 说明 | 示例 |
|---|---|---|
| **Canonical（模板）** | Excel 定义的原始数据，只读，全局唯一 | 卡牌"打击"的模板：费用1、伤害6、无升级 |
| **Mutable（运行时）** | Canonical 的浅克隆 + 可变字段，每个实例独立 | 战斗中的"打击+"：费用1、伤害9、已升级 |

```
CardDefine (Excel)
    ↓ CreateCardData()
CardData (Canonical, IsMutable=false)
    ├── 进入卡组时 → Clone() → Mutable 副本加入牌库
    ├── 升级时 → MutableClone() → 修改 → 存回牌库
    └── 存档时 → 只序列化 Mutable 副本 + CanonicalId
```

### 实现要点

**1. 所有数据类继承 `ICloneable` 或提供 `MutableClone()` 方法：**

```csharp
// Features/Card/Data/CardData.cs
public class CardData
{
    /// <summary>是否为可变副本（false = 模板，true = 运行时实例）</summary>
    public bool IsMutable { get; private set; }

    /// <summary>模板 ID，可变副本通过此 ID 找回原始定义</summary>
    public string CanonicalId { get; private set; }

    public string Name { get; set; }
    public int Cost { get; set; }
    public int BaseDamage { get; set; }
    public bool IsUpgraded { get; set; }
    public List<Effect> Effects { get; set; }

    /// <summary>创建此卡的可变副本（浅克隆 + 深克隆引用类型字段）</summary>
    public CardData MutableClone()
    {
        var clone = (CardData)MemberwiseClone();
        clone.IsMutable = true;
        // 深克隆引用类型字段，避免共享 List/Array
        clone.Effects = new List<Effect>(Effects);
        return clone;
    }
}
```

**2. 工厂方法统一入口：**

```csharp
// CardData 由 CardFactory 统一创建
public static class CardFactory
{
    /// <summary>从 Excel 数据创建模板卡牌（Canonical）</summary>
    public static CardData CreateCanonical(CardDefine define) { ... }

    /// <summary>创建入牌库用的可变副本</summary>
    public static CardData CreateForDeck(string canonicalId)
    {
        CardData canonical = CardDefineModel.GetCanonical(canonicalId);
        return canonical.MutableClone();
    }
}
```

**3. 存档/读档只处理 Mutable 数据：**

```
存档结构：
{
    "cards": [
        { "canonicalId": "strike_ironclad", "isUpgraded": true, "cost": 1 },
        { "canonicalId": "defend_ironclad", "isUpgraded": false, "cost": 1 }
    ]
}

读档流程：
    canonicalId → CardDefineModel.GetCanonical(id) → MutableClone() → 覆盖可变字段 → 完成
```

### 适用实体

| 实体 | Canonical 来源 | Mutable 场景 |
|---|---|---|
| **CardData** | Excel `CardDefine` | 牌库中的每张卡实例 |
| **EnemyData** | Excel `EnemyDefine` | 战斗中的敌方实例（血量、状态） |
| **RelicData** | Excel `RelicDefine` | 玩家拥有的遗物实例（计数器） |
| **PotionData** | Excel `PotionDefine` | 玩家持有的药水实例 |

### 约束

- **Canonical 禁止修改**：`IsMutable == false` 时，任何 setter 应该抛异常或 assert
- **Mutable 禁止当 Key 用**：字典/ HashSet 的 Key 用 `CanonicalId`（string），不用 CardData 实例
- **比较用 CanonicalId**：两张卡是否"同一种"看 `CanonicalId`，而非引用相等

### 演化时机（何时引入）

| 阶段 | 当前状态 | 触发条件 | 引入动作 |
|---|---|---|---|
| **不要现在做** | 所有 CardData 直接从 Excel 构造，战斗内直接修改原对象 | — | 把这页当参考文档 |
| **P0 阶段引入** | 开始做"升级卡牌"功能，或者第一次需要序列化牌库到存档文件 | 发现升级后的卡牌无法回滚，或者存档里写了 200 个 CardData 的完整字段 | 给 `CardData` 加 `IsMutable` + `MutableClone()`，改工厂入口 |
| **P1 阶段扩展** | 存档系统上线，或出现多实例共享引用被意外修改的 bug | 读档后修改一张卡牌导致同名卡牌全部变化 | 扩展到 `EnemyData`、`RelicData` 等实体 |

> **关键信号：** 当你第一次写 `cardData.Cost -= 1;` 来升级卡牌，然后发现找不到原始费用时，立即引入 Canonical/Mutable。不要等数据被覆盖了才补救。

---

## Command 异步化规范

### 背景

当前 `AbstractCommand.OnExecute()` 为同步 `void`，适用于即时数据修改。当战斗逻辑需要时序控制（播攻击动画 → 等待 → 扣血 → 等待 → 检查死亡）时，Command 自身不适合承载异步逻辑。

### 推荐方案：System 异步方法 + Command 触发

**Command 保持同步、轻量**，只做参数校验和调用 System 方法。异步时序逻辑放到 **ISystem 的 UniTask 方法** 中：

```csharp
// ❌ 不推荐：Command 里写 async
public class PlayCardCommand : AbstractCommand
{
    protected override async void OnExecute() { } // AbstractCommand 不支持
}

// ✅ 推荐：Command 同步触发，System 异步执行
public class PlayCardCommand : AbstractCommand
{
    private readonly CardData mCardData;
    private readonly ITargetable mManualTarget;

    public PlayCardCommand(CardData cardData, ITargetable manualTarget = null)
    {
        mCardData = cardData;
        mManualTarget = manualTarget;
    }

    protected override void OnExecute()
    {
        IResourceSystem resource = this.GetSystem<IResourceSystem>();
        if (!resource.CanSpend(mCardData.Cost))
            return;

        resource.Spend(mCardData.Cost);
        this.GetSystem<ICardSystem>().RemoveFromHand(mCardData);

        // 异步执行交给 CombatFlowSystem
        this.GetSystem<ICombatFlowSystem>().ExecutePlayCardAsync(
            mCardData, mManualTarget, mCardData.Cost
        ).Forget(); // UniTask.Forget() 启动火后即忘
    }
}
```

```csharp
// Features/Combat/System/CombatFlowSystem.cs
public interface ICombatFlowSystem : ISystem
{
    UniTask ExecutePlayCardAsync(CardData card, ITargetable target, int energySpent);
    UniTask ExecuteEnemyTurnAsync();
}

public class CombatFlowSystem : AbstractSystem, ICombatFlowSystem
{
    public async UniTask ExecutePlayCardAsync(CardData card, ITargetable target, int cost)
    {
        var effectSystem = this.GetSystem<CardEffectSystem>();

        // 1. 播卡牌动画
        this.SendEvent(new CardPlayAnimationEvent(card));
        await UniTask.Delay(300); // 等待动画

        // 2. 执行效果
        effectSystem.ExecuteCardEffects(card, target, cost);
        this.SendEvent(new CardPlayedEvent(card, target, cost, -1));

        // 3. 检查死亡
        await UniTask.Delay(200);
        CheckDeaths();

        // 4. 进入弃牌堆
        this.GetSystem<ICardSystem>().AddToDiscard(card);
    }

    private void CheckDeaths()
    {
        // 遍历敌方，检查 hp <= 0，发送 EnemyDiedEvent
    }
}
```

### 与现有回合系统的关系

当前 `TurnSystem` 已使用 UniTask 做敌方回合延迟。`CombatFlowSystem` 取代 `CardEffectSystem` 的时序编排职责，`CardEffectSystem` 退化为**纯效果执行器**（不含时序）：

| 系统 | 职责 |
|---|---|
| `CardEffectSystem`（精简） | 接收 CardPlayedEvent → 执行 Effect.Execute()（同步） |
| `CombatFlowSystem`（新增） | 编排出牌时序：动画→等待→效果→等待→检查死亡→弃牌 |
| `TurnSystem`（现有） | 回合切换：PlayerTurn↔EnemyTurn，抽牌时机 |

### 取消支持

当战斗结束或场景卸载时，通过 `CancellationToken` 取消进行中的异步操作：

```csharp
public class CombatFlowSystem : AbstractSystem, ICombatFlowSystem
{
    private CancellationTokenSource mCts;

    protected override void OnInit()
    {
        this.RegisterEvent<BattleStartEvent>(_ => mCts = new CancellationTokenSource());
        this.RegisterEvent<BattleEndEvent>(_ => mCts?.Cancel());
    }

    public async UniTask ExecutePlayCardAsync(CardData card, ITargetable target, int cost)
    {
        if (mCts.Token.IsCancellationRequested) return;

        await UniTask.Delay(300, cancellationToken: mCts.Token);
        // ... 后续步骤检查 Token
    }
}
```

### 转换指南（渐进式）

1. **现有同步 Command 不动**：`PlayCardCommand`、`StartBattleCommand` 等保持同步
2. **新增 CombatFlowSystem**：将 `CardEffectSystem` 中的时序逻辑抽到 `CombatFlowSystem`
3. **Command 改为委托异步**：`this.GetSystem<ICombatFlowSystem>().ExecuteXxxAsync(...).Forget()`
4. **动画/等待统一走 UniTask**：不用协程 `StartCoroutine`，不用 `Invoke`

### 原则

- **Command 是入口**：只做校验 + 调用 System，永远同步
- **System 是执行者**：持有异步方法，负责时序编排
- **UniTask 而非 Coroutine**：可等待、可取消、有返回值
- **`.Forget()` 处理火后即忘**：不需要等待结果的调用用 `.Forget()` 启动

### 演化时机（何时引入）

| 阶段 | 当前状态 | 触发条件 | 引入动作 |
|---|---|---|---|
| **不要现在做** | 所有效果即时结算，无动画等待 | — | 把这页当参考文档 |
| **P0 阶段引入** | 需要播攻击动画后再扣血，或卡牌效果需要分步执行 | 发现自己在 `Effect.Execute()` 里写 `StartCoroutine` 来等动画 | 创建 `CombatFlowSystem`，将 `CardEffectSystem` 时序逻辑迁入 |
| **P1 阶段扩展** | 多个异步操作需要取消支持 | 战斗结束/场景卸载时残留的协程还在跑，导致 NullReference | 引入 `CancellationTokenSource`，所有 async flow 接入取消支持 |

> **关键信号：** 当你第一次在 Command 或 Effect 里调 `StartCoroutine()` 来等动画完成时，立即引入 `CombatFlowSystem`。Command 不应该知道有"等待"这回事。

