using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;

namespace Server.Tests;

public class ConfigTests
{
    private string _tempDir;
    private string _configDir;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _configDir = Path.Combine(_tempDir, "Config");
        Directory.CreateDirectory(_configDir);

        // Redirect Core base directory to the temporary path
        var coreBase = typeof(Core).GetField("_BaseDirectory", BindingFlags.Static | BindingFlags.NonPublic);
        coreBase?.SetValue(null, _tempDir);
        var exePath = typeof(Core).GetField("_ExePath", BindingFlags.Static | BindingFlags.NonPublic);
        exePath?.SetValue(null, Path.Combine(_tempDir, "ServUO.exe"));

        // Reset Config static state
        var initialized = typeof(Config).GetField("_Initialized", BindingFlags.Static | BindingFlags.NonPublic);
        initialized?.SetValue(null, false);
        var entries = typeof(Config).GetField("_Entries", BindingFlags.Static | BindingFlags.NonPublic);
        (entries?.GetValue(null) as IDictionary)?.Clear();
        var warned = typeof(Config).GetField("_Warned", BindingFlags.Static | BindingFlags.NonPublic);
        (warned?.GetValue(null) as HashSet<string>)?.Clear();
        var pathField = typeof(Config).GetField("_Path", BindingFlags.Static | BindingFlags.NonPublic);
        pathField?.SetValue(null, Path.Combine(_tempDir, "Config"));
    }

    [TearDown]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Test]
    public void LoadConfig_ReadsValues()
    {
        File.WriteAllLines(Path.Combine(_configDir, "test.cfg"), new[]
        {
            "IntValue=42",
            "StringValue=hello"
        });

        var intValue = Config.Get("test.IntValue", 0);
        var stringValue = Config.Get("test.StringValue", "");

        Assert.That(intValue, Is.EqualTo(42));
        Assert.That(stringValue, Is.EqualTo("hello"));
    }

    [Test]
    public void LoadConfig_IgnoresCommentsAndUsesDefault()
    {
        File.WriteAllLines(Path.Combine(_configDir, "sample.cfg"), new[]
        {
            "# comment line",
            "BoolValue=true",
            "@DefaultValue=100"
        });

        var boolValue = Config.Get("sample.BoolValue", false);
        var defaultValue = Config.Get("sample.DefaultValue", 55);

        Assert.That(boolValue, Is.True);
        Assert.That(defaultValue, Is.EqualTo(55));
    }

    [Test]
    public void LoadConfig_InvalidValue_UsesDefault()
    {
        File.WriteAllLines(Path.Combine(_configDir, "invalid.cfg"), new[]
        {
            "Number=NaN"
        });

        var number = Config.Get("invalid.Number", 5);

        Assert.That(number, Is.EqualTo(5));
    }

    [Test]
    public void LoadConfig_DuplicateEntries_LastWins()
    {
        File.WriteAllLines(Path.Combine(_configDir, "duplicate.cfg"), new[]
        {
            "Value=1",
            "Value=2"
        });

        var result = Config.Get("duplicate.Value", 0);

        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void LoadConfig_NestedDirectory_ParsesScope()
    {
        var nestedDir = Path.Combine(_configDir, "Feature");
        Directory.CreateDirectory(nestedDir);

        File.WriteAllLines(Path.Combine(nestedDir, "nested.cfg"), new[]
        {
            "NestedValue=7"
        });

        var result = Config.Get("Feature.nested.NestedValue", 0);

        Assert.That(result, Is.EqualTo(7));
    }
}

