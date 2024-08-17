using System;

[Serializable]
public class Session 

{

    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string MacAddress { get; set; }
}