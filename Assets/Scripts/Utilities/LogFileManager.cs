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

        // Subscribe to gauge change events
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
        // Unsubscribe from event
        if (Timeline.Instance != null)
        {
            GaugeManager.Instance.OnGaugeChanged -= OnGaugeChanged;
        }

        CloseLogFile();
    }

    private void CreateLogFile()
    {
        try
        {
            string logFolderPath = Path.Combine(Application.dataPath, "..", LOG_FOLDER);
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logFilePath = Path.Combine(logFolderPath, $"GameLog_{timestamp}.csv");

            logWriter = new StreamWriter(logFilePath, false, Encoding.UTF8)
            {
                AutoFlush = true
            };

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
            logWriter.WriteLine($"# Human_PopulationGrowthPerYear: {humanParameter.PopulationGrowthPerYear.ToString(CultureInfo.InvariantCulture)}");
            logWriter.WriteLine($"# Human_GaugeImpactPerHuman: {Human.Instance.HumanCount.ToString(CultureInfo.InvariantCulture)}");
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
            logWriter.WriteLine("Tick,Date,ClimateGauge,SocietalGauge,HumanCount,ComputePower,PromptType,PromptDescription");
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

            // Cache the current tick data (without event data initially)
            cachedTickLine = BuildTickLine(currentTick);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to log tick data: {e.Message}");
        }
    }

    private string BuildTickLine(uint tick)
    {
        string date = Timeline.Instance.currentDate.ToString("yyyy-MM-dd");
        float climateValue = GaugeManager.Instance.ClimateGauge.value;
        float societalValue = GaugeManager.Instance.SocietalGauge.value;
        long humanCount = Human.Instance.HumanCount;
        int computePower = ComputePower.Instance.value;

        // Use InvariantCulture to ensure decimal separator is always a period (not comma)
        return $"{tick},{date},{climateValue.ToString("F4", CultureInfo.InvariantCulture)},{societalValue.ToString("F4", CultureInfo.InvariantCulture)},{humanCount},{computePower},,";
    }

    /// <summary>
    /// Logs a user action prompt at the current tick
    /// </summary>
    /// <param name="eventType">Type of prompt: "Upgrade", "Prompt", or "Warning"</param>
    /// <param name="eventDescription">Description of the prompt</param>
    public void LogUserAction(string eventType, string eventDescription)
    {
        if (logWriter == null) return;

        // Validate prompt type
        if (eventType != "Upgrade" && eventType != "Prompt" && eventType != "Warning")
        {
            Debug.LogWarning($"Invalid prompt type: {eventType}. Expected: Upgrade, Prompt, or Warning");
        }

        try
        {
            // If no cached line exists yet, create one for the current tick
            if (cachedTickLine == null)
            {
                cachedTickLine = BuildTickLine(Timeline.Instance.CurrentTick);
            }

            // Escape description if it contains commas or quotes
            string escapedDescription = EscapeCSVField(eventDescription);

            // Update the cached line by replacing the empty prompt fields with actual data
            if (cachedTickLine.EndsWith(",,"))
            {
                cachedTickLine = cachedTickLine.Substring(0, cachedTickLine.Length - 2) + $",{eventType},{escapedDescription}";
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
