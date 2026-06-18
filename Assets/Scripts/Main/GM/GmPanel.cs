using System.Text;
using Core.Architecture;
using QFramework;
using UnityEngine;

namespace Main.GM
{
    public partial class GmPanel : MonoBehaviour, IController
    {
        private GmCommandExecutor mExecutor;
        private GmHistory mHistory;
        private readonly StringBuilder mOutputBuffer = new();
        private bool mIsOpen;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            mExecutor = new GmCommandExecutor();
            mHistory = new GmHistory();
        }

        private void OnEnable()
        {
            InputField.onSubmit.AddListener(OnSubmitCommand);
        }

        private void OnDisable()
        {
            InputField.onSubmit.RemoveListener(OnSubmitCommand);
        }

        private void OnDestroy()
        {
            if (mIsOpen)
                Time.timeScale = 1f;
        }

        private void Update()
        {
            if (!mIsOpen)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!string.IsNullOrEmpty(InputField.text))
                {
                    InputField.text = "";
                    InputField.ActivateInputField();
                }
                else
                {
                    Close();
                }

                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!InputField.isFocused)
                    InputField.ActivateInputField();

                if (mHistory.TryNavigateUp(out string histCmd))
                {
                    InputField.text = histCmd;
                    InputField.caretPosition = InputField.text.Length;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (!InputField.isFocused)
                    InputField.ActivateInputField();

                if (mHistory.TryNavigateDown(out string histCmd))
                {
                    InputField.text = histCmd;
                    InputField.caretPosition = InputField.text.Length;
                }
            }
        }

        public void Open()
        {
            mIsOpen = true;
            transform.SetAsLastSibling();
            Time.timeScale = 0f;

            mOutputBuffer.Clear();
            OutputText.text = "";

            InputField.text = "";
            InputField.ActivateInputField();
            InputField.Select();

            ShowHelp();
        }

        private void Close()
        {
            mIsOpen = false;
            Time.timeScale = 1f;

            mOutputBuffer.Clear();
            OutputText.text = "";

            gameObject.SetActive(false);
        }

        private void OnSubmitCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            string command = input.Trim();
            AppendOutput($"> {command}");
            mHistory.Add(command);

            mExecutor.Execute(command, this, AppendOutput);

            InputField.text = "";
            InputField.ActivateInputField();
        }

        private void ShowHelp()
        {
            foreach (string line in GmHelpContent.Lines)
                AppendOutput(line);
        }

        private void AppendOutput(string message)
        {
            if (message == "__CLEAR__")
            {
                mOutputBuffer.Clear();
                OutputText.text = "";
                return;
            }

            mOutputBuffer.AppendLine(message);
            OutputText.text = mOutputBuffer.ToString();
            Canvas.ForceUpdateCanvases();
            OutputScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}