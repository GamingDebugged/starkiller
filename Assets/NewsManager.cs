using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// Manages news content generation for the news ticker
/// Pulls from various game systems to create dynamic news
/// </summary>
public class NewsManager : MonoBehaviour
{
    [System.Serializable]
    public class NewsTemplate
    {
        public string category;
        public string[] templates;
        public int weight = 1;
    }
    
    [Header("News Ticker Reference")]
    [SerializeField] private GameObject newsTickerObject;
    [SerializeField] private TMP_Text newsTickerText;
    [SerializeField] private NewsTicker newsTicker;
    
    [Header("News Settings")]
    [SerializeField] private float newsUpdateInterval = 30f; // How often to refresh news
    [SerializeField] private int maxNewsItems = 5; // How many items in the ticker
    [SerializeField] private string newsItemSeparator = " +++ ";
    [SerializeField] private bool includeTimestamps = false;
    
    [Header("Content Templates")]
    [SerializeField] private List<NewsTemplate> propagandaTemplates = new List<NewsTemplate>();
    [SerializeField] private List<NewsTemplate> ruleChangeTemplates = new List<NewsTemplate>();
    [SerializeField] private List<NewsTemplate> incidentTemplates = new List<NewsTemplate>();
    [SerializeField] private List<NewsTemplate> flavorTemplates = new List<NewsTemplate>();
    
    [Header("Content Mix")]
    [Range(0, 1)] [SerializeField] private float propagandaWeight = 0.3f;
    [Range(0, 1)] [SerializeField] private float incidentWeight = 0.3f;
    [Range(0, 1)] [SerializeField] private float ruleWeight = 0.2f;
    [Range(0, 1)] [SerializeField] private float flavorWeight = 0.2f;
    
    // System references
    private ConsequenceManager consequenceManager;
    private DailyRulesGenerator rulesGenerator;
    private GameManager gameManager;
    
    // Current news content
    private List<string> currentNewsItems = new List<string>();
    private string currentNewsString = "";
    private int currentDay = 1;
    
