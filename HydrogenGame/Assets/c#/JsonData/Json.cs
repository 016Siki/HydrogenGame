using System;
using System.Collections.Generic;

[System.Serializable]
public class RankingData
{
    public int rank;
    public string name;
    public int score;
}

[System.Serializable]
public class RankingList
{
    public List<RankingData> rankings;
}
