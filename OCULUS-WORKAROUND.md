# Oculus Workaround

Many users try to use Elite VR Cockpit on their Oculus Rift, only to find it won't work. Here is an explanation as to why it doesn't work, and how to fix it.

**This fix _probably_ only works with the Steam version of Elite Dangerous**

## Why doesn't it work?

Elite VR Cockpit doesn't utilize the Oculus SDK. Elite Dangerous, even when booted up through Steam/SteamVR, will insist on using the Oculus SDK when you are using an Oculus Rift.

Thankfully, there are certain ways to work around this.

## How do I fix it?

Because Oculus does not support Windows 7, setting the Elite Dangerous EXEs to run with Compatibility for Windows 7 will allow them to be booted up through SteamVR. Because they render to SteamVR instead of the Oculus SDK, you can see and interact with Elite VR Cockpit!

The basic idea is to set the EliteDangerous64.exe and EliteDangerous32.exe to run with Compatibility for Windows 7. That's it! Now when you boot up Elite Dangerous through Steam/SteamVR it should render to SteamVR instead of rendering directly to the Oculus SDK.

### Steps to set 64 bit version to compatibility mode:

1. Right click `Elite Dangerous` in your Steam Library
2. Click `Properties`
3. Click the `LOCAL FILES` tab
4. Click `Browse Local Files...`
5. Navigate to the `Products` folder
6. Navigate to `elite-dangerous-64`
7. Right click `EliteDangerous64.exe`
8. Click `Properties`
9. Click the `Compatibility` tab
10. Check the box next to `Run this program in compatibility mode for:`
11. Select `Windows 7` in the dropdown.
12. Click `Ok` to close the properties window.

Now the 64 bit exe will work with Elite VR Cockpit when launched through Steam in SteamVR mode. You should also make the 32 bit version compatible.

#### Steps to set 32 bit version to compatibility mode:

1. Navigate back to the `Products` folder
2. Navigate to `FORC-FDEV-D-1010`
3. Right click `EliteDangerous32.exe`
4. Click `Properties`
5. Click the `Compatibility` tab
6. Check the box next to `Run this program in compatibility mode for:`
7. Select `Windows 7` in the dropdown.
8. Click `Ok` to close the properties window.

Now both executables will work with Elite VR Cockpit when launched through Steam in SteamVR mode!

To confirm this is working, pop open the SteamVR Display Mirror. If you can see Elite Dangerous in the Display Mirror, it should be rendering through SteamVR! When you launch up Elite Dangerous normally, it does not appear in the Display Mirror, because it is rendering direct to headset.

> This documentation was coppied from OVRdrop's "[Rift & Elite Dangerous](https://github.com/Hotrian/OVRdrop-Public/wiki/Rift-&-Elite-Dangerous)" documentation with permission.
>
> Elite VR Cockpit's licence does not apply to this documentation page.
