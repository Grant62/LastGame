# AGENTS.md

## 项目概览

Unity 6 (6000.0.x) 卡牌/肉鸽游戏，使用 QFramework 架构。
语言：C# (Unity)、ShaderLab。
脚本后端：Mono / IL2CPP。

### QFramework 版本说明

本项目使用完整的 QFramework.Toolkits（包含 QFramework.cs 架构 + 全部工具集）。四层架构与 CQRS 模式是项目核心设计范式。所有 System、Model、Utility 统一注册到 `Architecture<T>.Init()` 中。

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
| 私有字段 | `m` 前缀（QFramework 惯例） | `private int mCount;` |
| 公有字段 | PascalCase，优先使用属性 | `public int Health { get; set; }` |
| 局部变量 | camelCase | `playerCount` |
| 方法参数 | camelCase | `playerData` |
| 常量 | PascalCase | `public const int MaxCount = 10;` |
| 静态只读 | PascalCase | `public static readonly string Path;` |
| `var` | 右侧类型明显时可用，原始类型不用 | `var list = new List<int>();`（不用 `var i = 0;`） |

### 命名规范

| 元素 | 规范 | 示例 |
|---|---|---|
| 类 / 结构体 | PascalCase | `GameManager`、`PlayerData` |
| 接口 | PascalCase，`I` 前缀 | `ISaveable`、`IController` |
| 方法 | PascalCase | `OnInit()`、`Execute()` |
| 属性 | PascalCase | `Instance`、`Health` |
| 私有字段 | `m` + PascalCase | `mInstance`、`mDataList` |
| 局部变量 | camelCase | `playerCount` |
| 方法参数 | camelCase | `playerData` |
| 常量 | PascalCase | `MaxHealth` |
| 枚举 | PascalCase，单数 | `PlayerState`、`DamageType` |
| 枚举值 | PascalCase | `Idle`、`Walking` |
| 事件 / 委托 | PascalCase | `OnHealthChanged` |
| 序列化字段 | `[SerializeField] private` + camelCase | `[SerializeField] private float moveSpeed` |

### QFramework 架构规则

**四层架构与能力矩阵（自上而下）：**

- **IController**（表现层，MonoBehaviour/ViewController）
  - 可以获取 System、Model
  - 可以发送 Command、Query
  - 可以监听 Event

- **ISystem**（系统层，共享逻辑）
  - 可以获取 System、Model
  - 可以监听 Event、发送 Event
  - 适合放成就、计时、商城等规则类逻辑

- **IModel**（数据层，数据定义与增删改查）
  - 可以获取 Utility
  - 可以发送 Event
  - 只放需要共享的数据，不需要共享的不要放

- **IUtility**（工具层，基础设施）
  - 不向上依赖任何层
  - 适合放存储/SDK/网络等封装

**Command 与 Query（CQRS 读写分离）：**

- **ICommand**：负责数据的 **增/删/改**。不能持有状态。可获取 System/Model、发送 Event/Command。
- **IQuery\<T\>**：负责数据的 **查**。可获取 System/Model、发送 Query。
- Command 修改数据后必须发送数据变更 Event，不要手动调用 UpdateView。

**层级访问规则：**

- 上层可以直接获取下层，下层不能获取上层对象。
- 上层向下层通信用方法调用（查询）或 Command（状态变更）。
- 下层向上层通信用 Event 或 BindableProperty。
- IController 更改 ISystem/IModel 的状态必须用 Command。
- ISystem/IModel 状态变更后通知 IController 必须用 Event 或 BindableProperty。
- ICommand、IQuery 不能持有状态。

**Architecture 注册规范：**

- **System / Model**：统一在 `Architecture<T>.Init()` 中注册（生命周期固定）。
- **Utility**：分两类注册：
  - 无状态/全局的 Utility（如 Logger、Random）在 `Init()` 中注册。
  - 持有场景引用的 Utility（如 TargetSelector、CursorDisplay）在场景 MonoBehaviour（Controller/Installer）的 Awake 中通过 `GameMain.Interface.RegisterUtility<T>()` 注册。
- 使用接口注册以支持依赖倒置：`this.RegisterSystem<ISystem>(new SystemImpl())`。
- Architecture 集中管理模块，可作为项目架构图使用。

**Command 拦截：**

- 覆写 `ExecuteCommand(ICommand)` 可以实现日志、权限、撤销等中间件功能。

**推荐项目结构：**

```
Architecture<T>        → 顶层，注册所有模块
IController (View)    → 监听输入，发 Command/Query
ISystem               → 监听 Event，处理规则逻辑
IModel                → 定义数据，暴露 BindableProperty
IUtility              → 封装基础设施（存储、网络等）
Command / Query       → 数据的增删改查操作
Event / BindableProperty → 层级间通知
```

**纸上设计：**

- 开发前先画功能图（Command → Model → Event → View 的流向）和架构图（模块所属层级）。
- 可用于团队沟通和任务分配。

## QFramework 核心工具规范

### TypeEventSystem（类型事件系统）

- 事件体定义为 `struct`（减少 GC）。
- 使用 `TypeEventSystem.Global.Register<T>(Action<T>)` 注册，返回 `IUnRegister` 可自动注销。
- 自动注销：`.UnRegisterWhenGameObjectDestroyed(gameObject)`。
- 支持接口事件：实现 `IOnEvent<T>` 接口，调用 `this.RegisterEvent<T>()`。
- 适合跨层级/跨模块通信，架构内部默认使用此机制。

### EasyEvent（轻量级事件）

