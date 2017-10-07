using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

public class SettingsHelper
{
    public SettingsHelper()
    {
    }

    public static DatabaseConnectionSettings ReadSettings(string path)
    {
        DatabaseConnectionSettings settings;
        string json = String.Empty;

        if (!File.Exists(path))
        {
            WriteSettings(path, new DatabaseConnectionSettings() { DatabaseName = "Test", ServerName = "Server", UserName = "UsrName_Here", Password = "SecurePasswordHere" });
        }

        json = File.ReadAllText(path);
        settings = new JavaScriptSerializer().Deserialize<DatabaseConnectionSettings>(json);

        return settings;
    }
    public static void WriteSettings(string path, DatabaseConnectionSettings settings)
    {
        string json = new JavaScriptSerializer().Serialize(settings);
        File.WriteAllText(path, JsonHelper.FormatJson(json));
    }
}