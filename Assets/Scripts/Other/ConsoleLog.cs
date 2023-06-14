#if (UNITY_EDITOR)
using System.Reflection;

/// <summary>
/// Console Log class.
/// </summary>
public static class ConsoleLog
{
    /// <summary>
    /// Clears the editor console log.
    /// </summary>
    public static void Clear()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
#endif