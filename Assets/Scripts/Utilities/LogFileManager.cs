using System;
using System.IO;
using System.Text;
using UnityEngine;

public class LogFileManager : Singleton<LogFileManager>
{
    private const string LOG_FOLDER = "GameLogs";
    private string logFilePath;
    private StreamWriter logWriter;
    private int lastLoggedTick = -1;

    protected override void Awake()
    {
        base.Awake();
        CreateLogFile();
    }

    private void Start()
    {
        WriteInitialParameters();
        WriteCSVHeader();

        // Subscribe to Timeline's OnTick event
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick += OnTimelineTick;
        }
        else
        {
            Debug.LogError("LogFileManager: Timeline instance not found!");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from OnTick event
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick -= OnTimelineTick;
        }

        // Close the log file
        CloseLogFile();
    }

    private void CreateLogFile()
    {
        try
        {
            // Create log folder if it doesn't exist
            string logFolderPath = Path.Combine(Application.dataPath, "..", LOG_FOLDER);
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            // Create unique filename based on current date and time
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"GameLog_{timestamp}.csv";
            logFilePath = Path.Combine(logFolderPath, fileName);

            // Create the file and keep it open for writing
            logWriter = new StreamWriter(logFilePath, false, Encoding.UTF8);
            logWriter.AutoFlush = true; // Ensure data is written immediately

            Debug.Log($"Log file created at: {logFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create log file: {e.Message}");
        }
    }

    private void WriteInitialParameters()
    {
        if (logWriter == null) return;

        try
        {
            // Find the parameter assets
            TimeLineParameter timeLineParameter = Resources.FindObjectsOfTypeAll<TimeLineParameter>()[0];
            ComputePowerParameter computePowerParameter = Resources.FindObjectsOfTypeAll<ComputePowerParameter>()[0];
            HumanParameter humanParameter = Resources.FindObjectsOfTypeAll<HumanParameter>()[0];

            logWriter.WriteLine("# Game Session Log");
            logWriter.WriteLine($"# Session Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            logWriter.WriteLine("#");
            logWriter.WriteLine("# Initial Parameters:");
            logWriter.WriteLine($"# TickDuration (months): {timeLineParameter.TickDuration}");
            logWriter.WriteLine($"# StartDate: {timeLineParameter.StartDate:yyyy-MM-dd}");
            logWriter.WriteLine($"# ComputePower_BaseModifier_AddedValue: {computePowerParameter.BaseModifier.AddedValue}");
            logWriter.WriteLine($"# Human_BaseModifier_AddedValue: {humanParameter.BaseModifier.AddedValue}");
            logWriter.WriteLine($"# Human_GaugeImpactPerHuman: {humanParameter.GaugeImpactPerHuman}");
            logWriter.WriteLine("#");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write initial parameters: {e.Message}");
        }
    }

    private void WriteCSVHeader()
    {
        if (logWriter == null) return;

        try
        {
            logWriter.WriteLine("Tick,Date,ClimateGauge,SocietalGauge,TrustGauge,HumanCount,ComputePower,EventType,EventDescription");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write CSV header: {e.Message}");
        }
    }

    private void OnTimelineTick(int currentTick)
    {
        if (logWriter == null) return;

        try
        {
            // Get current game state
            string date = Timeline.Instance.currentDate.ToString("yyyy-MM-dd");
            float climateValue = GaugeManager.Instance.ClimateGauge.value;
            float societalValue = GaugeManager.Instance.SocietalGauge.value;
            float trustValue = GaugeManager.Instance.TrustGauge.value;
            long humanCount = Human.Instance.HumanCount;
            int computePower = ComputePower.Instance.value;

            // Write tick data (without event data for normal ticks)
            logWriter.WriteLine($"{currentTick},{date},{climateValue:F4},{societalValue:F4},{trustValue:F4},{humanCount},{computePower},,");

            lastLoggedTick = currentTick;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to log tick data: {e.Message}");
        }
    }

    /// <summary>
    /// Logs a user action event at the current tick
    /// </summary>
    /// <param name="eventType">Type of event: "Upgrade", "Event", or "Warning"</param>
    /// <param name="eventDescription">Description of the event</param>
    public void LogUserAction(string eventType, string eventDescription)
    {
        if (logWriter == null) return;

        // Validate event type
        if (eventType != "Upgrade" && eventType != "Event" && eventType != "Warning")
        {
            Debug.LogWarning($"Invalid event type: {eventType}. Expected: Upgrade, Event, or Warning");
        }

        try
        {
            // Get current game state
            int currentTick = Timeline.Instance.CurrentTick;
            string date = Timeline.Instance.currentDate.ToString("yyyy-MM-dd");
            float climateValue = GaugeManager.Instance.ClimateGauge.value;
            float societalValue = GaugeManager.Instance.SocietalGauge.value;
            float trustValue = GaugeManager.Instance.TrustGauge.value;
            long humanCount = Human.Instance.HumanCount;
            int computePower = ComputePower.Instance.value;

            // Escape description if it contains commas or quotes
            string escapedDescription = EscapeCSVField(eventDescription);

            // Write event data
            logWriter.WriteLine($"{currentTick},{date},{climateValue:F4},{societalValue:F4},{trustValue:F4},{humanCount},{computePower},{eventType},{escapedDescription}");

            Debug.Log($"Logged user action: [{eventType}] {eventDescription}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to log user action: {e.Message}");
        }
    }

    private string EscapeCSVField(string field)
    {
        if (string.IsNullOrEmpty(field)) return field;

        // If field contains comma, newline, or quote, wrap in quotes and escape internal quotes
        if (field.Contains(",") || field.Contains("\n") || field.Contains("\""))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }

    private void CloseLogFile()
    {
        if (logWriter != null)
        {
            try
            {
                logWriter.WriteLine("#");
                logWriter.WriteLine($"# Session Ended: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                logWriter.Close();
                logWriter = null;
                Debug.Log("Log file closed successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to close log file: {e.Message}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        CloseLogFile();
    }
}
