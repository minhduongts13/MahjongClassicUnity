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
    WIN,
    LANGUAGES,
    SHOP
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

public static class Point
{
    static public int basePoint = 200;
    static public int comboStepPoint = 40;
    static public int comboPointEnd = 10;
    static public int bonusPoint = 80;
    static public int bonusStart = 10;
    static public int bonusStep = 5;
    static public int bonusMaxStart = 20;
    static public int bonusMax = 120;
}

