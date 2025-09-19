using System.Collections.Generic;

public enum THEME
{
    Green
}


public enum TileType
{
    None
}

public enum THEME_STYLE
{
    Green,
    Orange,
    Violet,
    Wood
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


public enum Button
{
    OFF,
    ON
}







public static class global
{
    public static string tilePath = "theme/tiles";
    public static string backgroundPath = "theme/background";
    public static string GetSprite(int type, THEME theme)
    {
        return theme.ToString() + "/" + type.ToString();
    }
}

