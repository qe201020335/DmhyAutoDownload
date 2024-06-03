namespace DmhyAutoDownload.AriaRPC.Models.Results;

public class GetVersionResult: Result
{
    public string[] enabledFeatures;

    public string version;
}