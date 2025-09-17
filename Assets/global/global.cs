using System.Collections.Generic;

public enum THEME
{

}


public enum TileType
{
    None
}
public class LevelData
{
    public int levelNumber;
    public List<int[,]> layerGrid;
}
public enum Popup
{
    DASHBOARD,
    
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