- 适合不需要声明事件类的场景，性能接近 C# 委托。
- 支持 `EasyEvent`（无参）、`EasyEvent<T>`（1参）、`EasyEvent<T1,T2>`（2参）。
- 作为局部系统内部通信的首选，如背包系统内部、对话系统内部。

### BindableProperty（可绑定属性）

- 提供 `数据 + 数据变更事件` 的封装。
- `Register(Action<T>)` 注册值变更回调，`RegisterWithInitValue` 注册时立即回调一次。
- Model 层推荐用 BindableProperty 暴露数据，Controller 通过监听实现表现更新。

### IOCContainer（控制反转容器）

- 本质是 `Dictionary<Type, object>`，支持接口到实现的映射。
- 在 Architecture 的 Init() 中统一注册，通过接口获取：`this.GetModel<IModel>()`。

## QFramework.Toolkits 工具集规范

### CodeGenKit（代码生成）

- **ViewController**：挂到根节点（快捷键 Alt+V），设置命名空间和生成目录。
- **Bind**：挂到子节点（快捷键 Alt+B），标记要绑定的组件和类型。
- 生成的代码分两个文件：`XXX.cs`（手动逻辑，只生成一次）和 `XXX.Designer.cs`（自动生成，每次覆盖）。
- 支持嵌套 ViewController（子节点再挂 ViewController），支持生成 Prefab。
- 默认脚本生成路径和命名空间在 QFramework 编辑器面板设置（Ctrl+E）。

### ResKit（资源管理）

- **模拟模式**：开发阶段勾选模拟模式，无需打 AssetBundle 即可加载资源。
- **真机模式**：发布前取消模拟模式，点击"打 AB 包"生成 AssetBundle。
- **资源标记**：在 Assets 中对文件/文件夹右键 → `@ResKit-AssetBundle Mark`。
- 每个需要加载资源的脚本申请一个 `ResLoader`（使用 `ResLoader.Allocate()`）。
- 通过 `mResLoader.LoadSync<T>(assetName)` 同步加载，或 `Add2Load` + `LoadAsync` 异步加载。
- **释放**：脚本销毁时调用 `mResLoader.Recycle2Cache()`。
- 资源名建议使用代码生成（QAssets.cs），避免字符串拼写错误。

### UIKit（界面管理）

- **UIPanel 生命周期**（按顺序）：`OnInit` → `OnOpen` → `OnShow` → `OnHide` → `OnClose`。
- **打开/关闭**：`UIKit.OpenPanel<T>()`、`UIKit.ClosePanel<T>()`、`this.CloseSelf()`。
- **层级**：`UILevel.Common`、`UILevel.Forward`、`UILevel.UITop`、`UILevel.Guide`。
- **UIElement**：子界面/子控件，通过 Bind 设置 `标记类型=Element` 自动生成。
- 推荐每个界面一个独立测试场景，用 UIPanelTester 运行。
- 异步加载：`UIKit.OpenPanelAsync<T>()`，WebGL 平台必须使用异步。

### ActionKit（时序动作系统）

- **Sequence**：顺序执行 `ActionKit.Sequence().Callback().Delay().Callback()...Start(this)`。
- **Parallel**：并行执行 `ActionKit.Parallel().Delay(1f, callback)...Start(this)`。
- **Delay**：延时 `ActionKit.Delay(seconds, callback).Start(this)`。
- **Repeat**：重复 `ActionKit.Repeat(count).Condition().Callback()...Start(this)`。
- **Condition**：条件等待 `ActionKit.Sequence().Condition(() => condition)`。
- **Coroutine**：协程支持 `ActionKit.Coroutine(IEnumerator).Start(this)`。
- **Custom**：自定义动作 `ActionKit.Custom(a => a.OnStart().OnExecute(dt => a.Finish()).OnFinish())`。

### AudioKit（音频管理）

- 三类音频 API：`AudioKit.PlayMusic()`（背景音乐）、`AudioKit.PlaySound()`（音效）、`AudioKit.PlayVoice()`（人声）。
- 设置开关/音量：`AudioKit.Settings.IsMusicOn`、`AudioKit.Settings.MusicVolume` 等。
- 默认音频资源名以 `resources://` 开头。

### SingletonKit（单例套件）

- **C# 单例**：继承 `Singleton<T>`，声明私有构造。
- **Mono 单例**：继承 `MonoSingleton<T>`，自动创建 GameObject。
- **PersistentMonoSingleton**：跨场景不销毁，先创建者保留。
- **ReplaceableMonoSingleton**：跨场景不销毁，后创建者替换。

### FSMKit（状态机）

- **链式**：`FSM.State(States.A).OnCondition().OnEnter().OnUpdate().OnExit()`。
- **类模式**：继承 `AbstractState<States, Target>` 实现 `OnCondition()`、`OnEnter()` 等。
- 生命周期：`FSM.StartState()` → `FSM.Update()` / `FSM.FixedUpdate()` / `FSM.OnGUI()`。

### PoolKit（对象池）

- **SimpleObjectPool**：简易对象池 `new SimpleObjectPool<T>(factory, initCount)`。
- **SafeObjectPool**：安全对象池，要求对象实现 `IPoolable` 和 `IPoolType`。
- **ListPool/DictionaryPool**：基础数据结构池化 `ListPool<T>.Get()` / `.Release2Pool()`。

### FluentAPI（链式 API）

- 对 Unity/C# 常用 API 提供链式封装：`Resources.Load<T>().Instantiate().transform.Parent(null).LocalIdentity()`。

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
