using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using EVRC.Core;
using NUnit.Framework;
using UnityEngine;


public class ControlBindingsUtilsTests
{
    private string tempBindingsPath;


    [SetUp]
    public void Setup()
    {
        tempBindingsPath = Path.Combine(Application.temporaryCachePath, "temp.4.0.binds");
    }

    #region ------------- ParseControlBinding Tests ------------------

    [Test]
    public void ParseControlBinding_Returns_Controls()
    {
        //Arrange
        string xml = @"<LeftThrustButton>
		                <Primary Device=""Keyboard"" Key=""Key_Q"" />
		                <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Left"" />
	                </LeftThrustButton>";
        XElement control = XElement.Parse(xml);

        //Act
        var controlBinding = new ControlButtonBinding
        {
            Primary = EDControlBindingsUtils.ParseControlBinding(control, "Primary"),
            Secondary = EDControlBindingsUtils.ParseControlBinding(control, "Secondary")
        };

        //Assert
        Assert.NotNull(controlBinding.Primary);
        Assert.NotNull(controlBinding.Primary.Key); //literal keyboard key
        Assert.NotNull(controlBinding.Primary.Device); 
        Assert.NotNull(controlBinding.Secondary);
        Assert.NotNull(controlBinding.Secondary.Key); //literal keyboard key
        Assert.NotNull(controlBinding.Secondary.Device);

        Assert.AreEqual("Key_Q",controlBinding.Primary.Key);
        Assert.AreEqual("Joy_POV3Left", controlBinding.Secondary.Key);
    }
    
    [Test]
    public void ParseControlBinding_Returns_Modifiers()
    {
        //Arrange
        string xml = @"<LeftThrustButton>
		                <Primary Device=""Keyboard"" Key=""Key_N"">
			                <Modifier Device=""Keyboard"" Key=""Key_RightAlt"" />
			                <Modifier Device=""Keyboard"" Key=""Key_LeftControl"" />
		                </Primary>
		                <Secondary Device=""Keyboard"" Key=""Key_S"">
			                <Modifier Device=""Keyboard"" Key=""Key_LeftAlt"" />
		                </Secondary>
	                </LeftThrustButton>";
        XElement control = XElement.Parse(xml);

        var expectedModifierPrimaryOne = new ControlButtonBinding.KeyModifier()
        {
            Key = "Key_RightAlt",
            Device = "Keyboard"
        };
        var expectedModifierPrimaryTwo = new ControlButtonBinding.KeyModifier()
        {
            Key = "Key_LeftControl",
            Device = "Keyboard"
        };
        var expectedModifierSecondaryOne = new ControlButtonBinding.KeyModifier()
        {
            Key = "Key_LeftAlt",
            Device = "Keyboard"
        };


        //Act
        var controlBinding = new ControlButtonBinding
        {
            Primary = EDControlBindingsUtils.ParseControlBinding(control, "Primary"),
            Secondary = EDControlBindingsUtils.ParseControlBinding(control, "Secondary")
        };

        //Assert
        Assert.NotNull(controlBinding.Primary);
        Assert.AreEqual(2, controlBinding.Primary.Modifiers.Count);
        Assert.That(controlBinding.Primary.Modifiers.Contains(expectedModifierPrimaryOne));
        Assert.That(controlBinding.Primary.Modifiers.Contains(expectedModifierPrimaryTwo));

        Assert.NotNull(controlBinding.Secondary);
        Assert.AreEqual(1, controlBinding.Secondary.Modifiers.Count);
        Assert.That(controlBinding.Secondary.Modifiers.Contains(expectedModifierSecondaryOne));

    }

    #endregion


    #region ------------- ParseFile Tests ------------------

    [Test]
    public void Invalid_Path_Throws_Exception()
    {
        string invalidPath = Path.Combine(Paths.OverlayStatePath,"docthatdoesntexist.4.0.binds");

        void TestDelegate()
        {
            EDControlBindingsUtils.ParseFile(invalidPath);
        }
        Assert.Throws<FileNotFoundException>(TestDelegate);
    }

    [Test]
    public void ParseFile_Returns_ValidEntries()
    {
        // Arrange - write some valid examples to a temp file
            // one entry has primary value only
            // one has secondary value only
            // one has both
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
                    <Root PresetName=""Custom"" MajorVersion=""4"" MinorVersion=""0"">
	                    <KeyboardLayout>en-US</KeyboardLayout>
	                    <YawLeftButton>
		                    <Primary Device=""Keyboard"" Key=""Key_A"" />
		                    <Secondary Device=""{NoDevice}"" Key="""" />
	                    </YawLeftButton>
	                    <YawRightButton>
		                    <Primary Device=""{NoDevice}"" Key="""" />
		                    <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Right"" />
	                    </YawRightButton>
                        <LeftThrustButton>
		                    <Primary Device=""Keyboard"" Key=""Key_Q"" />
		                    <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Left"" />
	                    </LeftThrustButton>
                    </Root>";
        File.WriteAllText(tempBindingsPath, xml);

        // Act 
        Dictionary<EDControlButton, ControlButtonBinding> readBindings 
            = EDControlBindingsUtils.ParseFile(tempBindingsPath);

        // Assert
        Assert.AreEqual(3,readBindings.Count);
    }

    [Test]
    public void ParseFile_Skips_Undefined_EDControlButton()
    {
        // Arrange - write some valid examples to a temp file
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
                    <Root PresetName=""Custom"" MajorVersion=""4"" MinorVersion=""0"">
	                    <KeyboardLayout>en-US</KeyboardLayout>
	                    <YawLeftButton>
		                    <Primary Device=""Keyboard"" Key=""Key_A"" />
		                    <Secondary Device=""{NoDevice}"" Key="""" />
	                    </YawLeftButton>
	                    <YawRightButton>
		                    <Primary Device=""Keyboard"" Key=""Key_D"" />
		                    <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Right"" />
	                    </YawRightButton>
                        <BUTTONTHATISNTREAL>
		                    <Primary Device=""Keyboard"" Key=""Key_D"" />
		                    <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Right"" />
	                    </BUTTONTHATISNTREAL>
                    </Root>";
        File.WriteAllText(tempBindingsPath, xml);

        // Act 
        Dictionary<EDControlButton, ControlButtonBinding> readBindings
            = EDControlBindingsUtils.ParseFile(tempBindingsPath);

        // Assert
        Assert.AreEqual(2, readBindings.Count); // only 2 of the 3 controls have matching EDControlButtons
    }



    #endregion


    // GetAttributeValue Tests
}
