using StreamJsonRpc;

namespace DmhyAutoDownload.AriaRPC;

internal class AriaMethodNameTransforms
{
    internal static Func<string, string> ScopedMethod(string scope)
    {
        return name => CommonMethodNameTransforms.Prepend(scope + ".").Invoke(CommonMethodNameTransforms.CamelCase(name.Replace("Async", "")));
    }
}