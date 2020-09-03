using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class ConfigReader
{
    private readonly Dictionary<string, string> _configs = new Dictionary<string, string>();
    private readonly string _fileName;

    public ConfigReader(string fileName)
    {
        _fileName = fileName;
        try
        {
            string[] lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (!line.StartsWith("#") && line.Trim().Length > 0)
                {
                    string[] lineSplit = line.Split('=');
                    if (lineSplit.Length > 1)
                    {
                        _configs.Add(lineSplit[0].Trim(), string.Join("=", lineSplit.Skip(1).ToArray()).Trim());
                    }
                }
            }
        }
        catch (Exception e)
        {
            LogManager.Log("Could not load " + fileName + ". " + e.Message);
        }
    }

    public string GetString(string config, string defaultValue)
    {
        if (!_configs.ContainsKey(config))
        {
            LogManager.Log("Missing config " + config + " from file " + _fileName + ". Default value " + defaultValue + " will be used instead.");
            return defaultValue;
        }
        return _configs[config];
    }

    public bool GetBool(string config, bool defaultValue)
    {
        if (!_configs.ContainsKey(config))
        {
            LogManager.Log("Missing config " + config + " from file " + _fileName + ". Default value " + defaultValue + " will be used instead.");
            return defaultValue;
        }

        try
        {
            return bool.Parse(_configs[config]);
        }
        catch (Exception)
        {
            LogManager.Log("Config " + config + " from file " + _fileName + " should be Boolean. Using default value " + defaultValue + " instead.");
            return defaultValue;
        }
    }

    public int GetInt(string config, int defaultValue)
    {
        if (!_configs.ContainsKey(config))
        {
            LogManager.Log("Missing config " + config + " from file " + _fileName + ". Default value " + defaultValue + " will be used instead.");
            return defaultValue;
        }

        try
        {
            return int.Parse(_configs[config]);
        }
        catch (Exception)
        {
            LogManager.Log("Config " + config + " from file " + _fileName + " should be Integer. Using default value " + defaultValue + " instead.");
            return defaultValue;
        }
    }

    public long GetLong(string config, long defaultValue)
    {
        if (!_configs.ContainsKey(config))
        {
            LogManager.Log("Missing config " + config + " from file " + _fileName + ". Default value " + defaultValue + " will be used instead.");
            return defaultValue;
        }

        try
        {
            return long.Parse(_configs[config]);
        }
        catch (Exception)
        {
            LogManager.Log("Config " + config + " from file " + _fileName + " should be Long. Using default value " + defaultValue + " instead.");
            return defaultValue;
        }
    }

    public float GetFloat(string config, float defaultValue)
    {
        if (!_configs.ContainsKey(config))
        {
            LogManager.Log("Missing config " + config + " from file " + _fileName + ". Default value " + defaultValue + " will be used instead.");
            return defaultValue;
        }

        try
        {
            return float.Parse(_configs[config], CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            LogManager.Log("Config " + config + " from file " + _fileName + " should be Float. Using default value " + defaultValue + " instead.");
            return defaultValue;
        }
    }

    public double GetDouble(string config, double defaultValue)
    {
        if (!_configs.ContainsKey(config))
        {
            LogManager.Log("Missing config " + config + " from file " + _fileName + ". Default value " + defaultValue + " will be used instead.");
            return defaultValue;
        }

        try
        {
            return double.Parse(_configs[config], CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            LogManager.Log("Config " + config + " from file " + _fileName + " should be Double. Using default value " + defaultValue + " instead.");
            return defaultValue;
        }
    }
}
