using System;

[Serializable]


// This class is used to store the session information of the user
public class Session 

{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string MacAddress { get; set; }
}