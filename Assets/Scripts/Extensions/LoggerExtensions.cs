using UnityEngine;

public static class LoggerExtensions 
{
    public static string LogFormat<T>(this string body, T context, Color? headerColor = null)
    {
        return body.LogFormat(context.GetType().Name, headerColor);
    }
        
    public static string LogFormat(this string body, string context, Color? headerColor = null)
    {
#if !UNITY_EDITOR
        return $"[{context}] {body}"; 
#else
        Color defaultColor = headerColor ?? Color.black;
        return $"[<color=#{ColorUtility.ToHtmlStringRGBA(defaultColor)}>{context}</color>] {body}";
#endif
    }
}