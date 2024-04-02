using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class DebugLogger
{

	private const string _defaultClassName = "Log";
	private const string CONDITIONAL_NAME = "DEV_BUILD";
	private static readonly Color DefaultLogColor = Color.white;
	private static readonly Color DefaultWarningColor = new Color(0.73f, 0.72f, 0.2f);
	private static readonly Color DefaultErrorColor = new Color(0.9f, 0.27f, 0.2f);

	#region Messages
	[Conditional(CONDITIONAL_NAME)]
	public static void Log(string message, object myClass, Color color = default)
	{
		if (color == default)
		{
			color = DefaultLogColor;
		}
		string className = _defaultClassName;
		if (myClass != null)
		{
			className = myClass.GetType().ToString();
		}

		if (CustomColoringReplacer(ref message, color))
		{
			Debug.Log($"{GetFormattedClassName(className)}{message}");
			return;
		}

		Debug.Log($"{GetFormattedClassName(className)}{GetColorStartString(color)}{message}{GetColorEndString()}");
	}

	public static void Log(string message, object myClass, Color body, Color highlight)
	{
		if (body == default)
		{
			body = DefaultLogColor;
		}
		string className = _defaultClassName;
		if (myClass != null)
		{
			className = myClass.GetType().ToString();
		}

		CustomColoringReplacer(ref message, highlight);
		Debug.Log($"{GetFormattedClassName(className)}{GetColorStartString(body)}{message}{GetColorEndString()}");
	}

	[Conditional(CONDITIONAL_NAME)]
	public static void Log(string message, Type type, Color color = default)
	{
		if (color == default)
		{
			color = DefaultLogColor;
		}
		string className = _defaultClassName;
		if (type != null)
		{
			className = type.ToString();
		}

		if (CustomColoringReplacer(ref message, color))
		{
			Debug.Log($"{GetFormattedClassName(className)}{message}");
			return;
		}

		Debug.Log($"{GetFormattedClassName(className)}{GetColorStartString(color)}{message}{GetColorEndString()}");
	}

	[Conditional(CONDITIONAL_NAME)]
	public static void Log(string message, Color color = default)
	{
		if (color == default)
		{
			color = DefaultLogColor;
		}

		if (CustomColoringReplacer(ref message, color))
		{
			Debug.Log($"{GetFormattedClassName(_defaultClassName)}{message}");
			return;
		}

		Debug.Log($"{GetFormattedClassName(_defaultClassName)}{GetColorStartString(color)}{message}{GetColorEndString()}");
	}

	[Conditional(CONDITIONAL_NAME)]
	public static void LogIntList(object myClass, string message, List<int> cardIds, Color color = default)
	{
		string className = _defaultClassName;
		if (myClass != null)
		{
			className = myClass.GetType().ToString();
		}
		if (color == default)
		{
			color = new Color(0.48f, 0.67f, 0.72f);
		}

		string debugString = "[";
		for (int i = 0; i < cardIds.Count; i++)
		{
			debugString += $"{cardIds[i]}";
			if (i != cardIds.Count - 1)
			{
				debugString += ", ";
			}
		}
		debugString += "]";
		Debug.Log($"{GetFormattedClassName(className)}<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}{debugString}</color>");
	}

	[Conditional(CONDITIONAL_NAME)]
	public static void LogVector3List(object myClass, string message, List<Vector3> vector3s, Color color = default)
	{
		string className = _defaultClassName;
		if (myClass != null)
		{
			className = myClass.GetType().ToString();
		}
		if (color == default)
		{
			color = new Color(0.48f, 0.67f, 0.72f);
		}

		string debugString = "[";
		for (int i = 0; i < vector3s.Count; i++)
		{
			debugString += $"{vector3s[i]}";
			if (i != vector3s.Count - 1)
			{
				debugString += ", ";
			}
		}
		debugString += "]";
		Debug.Log($"{GetFormattedClassName(className)}<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}{debugString}</color>");
	}

	[Conditional(CONDITIONAL_NAME)]
	public static void LogDeadwood(string message, object myClass, Color color = default)
	{
		if (color == default)
		{
			color = DefaultLogColor;
		}
		//Log(message, myClass, color);
	}
	#endregion

	#region Warnings
	[Conditional(CONDITIONAL_NAME)]
	public static void LogWarning(string message, object myClass, Color color = default)
	{
		if (color == default)
		{
			color = DefaultWarningColor;
		}
		string className = _defaultClassName;
		if (myClass != null)
		{
			className = myClass.GetType().ToString();
		}

		if (CustomColoringReplacer(ref message, color))
		{
			Debug.LogWarning($"{GetFormattedClassName(className)}{message}");
			return;
		}

		Debug.LogWarning($"{GetFormattedClassName(className)}{GetColorStartString(color)}{message}{GetColorEndString()}");
	}

	[Conditional(CONDITIONAL_NAME)]
	public static void LogWarning(string message, Color color = default)
	{
		if (color == default)
		{
			color = DefaultWarningColor;
		}

		if (CustomColoringReplacer(ref message, color))
		{
			Debug.LogWarning($"{GetFormattedClassName(_defaultClassName)}{message}");
			return;
		}

		Debug.LogWarning($"{GetFormattedClassName(_defaultClassName)}{GetColorStartString(color)}{message}{GetColorEndString()}");
	}
	#endregion

	#region Errors
	[Conditional(CONDITIONAL_NAME)]
	public static void LogError(string message, object myClass, Color color = default)
	{
		if (color == default)
		{
			color = DefaultErrorColor;
		}

		string className = _defaultClassName;
		if (myClass != null)
		{
			string[] split = myClass.GetType().ToString().Split('.');
			className = split[split.Length - 1];
		}

		if (CustomColoringReplacer(ref message, color))
		{
			Debug.LogError($"{GetFormattedClassName(className)}{message}");
			return;
		}

		Debug.LogError($"{GetColorStartString(color)}{message}{GetColorEndString()}");
	}

	[Conditional(CONDITIONAL_NAME)]
	public static void LogError(string message, Color color = default)
	{
		if (color == default)
		{
			color = DefaultErrorColor;
		}

		if (CustomColoringReplacer(ref message, color))
		{
			Debug.LogError($"{message}");
			return;
		}

		Debug.LogError($"{GetFormattedClassName(_defaultClassName)}{GetColorStartString(color)}{message}{GetColorEndString()}");
	}
	#endregion

	private static string GetFormattedClassName(string className)
	{
		string[] split = className.Split('.');
		className = split[split.Length - 1];
		return $"|{className}| ";
	}

	private static string GetColorStartString(Color color)
	{
		return Application.isEditor ? $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>" : "";
	}

	private static string GetColorEndString()
	{
		return Application.isEditor ? "</color>" : "";
	}

	private static bool CustomColoringReplacer(ref string message, Color color)
	{
		if (!message.Contains("<#")) return false;

		message = message.Replace("<#", GetColorStartString(color));
		message = message.Replace("#>", GetColorEndString());
		return true;

	}
}

