﻿using System.IO;

internal static class Constants
{
    public static readonly string FolderPath = Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
        "EvilPenguinIndustries","Spacelancer"
    );
}
