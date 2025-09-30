using System.Collections.Generic;

public enum THEME
{
    Green,
    WoodOld,
    OrangeNew,

    WoodNew,

    Violet,
}



public enum TileType
{
    None
}

public enum THEME_STYLE
{
    Classic,
    Flower,
    Simple,
    Antique,
    Poker,
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
    SHOP,
    Reward,
    LANTERN_INTRO,
}

public enum BACKGROUND
{
    BG1,
    BG2,
    BG3,
    BG4,
    BG5,
    BG6,
    TASK,

}


public enum Page
{
    DASHBOARD,
    PLAY,
    LANTERN_CHALLENGE,
    DAILY_CHALLENGE,
    BADGES,
}
public class Missiondata
{
    public string name;
    public int misionCount;
    public int remain;
    public bool done;

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
    public static string ToFolderPath(this THEME theme)
    {
        switch (theme)
        {
            case THEME.Green: return "Green";
            case THEME.WoodOld: return "Wood/old";
            case THEME.WoodNew: return "Wood/new";
            case THEME.OrangeNew: return "Orange/new";
            case THEME.Violet: return "Violet";
            default: return "Green";
        }
    }
    public static string GetSprite(int type, THEME theme)
    {
        return ToFolderPath(theme) + "/" + type.ToString();
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



public enum SpecialTile
{
    FLOWER = -1,
    SEASON = -2
}

