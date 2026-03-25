using System;
using System.Collections.Generic;

/// <summary>
/// Classes base para recebimento dos dados
/// </summary>
[Serializable]
public class TrafficResponse
{
    public Status current_status;
    public List<PredictedStatus> predicted_status;
}

[Serializable]
public class PredictedStatus
{
    public int estimated_time;
    public Status predictions;
}

[Serializable]
public class Status
{
    public float vehicleDensity;
    public float averageSpeed;
    public string weather;
}
