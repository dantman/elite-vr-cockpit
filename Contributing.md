# Contributing to Elite VR Cockpit
Thank you for your interest in contributing to Elite VR Cockpit! We welcome contributions from the community to make this project better. Here are some guidelines to get you started:

## How to Contribute
0. Check out the [Project Notes](#specific-notes-about-this-project) and [2023 Overhaul Notes](#2023-overhaul), so you understand the general setup of this project (work-in-progress)
1. Fork the repository and clone it to your local machine.
2. Create a new branch for your changes: `git checkout -b feature/your-feature-name`
3. Make your changes and ensure that your code follows the project's coding conventions and style guidelines.
4. Write unit tests if required - See [Unit Test Conventions](#unit-test-conventions) for more details
5. Commit your changes: `git commit -m "Add your commit message here"`
6. Push your changes to your forked repository: `git push origin feature/your-feature-name`
7. Create a pull request from your branch to the `main` branch of the original repository.

## Code Style Guidelines
- Follow the [Unity C# Coding Conventions](https://learn.unity.com/tutorial/coding-standards-and-best-practices#5c7f8528edbc2a002053b67a) for all code contributions.
- Use meaningful variable and function names.
- Add comments to explain complex code sections or to provide additional context.

## Bug Reports
If you encounter a bug while using Elite VR Cockpit, please submit a detailed bug report by creating a new issue. Include the steps to reproduce the bug, your platform, Unity version, and any relevant logs or error messages.

## Feature Requests
If you have an idea for a new feature or improvement, we'd love to hear about it! Please open a new issue with a clear description of the feature request, along with any additional context or use cases.

        **Note about feature requests**: In general, we view the EVRC overlay as a tool to help the user interact with the cockpit. We do not want to incorporate features that work with data, interact with servers, or other types of inputs.

        This isn't intended to replace other tools, it is intended to display the inputs/outputs for the core game. However, we may consider features that interact with other tools, if those developers are willing. For example, if there was a tool that could search for profitable trades: we would consider allowing that tool to output information to a JSON file that could be read by the overlay, but we would **not** integrate code that actually searches for the profitable trades.

## Code Review
All pull requests will undergo a code review process. Be open to feedback and suggestions from the maintainers and other contributors. We want to ensure that the code is maintainable, adheres to coding standards, and aligns with the project's goals.

## License
By contributing to Elite VR Cockpit, you agree that your contributions will be licensed under the same license as the project. If you're using any third-party assets or code in your contributions, ensure they are compatible with the project's [License](/LICENSE.md).

## Questions and Support
If you have any questions or need support while contributing, feel free to ask in the project's issue tracker or discussions.

## Specific Notes About This Project
1. Base ScriptableObjects Types
    - [GameEvents & Listeners](Assets/GameEventSystem/ReadMe_GameEvents.md)
2. SteamVR "Standalone Patch"
    - hacked by Dantman in 2019 to make SteamVR run as an overlay (the base of this whole project)
    - [Notes for Upgrading Steam VR](Assets/Scripts/SteamVR_Upgrade_Notes.md)


# Unit Test Conventions
Unit tests are technically compatible with Unity, but in practice, they can be difficult to implement and/or add little value for the effort.
Check out this site for some basics in unit testing: [Unity Unit Testing](https://unity.com/how-to/unity-test-framework-video-game-development)

This is an _incomplete_ list of situations where we've found unit tests to be helpful and easily compatible:
1. When the method or class does not require the VR overlay to function (e.g. Reading from the Status.json file).
2. When a method needs to programatically generate a component. Yes, you can still test components and prefabs without the VR overlay (see CockpitAnchors for an example)
3. When interacting with files


# 2023 Overhaul
As of August, 2023, this project has been rebuilt and published under this fork (/boyestrous) to include some new coding conventions that are intended to make future updates more straightforward.

It was a lot of de-coupling code by using an event-based system. I didn't reinvent the wheel and the work isn't really done...but there have been significant performance improvements from what's been done so far!

**General conventions that were used**:
1. Modular
    a) Systems are not directly dependent on each other
        - ScriptableObjects should be referenced to share data between systems, even if they are only used once
    b) Prefabs are self-contained
        - All logic should be within the prefab (as components)
        - GameEvents (a special type of ScriptableObject) can be used to simulate things that happen outside the prefab
    c) Components should only do one thing (where possible)
2. Editable
    a) Change the game without code, where possible
        - ScriptableObject references can be changed in the editor, **do not hard-code variables**
3. Debuggable
    a) Test in isolation (see modularity)
        - *Ideally*... prefabs can be loaded into a scene with nothing else and work as expected
        - Build test settings directly into prefabs or components via custom editor scripts (ex: if we need to simulate a log message, put a button on the component in the editor to make a log message)
    b) Unit Tests
        - *Ideally*...All new code has a corresponding unit test, this is tricky in unity, but I've been able to make significant headway in unit testing the Status file and other non-visual things

**Not Done Yet**
1. I created separate namespaces, but haven't yet moved all of them into separate assemblies. The main ones are: Core, Desktop, and Overlay, but right now they all _technically_ reside within the Core namespace. There's a lot more decoupling to be done before it's feasible to fully separate them.
2. Not everything uses the GameEvent system, there are still lots of direct references to other class instances

