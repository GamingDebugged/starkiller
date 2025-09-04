using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace Starkiller.Core.Managers.Editor
{
    /// <summary>
    /// Editor utility to create Personal Data Log UI structure automatically
    /// </summary>
    public class PersonalDataLogUICreator : EditorWindow
    {
        [MenuItem("Starkiller/Create Personal Data Log UI")]
        public static void CreatePersonalDataLogUI()
        {
            // Find or create Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            // Create main panel
            GameObject dataLogPanel = CreateMainPanel(canvas.transform);
            
            // Create header
            CreateHeader(dataLogPanel.transform);
            
            // Create content area with three sections
            GameObject contentArea = CreateContentArea(dataLogPanel.transform);
            CreateImperiumNewsSection(contentArea.transform);
            CreateFamilyChatSection(contentArea.transform);
            CreateFrontierEzineSection(contentArea.transform);
            
            // Create footer
            CreateFooter(dataLogPanel.transform);
            
            // Create entry templates
            CreateNewsEntryTemplate();
            CreateFamilyActionTemplate();
            
            Debug.Log("Personal Data Log UI structure created successfully!");
            Selection.activeGameObject = dataLogPanel;
        }

        private static GameObject CreateMainPanel(Transform parent)
        {
            GameObject panel = new GameObject("PersonalDataLogPanel");
            panel.transform.SetParent(parent, false);
            
            // Add RectTransform and set to full screen
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add Image component for background
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.05f, 0.1f, 0.2f, 0.95f);
            
            // Add CanvasGroup for fade control
            CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            // Add PersonalDataLogManager component
            panel.AddComponent<Starkiller.Core.Managers.PersonalDataLogManager>();
            
            panel.SetActive(false); // Start hidden
            
            return panel;
        }

        private static void CreateHeader(Transform parent)
        {
            GameObject header = new GameObject("Header");
            header.transform.SetParent(parent, false);
            
            RectTransform rect = header.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.9f);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add Horizontal Layout Group
            HorizontalLayoutGroup layout = header.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.padding = new RectOffset(20, 20, 10, 10);
            
            // Create title text
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(header.transform, false);
            
            TextMeshProUGUI title = titleGO.AddComponent<TextMeshProUGUI>();
            title.text = "PERSONAL DATA LOG - DAY 1";
            title.fontSize = 24;
            title.fontStyle = FontStyles.Bold;
            title.color = new Color(0.8f, 0.9f, 1f, 1f);
            title.alignment = TextAlignmentOptions.Left;
            
            // Add Layout Element
            LayoutElement titleLayout = titleGO.AddComponent<LayoutElement>();
            titleLayout.flexibleWidth = 1;
        }

        private static GameObject CreateContentArea(Transform parent)
        {
            GameObject contentArea = new GameObject("ContentArea");
            contentArea.transform.SetParent(parent, false);
            
            RectTransform rect = contentArea.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.1f);
            rect.anchorMax = new Vector2(1, 0.9f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add Horizontal Layout Group for three columns
            HorizontalLayoutGroup layout = contentArea.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.spacing = 20;
            layout.padding = new RectOffset(20, 20, 10, 10);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            
            return contentArea;
        }

        private static GameObject CreateImperiumNewsSection(Transform parent)
        {
            GameObject section = CreateFeedSection(parent, "ImperiumNewsSection", "IMPERIUM NEWS");
            
            // Add imperial red accent
            Transform header = section.transform.Find("SectionHeader");
            if (header != null)
            {
                Image headerBg = header.GetComponent<Image>();
                if (headerBg != null)
                {
                    headerBg.color = new Color(0.8f, 0.1f, 0.1f, 0.3f);
                }
            }
            
            return section;
        }

        private static GameObject CreateFamilyChatSection(Transform parent)
        {
            GameObject section = CreateFeedSection(parent, "FamilyChatSection", "FAMILY GROUP CHAT");
            
            // Add warm orange accent
            Transform header = section.transform.Find("SectionHeader");
            if (header != null)
            {
                Image headerBg = header.GetComponent<Image>();
                if (headerBg != null)
                {
                    headerBg.color = new Color(1f, 0.6f, 0.2f, 0.3f);
                }
            }
            
            return section;
        }

        private static GameObject CreateFrontierEzineSection(Transform parent)
        {
            GameObject section = CreateFeedSection(parent, "FrontierEzineSection", "FRONTIER E-ZINE");
            
            // Add cyan accent
            Transform header = section.transform.Find("SectionHeader");
            if (header != null)
            {
                Image headerBg = header.GetComponent<Image>();
                if (headerBg != null)
                {
                    headerBg.color = new Color(0.2f, 0.8f, 1f, 0.3f);
                }
            }
            
            return section;
        }

        private static GameObject CreateFeedSection(Transform parent, string sectionName, string title)
        {
            GameObject section = new GameObject(sectionName);
            section.transform.SetParent(parent, false);
            
            // Add vertical layout for header and content
            VerticalLayoutGroup sectionLayout = section.AddComponent<VerticalLayoutGroup>();
            sectionLayout.childAlignment = TextAnchor.UpperLeft;
            sectionLayout.childForceExpandWidth = true;
            sectionLayout.childForceExpandHeight = false;
            sectionLayout.spacing = 5;
            
            // Add Layout Element
            LayoutElement sectionElement = section.AddComponent<LayoutElement>();
            sectionElement.flexibleWidth = 1;
            sectionElement.flexibleHeight = 1;
            
            // Create section header
            GameObject headerGO = new GameObject("SectionHeader");
            headerGO.transform.SetParent(section.transform, false);
            
            Image headerBg = headerGO.AddComponent<Image>();
            headerBg.color = new Color(0.1f, 0.2f, 0.3f, 0.5f);
            
            LayoutElement headerLayout = headerGO.AddComponent<LayoutElement>();
            headerLayout.preferredHeight = 40;
            
            // Header text
            GameObject headerTextGO = new GameObject("Title");
            headerTextGO.transform.SetParent(headerGO.transform, false);
            
            RectTransform headerTextRect = headerTextGO.AddComponent<RectTransform>();
            headerTextRect.anchorMin = Vector2.zero;
            headerTextRect.anchorMax = Vector2.one;
            headerTextRect.offsetMin = new Vector2(10, 0);
            headerTextRect.offsetMax = new Vector2(-10, 0);
            
            TextMeshProUGUI headerText = headerTextGO.AddComponent<TextMeshProUGUI>();
            headerText.text = title;
            headerText.fontSize = 16;
            headerText.fontStyle = FontStyles.Bold;
            headerText.color = new Color(0.8f, 0.9f, 1f, 1f);
            headerText.alignment = TextAlignmentOptions.Left;
            
            // Create scroll view
            GameObject scrollViewGO = new GameObject("ScrollView");
            scrollViewGO.transform.SetParent(section.transform, false);
            
            ScrollRect scrollRect = scrollViewGO.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            
            LayoutElement scrollLayout = scrollViewGO.AddComponent<LayoutElement>();
            scrollLayout.flexibleHeight = 1;
            
            // Create viewport
            GameObject viewportGO = new GameObject("Viewport");
            viewportGO.transform.SetParent(scrollViewGO.transform, false);
            
            RectTransform viewportRect = viewportGO.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            
            Mask viewportMask = viewportGO.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            
            Image viewportImage = viewportGO.AddComponent<Image>();
            viewportImage.color = Color.clear;
            
            scrollRect.viewport = viewportRect;
            
            // Create content
            GameObject contentGO = new GameObject("Content");
            contentGO.transform.SetParent(viewportGO.transform, false);
            
            RectTransform contentRect = contentGO.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            
            VerticalLayoutGroup contentLayout = contentGO.AddComponent<VerticalLayoutGroup>();
            contentLayout.childAlignment = TextAnchor.UpperLeft;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            contentLayout.spacing = 10;
            contentLayout.padding = new RectOffset(10, 10, 10, 10);
            
            ContentSizeFitter contentFitter = contentGO.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scrollRect.content = contentRect;
            
            return section;
        }

        private static void CreateFooter(Transform parent)
        {
            GameObject footer = new GameObject("Footer");
            footer.transform.SetParent(parent, false);
            
            RectTransform rect = footer.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0.1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Create continue button
            GameObject buttonGO = new GameObject("ContinueButton");
            buttonGO.transform.SetParent(footer.transform, false);
            
            RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.7f, 0.2f);
            buttonRect.anchorMax = new Vector2(0.95f, 0.8f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            Button button = buttonGO.AddComponent<Button>();
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = new Color(1f, 1f, 0.3f, 0.8f);
            
            // Button text
            GameObject buttonTextGO = new GameObject("ButtonText");
            buttonTextGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform textRect = buttonTextGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI buttonText = buttonTextGO.AddComponent<TextMeshProUGUI>();
            buttonText.text = "CONTINUE TO BRIEFING";
            buttonText.fontSize = 16;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = Color.black;
            buttonText.alignment = TextAlignmentOptions.Center;
        }

        private static void CreateNewsEntryTemplate()
        {
            GameObject template = new GameObject("NewsEntryTemplate");
            
            // Add Image background
            Image bg = template.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.15f, 0.25f, 0.7f);
            
            // Add Layout Element
            LayoutElement layout = template.AddComponent<LayoutElement>();
            layout.preferredHeight = 80;
            
            // Add Content Size Fitter
            ContentSizeFitter fitter = template.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Add Vertical Layout Group
            VerticalLayoutGroup vlg = template.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(15, 15, 10, 10);
            vlg.spacing = 5;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            
            // Create headline
            GameObject headlineGO = new GameObject("Headline");
            headlineGO.transform.SetParent(template.transform, false);
            
            TextMeshProUGUI headline = headlineGO.AddComponent<TextMeshProUGUI>();
            headline.text = "News Headline";
            headline.fontSize = 14;
            headline.fontStyle = FontStyles.Bold;
            headline.color = new Color(0.8f, 0.9f, 1f, 1f);
            
            // Create content text
            GameObject contentGO = new GameObject("Content");
            contentGO.transform.SetParent(template.transform, false);
            
            TextMeshProUGUI content = contentGO.AddComponent<TextMeshProUGUI>();
            content.text = "News content goes here...";
            content.fontSize = 12;
            content.color = new Color(0.7f, 0.8f, 0.9f, 1f);
            content.enableWordWrapping = true;
            
            // Save as prefab
            string prefabPath = "Assets/_RefactoredScripts/Core/Managers/Prefabs/NewsEntryTemplate.prefab";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(prefabPath));
            PrefabUtility.SaveAsPrefabAsset(template, prefabPath);
            
            DestroyImmediate(template);
        }

        private static void CreateFamilyActionTemplate()
        {
            GameObject template = new GameObject("FamilyActionTemplate");
            
            // Add Image background (slightly different color)
            Image bg = template.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.1f, 0.25f, 0.7f);
            
            // Add Layout Element
            LayoutElement layout = template.AddComponent<LayoutElement>();
            layout.preferredHeight = 120;
            
            // Add Content Size Fitter
            ContentSizeFitter fitter = template.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Add Vertical Layout Group
            VerticalLayoutGroup vlg = template.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(15, 15, 10, 10);
            vlg.spacing = 8;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            
            // Create headline
            GameObject headlineGO = new GameObject("Headline");
            headlineGO.transform.SetParent(template.transform, false);
            
            TextMeshProUGUI headline = headlineGO.AddComponent<TextMeshProUGUI>();
            headline.text = "Family Message";
            headline.fontSize = 14;
            headline.fontStyle = FontStyles.Bold;
            headline.color = new Color(1f, 0.6f, 0.2f, 1f);
            
            // Create content text
            GameObject contentGO = new GameObject("Content");
            contentGO.transform.SetParent(template.transform, false);
            
            TextMeshProUGUI content = contentGO.AddComponent<TextMeshProUGUI>();
            content.text = "Family message content...";
            content.fontSize = 12;
            content.color = new Color(0.7f, 0.8f, 0.9f, 1f);
            content.enableWordWrapping = true;
            
            // Create action button
            GameObject buttonGO = new GameObject("ActionButton");
            buttonGO.transform.SetParent(template.transform, false);
            
            LayoutElement buttonLayout = buttonGO.AddComponent<LayoutElement>();
            buttonLayout.preferredHeight = 30;
            
            Button button = buttonGO.AddComponent<Button>();
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = new Color(1f, 1f, 0.3f, 0.8f);
            
            // Button text
            GameObject buttonTextGO = new GameObject("ButtonText");
            buttonTextGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform textRect = buttonTextGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI buttonText = buttonTextGO.AddComponent<TextMeshProUGUI>();
            buttonText.text = "Pay 100 Credits";
            buttonText.fontSize = 12;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = Color.black;
            buttonText.alignment = TextAlignmentOptions.Center;
            
            // Save as prefab
            string prefabPath = "Assets/_RefactoredScripts/Core/Managers/Prefabs/FamilyActionTemplate.prefab";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(prefabPath));
            PrefabUtility.SaveAsPrefabAsset(template, prefabPath);
            
            DestroyImmediate(template);
        }
    }
}