    // Singleton
    private static NewsManager _instance;
    public static NewsManager Instance => _instance;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        
        // Initialize default templates if empty
        InitializeDefaultTemplates();
    }
    
    void Start()
    {
        // Find system references
        consequenceManager = FindFirstObjectByType<ConsequenceManager>();
        rulesGenerator = DailyRulesGenerator.Instance;
        gameManager = FindFirstObjectByType<GameManager>();
        
        // Find news ticker components if not assigned
        if (newsTickerObject == null)
        {
            newsTickerObject = GameObject.Find("NewsTicker");
        }
        
        if (newsTickerObject != null)
        {
            if (newsTickerText == null)
            {
                newsTickerText = newsTickerObject.GetComponent<TMP_Text>();
            }
            if (newsTicker == null)
            {
                newsTicker = newsTickerObject.GetComponent<NewsTicker>();
            }
        }
        
        // Generate initial news
        GenerateNewsForDay(1);
        
        // Start news update cycle
        InvokeRepeating(nameof(RefreshNews), newsUpdateInterval, newsUpdateInterval);
        
        Debug.Log("NewsManager initialized");
    }
    
    /// <summary>
    /// Initialize default news templates
    /// </summary>
    private void InitializeDefaultTemplates()
    {
        // Propaganda templates
        if (propagandaTemplates.Count == 0)
        {
            propagandaTemplates.Add(new NewsTemplate
            {
                category = "strength",
                templates = new string[]
                {
                    "The Imperium's might grows stronger! Another glorious day at Starkiller Base.",
                    "Imperium forces report complete control of all sectors.",
                    "Supreme Leader's vision guides us to inevitable victory!",
                    "Starkiller Base efficiency at all-time high under your command.",
                    "The Galaxy trembles before the Imperium's power!"
                },
                weight = 2
            });
            
            propagandaTemplates.Add(new NewsTemplate
            {
                category = "enemy",
                templates = new string[]
                {
                    "Insurgent forces continue to fail against Imperium superiority.",
                    "Another rebel cell eliminated by our brave troops.",
                    "The so-called 'rebellion' crumbles before Imperium justice.",
                    "Enemies of order will find no mercy in Imperium space."
                },
                weight = 1
            });
        }
        
        // Rule change templates
        if (ruleChangeTemplates.Count == 0)
        {
            ruleChangeTemplates.Add(new NewsTemplate
            {
                category = "new_rule",
                templates = new string[]
                {
                    "ATTENTION: New security protocols in effect. Check daily briefing.",
                    "Updated access codes now active. Expired codes will be rejected.",
                    "Security Notice: Today's approved ship list has been updated.",
                    "Command Directive: Enhanced screening procedures now mandatory.",
                    "New regulations from High Command. Compliance is mandatory."
                },
                weight = 1
            });
        }
        
        // Incident templates
        if (incidentTemplates.Count == 0)
        {
            incidentTemplates.Add(new NewsTemplate
            {
                category = "minor",
                templates = new string[]
                {
                    "Minor security variance detected and resolved in Sector {SECTOR}.",
                    "Routine inspection completed. All systems nominal.",
                    "Security protocols maintained at optimal efficiency."
                },
                weight = 1
            });
            
            incidentTemplates.Add(new NewsTemplate
            {
                category = "major",
                templates = new string[]
                {
                    "Security alert in Sector {SECTOR}. Situation contained.",
                    "Unauthorized access attempt thwarted. {CASUALTIES} casualties reported.",
                    "High Command investigating security breach. Expect inspections.",
                    "Critical: Multiple security failures detected. Command notified."
                },
                weight = 1
            });
        }
        
        // Flavor templates
        if (flavorTemplates.Count == 0)
        {
            flavorTemplates.Add(new NewsTemplate
            {
                category = "daily",
                templates = new string[]
                {
                    "Starkiller Base operating at peak efficiency.",
                    "Today marks Day {DAY} of flawless Imperium operations.",
                    "Weather Control: Artificial atmosphere stable.",
                    "Reminder: Report suspicious activity immediately.",
                    "Commissary special: Blue milk half price in Sector C.",
                    "Maintenance complete on primary weapon systems.",
                    "Training drills scheduled for 1400 hours.",
                    "Glory to the Imperium! Long live the Supreme Leader!"
                },
                weight = 1
            });
        }
    }
    
    /// <summary>
    /// Generate news content for a new day
    /// </summary>
    public void GenerateNewsForDay(int day)
    {
        currentDay = day;
        currentNewsItems.Clear();
        
        Debug.Log($"Generating news for day {day}");
        
        // Add rule changes if it's a new day
        if (rulesGenerator != null && day > 1)
        {
            AddRuleChangeNews();
        }
        
        // Add recent incidents
        if (consequenceManager != null)
        {
            AddIncidentNews();
        }
        
        // Add propaganda
        AddPropagandaNews();
        
        // Add flavor text
        AddFlavorNews();
        
        // Ensure we have content
        if (currentNewsItems.Count == 0)
        {
            currentNewsItems.Add("Starkiller Base Command Center Online. Awaiting daily briefing...");
        }
        
        // Compile news string
        CompileNewsString();
        
        // Update the ticker
        UpdateNewsTicker();
    }
    
    /// <summary>
    /// Add rule change announcements
    /// </summary>
    private void AddRuleChangeNews()
    {
        if (rulesGenerator.CurrentRules.Count > 0)
        {
            // Add general rule change notification
            string ruleNews = GetRandomTemplate(ruleChangeTemplates, "new_rule");
            currentNewsItems.Add(ruleNews);
            
            // Specifically mention access codes if they changed
            if (rulesGenerator.CurrentAccessCodes.Count > 0)
            {
                string codeNews = $"Valid access codes for Day {currentDay}: {string.Join(", ", rulesGenerator.CurrentAccessCodes)}";
                currentNewsItems.Add(codeNews);
            }
        }
    }
    
    /// <summary>
    /// Add incident reports from ConsequenceManager
    /// </summary>
    private void AddIncidentNews()
    {
        // Get today's incident summary
        int minorCount = consequenceManager.GetIncidentCount(StarkillerBaseCommand.Consequence.SeverityLevel.Minor);
        int moderateCount = consequenceManager.GetIncidentCount(StarkillerBaseCommand.Consequence.SeverityLevel.Moderate);
        int severeCount = consequenceManager.GetIncidentCount(StarkillerBaseCommand.Consequence.SeverityLevel.Severe);
        int criticalCount = consequenceManager.GetIncidentCount(StarkillerBaseCommand.Consequence.SeverityLevel.Critical);
        
        // Add news based on severity
        if (criticalCount > 0)
        {
            string news = GetRandomTemplate(incidentTemplates, "major")
                .Replace("{SECTOR}", Random.Range(1, 9).ToString())
                .Replace("{CASUALTIES}", consequenceManager.GetTotalCasualties().ToString());
            currentNewsItems.Add($"CRITICAL ALERT: {news}");
        }
        else if (severeCount > 0)
        {
            string news = GetRandomTemplate(incidentTemplates, "major")
                .Replace("{SECTOR}", Random.Range(1, 9).ToString())
                .Replace("{CASUALTIES}", consequenceManager.GetTotalCasualties().ToString());
            currentNewsItems.Add($"SECURITY NOTICE: {news}");
        }
        else if (moderateCount > 0 || minorCount > 0)
        {
            string news = GetRandomTemplate(incidentTemplates, "minor")
                .Replace("{SECTOR}", Random.Range(1, 9).ToString());
            currentNewsItems.Add(news);
        }
        
        // Add casualty report if significant
        int totalCasualties = consequenceManager.GetTotalCasualties();
        if (totalCasualties > 10)
        {
            currentNewsItems.Add($"Imperial losses this week: {totalCasualties} brave soldiers. Their sacrifice will be avenged.");
        }
    }
    
    /// <summary>
    /// Add propaganda messages
    /// </summary>
    private void AddPropagandaNews()
    {
        int propagandaCount = Mathf.RoundToInt(maxNewsItems * propagandaWeight);
        for (int i = 0; i < propagandaCount; i++)
        {
            string category = Random.value > 0.5f ? "strength" : "enemy";
            string news = GetRandomTemplate(propagandaTemplates, category);
            currentNewsItems.Add(news);
        }
    }
    
    /// <summary>
    /// Add flavor text and daily messages
    /// </summary>
    private void AddFlavorNews()
    {
        int flavorCount = Mathf.RoundToInt(maxNewsItems * flavorWeight);
        for (int i = 0; i < flavorCount; i++)
        {
            string news = GetRandomTemplate(flavorTemplates, "daily")
                .Replace("{DAY}", currentDay.ToString());
            currentNewsItems.Add(news);
        }
        
        // Add special day messages
        if (currentDay % 3 == 0)
        {
            currentNewsItems.Add("Imperial Inspection today. All personnel maintain exemplary standards.");
        }
        
        if (currentDay % 5 == 0)
        {
            currentNewsItems.Add("The Order delegation arriving. Sacred visitors must receive priority clearance.");
        }
    }
    
    /// <summary>
    /// Get a random template from a category
    /// </summary>
    private string GetRandomTemplate(List<NewsTemplate> templates, string category)
    {
        var matchingTemplates = templates.Where(t => t.category == category).ToList();
        if (matchingTemplates.Count == 0)
        {
            // Fallback to any template
            matchingTemplates = templates;
        }
        
        if (matchingTemplates.Count == 0)
        {
            return "No news available.";
        }
        
        // Weighted random selection
        float totalWeight = matchingTemplates.Sum(t => t.weight);
        float randomValue = Random.value * totalWeight;
        float currentWeight = 0;
        
        foreach (var template in matchingTemplates)
        {
            currentWeight += template.weight;
            if (randomValue <= currentWeight)
            {
                return template.templates[Random.Range(0, template.templates.Length)];
            }
        }
        
        // Fallback
        var fallback = matchingTemplates[Random.Range(0, matchingTemplates.Count)];
        return fallback.templates[Random.Range(0, fallback.templates.Length)];
    }
    
    /// <summary>
    /// Compile news items into a single string
    /// </summary>
    private void CompileNewsString()
    {
        // Shuffle news items for variety
        for (int i = 0; i < currentNewsItems.Count; i++)
        {
            int randomIndex = Random.Range(i, currentNewsItems.Count);
            string temp = currentNewsItems[i];
            currentNewsItems[i] = currentNewsItems[randomIndex];
            currentNewsItems[randomIndex] = temp;
        }
        
        // Limit to max items
        if (currentNewsItems.Count > maxNewsItems)
        {
            currentNewsItems = currentNewsItems.Take(maxNewsItems).ToList();
        }
        
        // Add timestamps if enabled
        if (includeTimestamps)
        {
            for (int i = 0; i < currentNewsItems.Count; i++)
            {
                string timestamp = $"[{8 + i:00}:00] ";
                currentNewsItems[i] = timestamp + currentNewsItems[i];
            }
        }
        
        // Join with separator
        currentNewsString = string.Join(newsItemSeparator, currentNewsItems);
        
        // Add some padding for smooth scrolling
        currentNewsString = "    " + currentNewsString + "    ";
    }
    
    /// <summary>
    /// Update the news ticker with new content
    /// </summary>
    private void UpdateNewsTicker()
    {
        if (newsTickerText != null)
        {
            newsTickerText.text = currentNewsString;
            Debug.Log($"Updated news ticker with: {currentNewsString.Substring(0, Mathf.Min(50, currentNewsString.Length))}...");
        }
        else
        {
            Debug.LogWarning("News ticker text component not found!");
        }
    }
    
    /// <summary>
    /// Refresh news content (called periodically)
    /// </summary>
    private void RefreshNews()
    {
        // Re-generate some news items for variety
        // Keep important news but refresh flavor text
        List<string> importantNews = currentNewsItems.Where(n => 
            n.Contains("CRITICAL") || 
            n.Contains("SECURITY") || 
            n.Contains("Valid access codes")).ToList();
        
        currentNewsItems = importantNews;
        
        // Add fresh propaganda and flavor
        AddPropagandaNews();
        AddFlavorNews();
        
        // Recompile and update
        CompileNewsString();
        UpdateNewsTicker();
    }
    
    /// <summary>
    /// Add a custom news item (for special events)
    /// </summary>
    public void AddCustomNews(string newsItem, bool priority = false)
    {
        if (priority)
        {
            currentNewsItems.Insert(0, newsItem);
        }
        else
        {
            currentNewsItems.Add(newsItem);
        }
        
        // Recompile and update immediately
        CompileNewsString();
        UpdateNewsTicker();
    }
    
    /// <summary>
    /// Add an urgent news flash
    /// </summary>
    public void AddUrgentNews(string newsItem)
    {
        string urgentNews = $"BREAKING: {newsItem}";
        AddCustomNews(urgentNews, true);
    }
}