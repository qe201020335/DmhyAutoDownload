namespace DmhyAutoDownload.AriaRPC;

public interface IAria2System 
{
    Task<string[]> ListMethodsAsync();
    
    Task<string[]> ListNotificationsAsync();
}