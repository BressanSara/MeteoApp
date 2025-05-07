using System.Reflection;
using Microsoft.Maui.ApplicationModel;

namespace MeteoApp.Services;

public class AppInfoService
{
    public string Version => AppInfo.VersionString;
    public string BuildNumber => AppInfo.BuildString;
    public string AppName => AppInfo.Name;
    public string LastBuildDate => File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("dd/MM/yyyy HH:mm");
} 