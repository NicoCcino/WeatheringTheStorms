using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class LogFileManager : Singleton<LogFileManager>
{
    private const string LOG_FOLDER = "GameLogs";
    private string logFilePath;
    private StreamWriter logWriter;
    
    // Cache for the current tick's data
    private string cachedTickLine = null;

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
            GaugeManager.Instance.OnGaugeChanged += OnGaugeChanged;
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
            GaugeManager.Instance.OnGaugeChanged -= OnGaugeChanged;
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
            logWriter.WriteLine($"# ComputePower_BaseModifier_AddedValue: {computePowerParameter.BaseModifier.AddedValue.ToString(CultureInfo.InvariantCulture)}");
            logWriter.WriteLine($"# Human_BaseModifier_AddedValue: {humanParameter.BaseModifier.AddedValue.ToString(CultureInfo.InvariantCulture)}");
            logWriter.WriteLine($"# Human_GaugeImpactPerHuman: {humanParameter.GaugeImpactPerHuman.ToString(CultureInfo.InvariantCulture)}");
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

    private void OnGaugeChanged(uint currentTick)
    {
        if (logWriter == null) return;

        try
        {
            // Write the previous tick's cached line (if it exists)
            if (cachedTickLine != null)
            {
                logWriter.WriteLine(cachedTickLine);
            }

            // Get current game state and cache it
            string date = Timeline.Instance.currentDate.ToString("yyyy-MM-dd");
            float climateValue = GaugeManager.Instance.ClimateGauge.value;
            float societalValue = GaugeManager.Instance.SocietalGauge.value;
            float trustValue = GaugeManager.Instance.TrustGauge.value;
            long humanCount = Human.Instance.HumanCount;
            int computePower = ComputePower.Instance.value;

            // Cache the current tick data (without event data initially)
            // Use InvariantCulture to ensure decimal separator is always a period (not comma)
            cachedTickLine = $"{currentTick},{date},{climateValue.ToString("F4", CultureInfo.InvariantCulture)},{societalValue.ToString("F4", CultureInfo.InvariantCulture)},{trustValue.ToString("F4", CultureInfo.InvariantCulture)},{humanCount},{computePower},,";
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
            // If no cached line exists yet, create one for the current tick
            if (cachedTickLine == null)
            {
                uint currentTick = Timeline.Instance.CurrentTick;
                string date = Timeline.Instance.currentDate.ToString("yyyy-MM-dd");
                float climateValue = GaugeManager.Instance.ClimateGauge.value;
                float societalValue = GaugeManager.Instance.SocietalGauge.value;
                float trustValue = GaugeManager.Instance.TrustGauge.value;
                long humanCount = Human.Instance.HumanCount;
                int computePower = ComputePower.Instance.value;

                cachedTickLine = $"{currentTick},{date},{climateValue.ToString("F4", CultureInfo.InvariantCulture)},{societalValue.ToString("F4", CultureInfo.InvariantCulture)},{trustValue.ToString("F4", CultureInfo.InvariantCulture)},{humanCount},{computePower},,";
            }

            // Escape description if it contains commas or quotes
            string escapedDescription = EscapeCSVField(eventDescription);

            // Update the cached line by replacing the empty event fields with actual data
            // Remove the last two commas (empty event fields) and add the event data
            if (cachedTickLine.EndsWith(",,"))
            {
                cachedTickLine = cachedTickLine.Substring(0, cachedTickLine.Length - 2);
                cachedTickLine += $",{eventType},{escapedDescription}";
            }

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
                // Write any remaining cached line before closing
                if (cachedTickLine != null)
                {
                    logWriter.WriteLine(cachedTickLine);
                    cachedTickLine = null;
                }

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
