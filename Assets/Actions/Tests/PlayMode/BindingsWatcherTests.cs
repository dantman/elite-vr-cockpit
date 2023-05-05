using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using EVRC.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class BindingsWatcherTests
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

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator BindingsWatcher_Reloads_Bindings_AfterChange()
    {
        // Arrange
        // Create a copy of the current bindings file for safe testing
        string bindingsPath = Path.Combine(Application.temporaryCachePath, Path.GetFileNameWithoutExtension(Paths.ControlBindingsPath) + ".Editor.binds");
        if (!File.Exists(bindingsPath))
        {
            File.Copy(Paths.ControlBindingsPath,bindingsPath, true);
        }

        // Set the bindings file path and start watching (through reload method, which calls the private Watch method)
        controlBindingsManager.SetBindingsFile(bindingsPath);
        controlBindingsManager.Reload();

        // Read it directly from XML (so we can edit the content for testing)
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
        yield return new WaitForSeconds(5.0f);

        // Assert - State should be updated after re-reading the file
        var yawLeftPrimaryKey = controlBindingsState.buttonBindings[EDControlButton.YawLeftButton].Primary.Key;
        Assert.AreEqual("Key_W", yawLeftPrimaryKey);

        yield return null;
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
