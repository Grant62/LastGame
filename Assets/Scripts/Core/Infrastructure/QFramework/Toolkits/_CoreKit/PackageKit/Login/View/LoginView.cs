#if UNITY_EDITOR
namespace QFramework
{
    internal class LoginView : VerticalLayout, IController
    {
        public LoginView()
        {
            IMGUIHorizontalLayout usernameLine = EasyIMGUI.Horizontal().Parent(this);
            EasyIMGUI.Label().Text("username:").Parent(usernameLine);
            IMGUITextField username = EasyIMGUI.TextField().Parent(usernameLine);

            IMGUIHorizontalLayout passwordLine = EasyIMGUI.Horizontal().Parent(this);
            EasyIMGUI.Label().Text("password:").Parent(passwordLine);
            IMGUITextField password = EasyIMGUI.TextField().PasswordMode().Parent(passwordLine);

            EasyIMGUI.Button()
                .Text("登录")
                .OnClick(() => { this.SendCommand(new LoginCommand(username.Content.Value, password.Content.Value)); })
                .Parent(this);

            EasyIMGUI.Button()
                .Text("注册")
                .OnClick(() => { this.SendCommand<OpenRegisterWebsiteCommand>(); })
                .Parent(this);
        }

        protected override void OnDisposed() { }

        public IArchitecture GetArchitecture()
        {
            return PackageKitLoginApp.Interface;
        }
    }
}
#endif