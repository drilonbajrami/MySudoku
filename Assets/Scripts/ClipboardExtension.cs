using UnityEngine;

/// <summary>
/// Clipboard extension class.
/// </summary>
public static class ClipboardExtension
{
    /// <summary>
    /// Copies this string into the Clipboard.
    /// </summary>
    public static void CopyToClipboard(this string str) => GUIUtility.systemCopyBuffer = str;
}