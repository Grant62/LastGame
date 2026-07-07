using System.Collections;
using System.Text;
using Core.Architecture;
using Cysharp.Threading.Tasks;
using Features.Card.Event;
using Features.Combat.System;
using QFramework;
using UnityEngine;

namespace Main.GM
{
    public partial class GmPanel : MonoBehaviour, IController
    {
        private GmCommandExecutor mExecutor;
        private GmHistory mHistory;
        private readonly StringBuilder mOutputBuffer = new();

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
            this.GetSystem<IInteractionSystem>().EndAnimation();
            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        public void Open()
        {
            this.GetSystem<IInteractionSystem>().BeginAnimation();
            GameMain.Interface.SendEvent<ForceClearHoverEvent>();
            GameMain.Interface.SendEvent<ForceEndAllDragsEvent>();
            Time.timeScale = 0f;

            mOutputBuffer.Clear();
            OutputText.text = "";

            InputField.text = "";
            InputField.ActivateInputField();
            InputField.Select();

            ShowHelp();

            transform.SetAsLastSibling();
            GameMain.Interface.GetSystem<IPopupStackSystem>().Push(gameObject);
        }

        public void Close()
        {
            GameMain.Interface.GetSystem<IPopupStackSystem>().Remove(gameObject);
            Time.timeScale = 1f;
            mOutputBuffer.Clear();
            OutputText.text = "";
            gameObject.SetActive(false);
        }

        private void Update()
        {
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

        private void OnSubmitCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            string command = input.Trim();
            AppendOutput($"> {command}");
            mHistory.Add(command);

            mExecutor.Execute(command, this, AppendOutput);

            InputField.text = "";
            RefocusInputAsync().Forget();
        }

        private async UniTaskVoid RefocusInputAsync()
        {
            await UniTask.NextFrame();
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