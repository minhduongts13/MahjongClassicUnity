using System.Collections.Generic;

public enum THEME
{

}

public enum THEME_STYLE
{
    Green,
    Orange,
    Violet,
    Wood
}

public static class global
{
    public static string tilePath = "theme/tiles";
    public static string backgroundPath = "theme/background";
}
public class LevelData
    {
        public int levelNumber;
        public List<int[,]> layerGrid;
    }
public enum Popup
{
    SETTINGS,
    THEME

}

public enum Page
{
    DASHBOARD,
    PLAY
}

public enum Button
{
    OFF,
    ON
}