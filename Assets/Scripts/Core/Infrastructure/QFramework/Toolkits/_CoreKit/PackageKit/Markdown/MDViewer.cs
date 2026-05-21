/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using Markdig;
using Markdig.Extensions.JiraLinks;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class MDViewer
    {
        public static readonly Vector2 Margin = new(6.0f, 4.0f);

        private readonly GUISkin mSkin;
        private string mText = string.Empty;
        private readonly string mCurrentPath = string.Empty;
        private readonly MDHandlerImages mHandlerImages = new();
        private readonly MDHandlerNavigate mHandlerNavigate = new();

        private MDLayout mLayout;
        private bool mRaw;

        private static readonly MDHistory mMDHistory = new();


        public string MarkdownFilePath { get; set; }

        public MDViewer(GUISkin skin, string path, string content)
        {
            mSkin = skin;
            mCurrentPath = path;
            mText = content;

            mMDHistory.OnOpen(mCurrentPath);
            mLayout = ParseDocument();

            mHandlerImages.CurrentPath = mCurrentPath;

            mHandlerNavigate.CurrentPath = mCurrentPath;
            mHandlerNavigate.MDHistory = mMDHistory;
            mHandlerNavigate.FindBlock = id => mLayout.Find(id);
            mHandlerNavigate.ScrollTo = pos => { }; // TODO: mScrollPos.y = pos;
        }

        //------------------------------------------------------------------------------

        public bool Update()
        {
            return mHandlerImages.Update();
        }


        //------------------------------------------------------------------------------

        private MDLayout ParseDocument()
        {
            MDContext context = new(mSkin, mHandlerImages, mHandlerNavigate);
            IMDLayoutBuilder builder = new(context);
            MDRendererMarkdown renderer = new(builder);

            MarkdownPipelineBuilder pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAutoLinks();

            if (!string.IsNullOrEmpty(MDPreferences.JIRA))
            {
                pipelineBuilder.UseJiraLinks(new JiraLinkOptions(MDPreferences.JIRA));
            }


            if (MDPreferences.PipedTables)
            {
                pipelineBuilder.UsePipeTables(new PipeTableOptions
                {
                    RequireHeaderSeparator = MDPreferences.PipedTablesRequireRequireHeaderSeparator
                });
            }


            MarkdownPipeline pipeline = pipelineBuilder.Build();
            pipeline.Setup(renderer);

            MarkdownDocument doc = Markdown.Parse(mText, pipeline);
            renderer.Render(doc);

            return builder.GetLayout();
        }


        //------------------------------------------------------------------------------

        private void ClearBackground(float height)
        {
            Rect rectFullScreen = new(0.0f, 0.0f, Screen.width, Mathf.Max(height, Screen.height));
            GUI.DrawTexture(rectFullScreen, mSkin.window.normal.background, ScaleMode.StretchToFill, false);
        }


        private Vector2 mScrollPos;
        //------------------------------------------------------------------------------

        public void Draw()
        {
            GUI.skin = mSkin;
            GUI.enabled = true;

            // useable width of inspector windows

            float contentWidth = EditorGUIUtility.currentViewWidth - mSkin.verticalScrollbar.fixedWidth - 2.0f * Margin.x;


            // draw content

            if (mRaw)
            {
                GUIStyle style = mSkin.GetStyle("raw");
                float width = contentWidth - mSkin.button.fixedHeight;
                float height = style.CalcHeight(new GUIContent(mText), width);

                ClearBackground(height);
                EditorGUILayout.SelectableLabel(mText, style, GUILayout.Width(width), GUILayout.Height(height));
            }
            else
            {
                ClearBackground(mLayout.Height);
                DrawMarkdown(contentWidth);
            }

            DrawToolbar(contentWidth);
        }

        private void DrawRaw(Rect rect)
        {
            EditorGUI.SelectableLabel(rect, mText, GUI.skin.GetStyle("raw"));
        }


        //------------------------------------------------------------------------------

        private void DrawToolbar(float contentWidth)
        {
            GUIStyle style = GUI.skin.button;
            float size = style.fixedHeight;
            Rect btn = new(Margin.x + contentWidth - size, Margin.y, size, size);

            if (GUI.Button(btn, string.Empty, GUI.skin.GetStyle(mRaw ? "btnRaw" : "btnFile")))
            {
                mRaw = !mRaw;
            }

            if (!mRaw)
            {
                if (mMDHistory.CanForward)
                {
                    btn.x -= size;

                    if (GUI.Button(btn, string.Empty, GUI.skin.GetStyle("btnForward")))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>(mMDHistory.Forward());
                    }
                }

                if (mMDHistory.CanBack)
                {
                    btn.x -= size;

                    if (GUI.Button(btn, string.Empty, GUI.skin.GetStyle("btnBack")))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>(mMDHistory.Back());
                    }
                }
            }
        }

        //------------------------------------------------------------------------------

        private void DrawMarkdown(float width)
        {
            switch (Event.current.type)
            {
                case EventType.Ignore:
                    break;

                case EventType.ContextClick:
                    GenericMenu menu = new();
                    menu.AddItem(new GUIContent("View Source"), false, () => mRaw = !mRaw);

                    if (MarkdownFilePath.IsNotNullAndEmpty())
                    {
                        menu.AddItem(new GUIContent("Select File"), false, () => { Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>(MarkdownFilePath); });
                    }

                    menu.ShowAsContext();
                    break;

                case EventType.Layout:
                    GUILayout.Space(mLayout.Height);
                    mLayout.Arrange(width);
                    break;

                default:
                    mLayout.Draw();
                    break;
            }
        }


        public void UpdateText(string value)
        {
            if (mText != value)
            {
                mText = value;
                mLayout = ParseDocument();
            }
        }

        private float ContentHeight(float width)
        {
            return mRaw ? GUI.skin.GetStyle("raw").CalcHeight(new GUIContent(mText), width) : mLayout.Height;
        }

        public void ResetScrollPos()
        {
            mScrollPos = Vector2.zero;
        }

        public void DrawWithRect(Rect rect)
        {
            GUI.skin = mSkin;
            GUI.enabled = true;

            // content rect

            Rect rectContainer = rect;


            // clear background

            Rect rectFullScreen = new(0.0f, rectContainer.yMin - 4.0f, Screen.width, Screen.height);
            GUI.DrawTexture(rectFullScreen, mSkin.window.normal.background, ScaleMode.StretchToFill, false);

            // scroll window

            float padLeft = 8.0f;
            float padRight = 4.0f;
            float padHoriz = padLeft + padRight;
            float scrollWidth = GUI.skin.verticalScrollbar.fixedWidth;
            float minWidth = rectContainer.width - scrollWidth - padHoriz;
            float maxHeight = ContentHeight(minWidth);

            bool hasScrollbar = maxHeight >= rectContainer.height;
            float contentWidth = hasScrollbar ? minWidth : rectContainer.width - padHoriz;
            Rect rectContent = new(-padLeft, 0.0f, contentWidth, maxHeight);

            // draw content

            using (GUI.ScrollViewScope scroll = new(rectContainer, mScrollPos, rectContent))
            {
                GUILayout.BeginHorizontal();

                mScrollPos = scroll.scrollPosition;

                if (mRaw)
                {
                    rectContent.width = minWidth - GUI.skin.button.fixedWidth;
                    DrawRaw(rectContent);
                }
                else
                {
                    DrawMarkdown(rectContainer.width);
                }

                GUILayout.Space(20); // scroll bar 增加 20 个像素
                GUILayout.EndHorizontal();
            }

            GUIStyle style = GUI.skin.button;
            float size = style.fixedHeight;
            Rect btn = new(Margin.x + contentWidth - size + 15, Margin.y + 30, size, size);

            if (GUI.Button(btn, string.Empty, GUI.skin.GetStyle(mRaw ? "btnRaw" : "btnFile")))
            {
                mRaw = !mRaw;
            }
        }
    }
}
#endif