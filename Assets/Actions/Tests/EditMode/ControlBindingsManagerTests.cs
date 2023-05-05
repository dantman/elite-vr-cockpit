using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using EVRC.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ControlBindingsManagerTests
{
    private GameObject parentGameObject;
    private ControlBindingsManager controlBindingsManager;
    private ControlBindingsState controlBindingsState;
    private GameEvent bindingsChangedEvent;

    private List<string> temporaryFiles = new List<string>();

    [SetUp]
    public void Setup()
    {
        parentGameObject = new GameObject("Test Control Binding Manager");
        controlBindingsManager = parentGameObject.AddComponent<ControlBindingsManager>();
        controlBindingsState = ScriptableObject.CreateInstance<ControlBindingsState>();
        bindingsChangedEvent = ScriptableObject.CreateInstance<GameEvent>();

        controlBindingsManager.controlBindingsState = controlBindingsState;
        controlBindingsManager.controlBindingsState.gameEvent = bindingsChangedEvent;
    }

    [Test]
    public void LoadBindings_Updates_ScriptableObject()
    {
        // Arrange
        var startCount = controlBindingsState.buttonBindings.Count;

        // Act
        controlBindingsManager.LoadControlBindings();

        // Assert
        Assert.AreNotEqual(startCount, controlBindingsState.buttonBindings.Count);
    }

    [Test]
    public void BindingsWatcherTestsWithEnumeratorPasses()
    {
        // Arrange
        // Start watching (through reload method, which calls the private Watch method)
        controlBindingsManager.Reload();

        // read a copy directly from XML (easier to edit)
        string bindingsPath = Paths.ControlBindingsPath;
        temporaryFiles.Add(bindingsPath);
        XDocument doc = XDocument.Load(bindingsPath);

        // Find the YawLeftButton element
        XElement yawLeftButton = doc.Descendants("YawLeftButton").FirstOrDefault();

        // If the YawLeftButton element exists, update the Primary element
        if (yawLeftButton != null)
        {
            XElement primary = yawLeftButton.Descendants("Primary").FirstOrDefault();
            if (primary != null)
            {
                // Update the Key attribute
                primary.Attribute("Key")!.Value = "Key_W";
            }
        }

        // Act
        doc.Save(bindingsPath); // should kick off a re-read of the bindings
        // yield return null;

        bool eventInvoked = false;
        bindingsChangedEvent.Event += () => eventInvoked = true;

        Assert.That(
            () => eventInvoked,
            Is.True.After(5000),
            "BindingsChanged event was not invoked within 5 seconds after file change.");
    }

    [TearDown]
    public void TearDown()
    {
        foreach (string file in temporaryFiles)
        {
            File.Delete(file);
        }
    }
}
