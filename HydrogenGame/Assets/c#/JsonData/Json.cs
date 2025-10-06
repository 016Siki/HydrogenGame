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

[System.Serializable]
public class UserObj
{
    public int id;
    public string name;
    public string email;
}

[System.Serializable]
public class RegisterResponse
{
    public string token;
    public UserObj user;
}
