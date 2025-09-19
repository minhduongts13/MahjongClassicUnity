using System.Collections.Generic;

public enum THEME
{

}
public class LevelData
{
    public int levelNumber;
    public List<int[,]> layerGrid;
}
public enum Popup
{
    SETTINGS,
    THEME,
    DAILY,
    TASK,   

}


public enum Page
{
    DASHBOARD,
    PLAY
}
public class Missiondata
{
    public string name;
    public int misionCount;
}
