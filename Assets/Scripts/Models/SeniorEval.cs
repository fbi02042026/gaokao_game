using System;
using System.Collections.Generic;

[Serializable]
public class SeniorEval
{
    public string evalId;
    public string majorId;
    public ReviewerInfo reviewer;
    public string content;
    public string[] tags;
    public bool adLocked;
    public int priority;
}

[Serializable]
public class ReviewerInfo
{
    public string identity;
    public string collegeType;
    public string yearsAfterGraduation;
    public string currentStatus;
    public string npcType;
}

[Serializable]
public class SeniorEvalData
{
    public List<SeniorEval> evals;
}