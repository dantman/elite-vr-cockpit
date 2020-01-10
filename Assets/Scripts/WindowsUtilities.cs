using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EVRC
{
    /**
     * Wrappers around general purpose Windows APIs
     */
    public static class WindowsUtilities
    {
        // HRESULT constants
        public static readonly uint E_ACCESSDENIED = 0x80070005; // Access denied
        public static readonly uint E_FAIL = 0x80004005; // Unspecified error
        public static readonly uint E_INVALIDARG = 0x80070057; // Invalid parameter value
        public static readonly uint E_OUTOFMEMORY = 0x8007000E; // Out of memory
        public static readonly uint E_POINTER = 0x80004003; // NULL was passed incorrectly for a pointer value
        public static readonly uint E_UNEXPECTED = 0x8000FFFF; // Unexpected condition
        public static readonly uint S_OK = 0x0; // Success
        public static readonly uint S_FALSE = 0x1; // Success

        public static class KnownFolderId
        {
            public static readonly Guid AccountPictures = new Guid("008ca0b1-55b4-4c56-b8a8-4de4b299d3be"); // Account Pictures
            public static readonly Guid AddNewPrograms = new Guid("de61d971-5ebc-4f02-a3a9-6c82895e5c04"); // Get Programs
            public static readonly Guid AdminTools = new Guid("724EF170-A42D-4FEF-9F26-B60E846FBA4F"); // Administrative Tools
            public static readonly Guid AppDataDesktop = new Guid("B2C5E279-7ADD-439F-B28C-C41FE1BBF672"); // AppDataDesktop
            public static readonly Guid AppDataDocuments = new Guid("7BE16610-1F7F-44AC-BFF0-83E15F2FFCA1"); // AppDataDocuments
            public static readonly Guid AppDataFavorites = new Guid("7CFBEFBC-DE1F-45AA-B843-A542AC536CC9"); // AppDataFavorites
            public static readonly Guid AppDataProgramData = new Guid("559D40A3-A036-40FA-AF61-84CB430A4D34"); // AppDataProgramData
            public static readonly Guid ApplicationShortcuts = new Guid("A3918781-E5F2-4890-B3D9-A7E54332328C"); // Application Shortcuts
            public static readonly Guid AppsFolder = new Guid("1e87508d-89c2-42f0-8a7e-645a0f50ca58"); // Applications
            public static readonly Guid AppUpdates = new Guid("a305ce99-f527-492b-8b1a-7e76fa98d6e4"); // Installed Updates
            public static readonly Guid CameraRoll = new Guid("AB5FB87B-7CE2-4F83-915D-550846C9537B"); // Camera Roll
            public static readonly Guid CDBurning = new Guid("9E52AB10-F80D-49DF-ACB8-4330F5687855"); // Temporary Burn Folder
            public static readonly Guid ChangeRemovePrograms = new Guid("df7266ac-9274-4867-8d55-3bd661de872d"); // Programs and Features
            public static readonly Guid CommonAdminTools = new Guid("D0384E7D-BAC3-4797-8F14-CBA229B392B5"); // Administrative Tools
            public static readonly Guid CommonOEMLinks = new Guid("C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D"); // OEM Links
            public static readonly Guid CommonPrograms = new Guid("0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8"); // Programs
            public static readonly Guid CommonStartMenu = new Guid("A4115719-D62E-491D-AA7C-E74B8BE3B067"); // Start Menu
            public static readonly Guid CommonStartup = new Guid("82A5EA35-D9CD-47C5-9629-E15D2F714E6E"); // Startup
            public static readonly Guid CommonTemplates = new Guid("B94237E7-57AC-4347-9151-B08C6C32D1F7"); // Templates
            public static readonly Guid ComputerFolder = new Guid("0AC0837C-BBF8-452A-850D-79D08E667CA7"); // Computer
            public static readonly Guid ConflictFolder = new Guid("4bfefb45-347d-4006-a5be-ac0cb0567192"); // Conflicts
            public static readonly Guid ConnectionsFolder = new Guid("6F0CD92B-2E97-45D1-88FF-B0D186B8DEDD"); // Network Connections
            public static readonly Guid Contacts = new Guid("56784854-C6CB-462b-8169-88E350ACB882"); // Contacts
            public static readonly Guid ControlPanelFolder = new Guid("82A74AEB-AEB4-465C-A014-D097EE346D63"); // Control Panel
            public static readonly Guid Cookies = new Guid("2B0F765D-C0E9-4171-908E-08A611B84FF6"); // Cookies
            public static readonly Guid Desktop = new Guid("B4BFCC3A-DB2C-424C-B029-7FE99A87C641"); // Desktop
            public static readonly Guid DeviceMetadataStore = new Guid("5CE4A5E9-E4EB-479D-B89F-130C02886155"); // DeviceMetadataStore
            public static readonly Guid Documents = new Guid("FDD39AD0-238F-46AF-ADB4-6C85480369C7"); // Documents
            public static readonly Guid DocumentsLibrary = new Guid("7B0DB17D-9CD2-4A93-9733-46CC89022E7C"); // Documents
            public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B"); // Downloads
            public static readonly Guid Favorites = new Guid("1777F761-68AD-4D8A-87BD-30B759FA33DD"); // Favorites
            public static readonly Guid Fonts = new Guid("FD228CB7-AE11-4AE3-864C-16F3910AB8FE"); // Fonts
            public static readonly Guid Games = new Guid("CAC52C1A-B53D-4edc-92D7-6B2E8AC19434"); // Games (Deprecated)
            public static readonly Guid GameTasks = new Guid("054FAE61-4DD8-4787-80B6-090220C4B700"); // GameExplorer
            public static readonly Guid History = new Guid("D9DC8A3B-B784-432E-A781-5A1130A75963"); // History
            public static readonly Guid HomeGroup = new Guid("52528A6B-B9E3-4ADD-B60D-588C2DBA842D"); // Homegroup
            public static readonly Guid HomeGroupCurrentUser = new Guid("9B74B6A3-0DFD-4f11-9E78-5F7800F2E772"); // The user's username (%USERNAME%)
            public static readonly Guid ImplicitAppShortcuts = new Guid("BCB5256F-79F6-4CEE-B725-DC34E402FD46"); // ImplicitAppShortcuts
            public static readonly Guid InternetCache = new Guid("352481E8-33BE-4251-BA85-6007CAEDCF9D"); // Temporary Internet Files
            public static readonly Guid InternetFolder = new Guid("4D9F7874-4E0C-4904-967B-40B0D20C3E4B"); // The Internet
            public static readonly Guid Libraries = new Guid("1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE"); // Libraries
            public static readonly Guid Links = new Guid("bfb9d5e0-c6a9-404c-b2b2-ae6db6af4968"); // Links
            public static readonly Guid LocalAppData = new Guid("F1B32785-6FBA-4FCF-9D55-7B8E7F157091"); // Local
            public static readonly Guid LocalAppDataLow = new Guid("A520A1A4-1780-4FF6-BD18-167343C5AF16"); // LocalLow
            public static readonly Guid LocalizedResourcesDir = new Guid("2A00375E-224C-49DE-B8D1-440DF7EF3DDC"); // None
            public static readonly Guid Music = new Guid("4BD8D571-6D19-48D3-BE97-422220080E43"); // Music
            public static readonly Guid MusicLibrary = new Guid("2112AB0A-C86A-4FFE-A368-0DE96E47012E"); // Music
            public static readonly Guid NetHood = new Guid("C5ABBF53-E17F-4121-8900-86626FC2C973"); // Network Shortcuts
            public static readonly Guid NetworkFolder = new Guid("D20BEEC4-5CA8-4905-AE3B-BF251EA09B53"); // Network
            public static readonly Guid Objects3D = new Guid("31C0DD25-9439-4F12-BF41-7FF4EDA38722"); // 3D Objects
            public static readonly Guid OriginalImages = new Guid("2C36C0AA-5812-4b87-BFD0-4CD0DFB19B39"); // Original Images
            public static readonly Guid PhotoAlbums = new Guid("69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C"); // Slide Shows
            public static readonly Guid PicturesLibrary = new Guid("A990AE9F-A03B-4E80-94BC-9912D7504104"); // Pictures
            public static readonly Guid Pictures = new Guid("33E28130-4E1E-4676-835A-98395C3BC3BB"); // Pictures
            public static readonly Guid Playlists = new Guid("DE92C1C7-837F-4F69-A3BB-86E631204A23"); // Playlists
            public static readonly Guid PrintersFolder = new Guid("76FC4E2D-D6AD-4519-A663-37BD56068185"); // Printers
            public static readonly Guid PrintHood = new Guid("9274BD8D-CFD1-41C3-B35E-B13F55A758F4"); // Printer Shortcuts
            public static readonly Guid Profile = new Guid("5E6C858F-0E22-4760-9AFE-EA3317B67173"); // The user's username (%USERNAME%)
            public static readonly Guid ProgramData = new Guid("62AB5D82-FDC1-4DC3-A9DD-070D1D495D97"); // ProgramData
            public static readonly Guid ProgramFiles = new Guid("905e63b6-c1bf-494e-b29c-65b732d3d21a"); // Program Files
            public static readonly Guid ProgramFilesX64 = new Guid("6D809377-6AF0-444b-8957-A3773F02200E"); // Program Files
            public static readonly Guid ProgramFilesX86 = new Guid("7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E"); // Program Files
            public static readonly Guid ProgramFilesCommon = new Guid("F7F1ED05-9F6D-47A2-AAAE-29D317C6F066"); // Common Files
            public static readonly Guid ProgramFilesCommonX64 = new Guid("6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D"); // Common Files
            public static readonly Guid ProgramFilesCommonX86 = new Guid("DE974D24-D9C6-4D3E-BF91-F4455120B917"); // Common Files
            public static readonly Guid Programs = new Guid("A77F5D77-2E2B-44C3-A6A2-ABA601054A51"); // Programs
            public static readonly Guid Public = new Guid("DFDF76A2-C82A-4D63-906A-5644AC457385"); // Public
            public static readonly Guid PublicDesktop = new Guid("C4AA340D-F20F-4863-AFEF-F87EF2E6BA25"); // Public Desktop
            public static readonly Guid PublicDocuments = new Guid("ED4824AF-DCE4-45A8-81E2-FC7965083634"); // Public Documents
            public static readonly Guid PublicDownloads = new Guid("3D644C9B-1FB8-4f30-9B45-F670235F79C0"); // Public Downloads
            public static readonly Guid PublicGameTasks = new Guid("DEBF2536-E1A8-4c59-B6A2-414586476AEA"); // GameExplorer
            public static readonly Guid PublicLibraries = new Guid("48DAF80B-E6CF-4F4E-B800-0E69D84EE384"); // Libraries
            public static readonly Guid PublicMusic = new Guid("3214FAB5-9757-4298-BB61-92A9DEAA44FF"); // Public Music
            public static readonly Guid PublicPictures = new Guid("B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5"); // Public Pictures
            public static readonly Guid PublicRingtones = new Guid("E555AB60-153B-4D17-9F04-A5FE99FC15EC"); // Ringtones
            public static readonly Guid PublicUserTiles = new Guid("0482af6c-08f1-4c34-8c90-e17ec98b1e17"); // Public Account Pictures
            public static readonly Guid PublicVideos = new Guid("2400183A-6185-49FB-A2D8-4A392A602BA3"); // Public Videos
            public static readonly Guid QuickLaunch = new Guid("52a4f021-7b75-48a9-9f6b-4b87a210bc8f"); // Quick Launch
            public static readonly Guid Recent = new Guid("AE50C081-EBD2-438A-8655-8A092E34987A"); // Recent Items
            public static readonly Guid RecordedTVLibrary = new Guid("1A6FDBA2-F42D-4358-A798-B74D745926C5"); // Recorded TV
            public static readonly Guid RecycleBinFolder = new Guid("B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC"); // Recycle Bin
            public static readonly Guid ResourceDir = new Guid("8AD10C31-2ADB-4296-A8F7-E4701232C972"); // Resources
            public static readonly Guid Ringtones = new Guid("C870044B-F49E-4126-A9C3-B52A1FF411E8"); // Ringtones
            public static readonly Guid RoamingAppData = new Guid("3EB685DB-65F9-4CF6-A03A-E3EF65729F3D"); // Roaming
            public static readonly Guid RoamedTileImages = new Guid("AAA8D5A5-F1D6-4259-BAA8-78E7EF60835E"); // RoamedTileImages
            public static readonly Guid RoamingTiles = new Guid("00BCFC5A-ED94-4e48-96A1-3F6217F21990"); // RoamingTiles
            public static readonly Guid SampleMusic = new Guid("B250C668-F57D-4EE1-A63C-290EE7D1AA1F"); // Sample Music
            public static readonly Guid SamplePictures = new Guid("C4900540-2379-4C75-844B-64E6FAF8716B"); // Sample Pictures
            public static readonly Guid SamplePlaylists = new Guid("15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5"); // Sample Playlists
            public static readonly Guid SampleVideos = new Guid("859EAD94-2E85-48AD-A71A-0969CB56A6CD"); // Sample Videos
            public static readonly Guid SavedGames = new Guid("4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4"); // Saved Games
            public static readonly Guid SavedPictures = new Guid("3B193882-D3AD-4eab-965A-69829D1FB59F"); // Saved Pictures
            public static readonly Guid SavedPicturesLibrary = new Guid("E25B5812-BE88-4bd9-94B0-29233477B6C3"); // Saved Pictures Library
            public static readonly Guid SavedSearches = new Guid("7d1d3a04-debb-4115-95cf-2f29da2920da"); // Searches
            public static readonly Guid Screenshots = new Guid("b7bede81-df94-4682-a7d8-57a52620b86f"); // Screenshots
            public static readonly Guid SEARCH_CSC = new Guid("ee32e446-31ca-4aba-814f-a5ebd2fd6d5e"); // Offline Files
            public static readonly Guid SearchHistory = new Guid("0D4C3DB6-03A3-462F-A0E6-08924C41B5D4"); // History
            public static readonly Guid SearchHome = new Guid("190337d1-b8ca-4121-a639-6d472d16972a"); // Search Results
            public static readonly Guid SEARCH_MAPI = new Guid("98ec0e18-2098-4d44-8644-66979315a281"); // Microsoft Office Outlook
            public static readonly Guid SearchTemplates = new Guid("7E636BFE-DFA9-4D5E-B456-D7B39851D8A9"); // Templates
            public static readonly Guid SendTo = new Guid("8983036C-27C0-404B-8F08-102D10DCFD74"); // SendTo
            public static readonly Guid SidebarDefaultParts = new Guid("7B396E54-9EC5-4300-BE0A-2482EBAE1A26"); // Gadgets
            public static readonly Guid SidebarParts = new Guid("A75D362E-50FC-4fb7-AC2C-A8BEAA314493"); // Gadgets
            public static readonly Guid SkyDrive = new Guid("A52BBA46-E9E1-435f-B3D9-28DAA648C0F6"); // OneDrive
            public static readonly Guid SkyDriveCameraRoll = new Guid("767E6811-49CB-4273-87C2-20F355E1085B"); // Camera Roll
            public static readonly Guid SkyDriveDocuments = new Guid("24D89E24-2F19-4534-9DDE-6A6671FBB8FE"); // Documents
            public static readonly Guid SkyDrivePictures = new Guid("339719B5-8C47-4894-94C2-D8F77ADD44A6"); // Pictures
            public static readonly Guid StartMenu = new Guid("625B53C3-AB48-4EC1-BA1F-A1EF4146FC19"); // Start Menu
            public static readonly Guid Startup = new Guid("B97D20BB-F46A-4C97-BA10-5E3608430854"); // Startup
            public static readonly Guid SyncManagerFolder = new Guid("43668BF8-C14E-49B2-97C9-747784D784B7"); // Sync Center
            public static readonly Guid SyncResultsFolder = new Guid("289a9a43-be44-4057-a41b-587a76d7e7f9"); // Sync Results
            public static readonly Guid SyncSetupFolder = new Guid("0F214138-B1D3-4a90-BBA9-27CBC0C5389A"); // Sync Setup
            public static readonly Guid System = new Guid("1AC14E77-02E7-4E5D-B744-2EB1AE5198B7"); // System32
            public static readonly Guid SystemX86 = new Guid("D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27"); // System32
            public static readonly Guid Templates = new Guid("A63293E8-664E-48DB-A079-DF759E0509F7"); // Templates
            public static readonly Guid UserPinned = new Guid("9E3995AB-1F9C-4F13-B827-48B24B6C7174"); // User Pinned
            public static readonly Guid UserProfiles = new Guid("0762D272-C50A-4BB0-A382-697DCD729B80"); // Users
            public static readonly Guid UserProgramFiles = new Guid("5CD7AEE2-2219-4A67-B85D-6C9CE15660CB"); // Programs
            public static readonly Guid UserProgramFilesCommon = new Guid("BCBD3057-CA5C-4622-B42D-BC56DB0AE516"); // Programs
            public static readonly Guid UsersFiles = new Guid("f3ce0f7c-4901-4acc-8648-d5d44b04ef8f"); // The user's full name (for instance, Jean Philippe Bagel) entered when the user account was created.
            public static readonly Guid UsersLibraries = new Guid("A302545D-DEFF-464b-ABE8-61C8648D939B"); // Libraries
            public static readonly Guid Videos = new Guid("18989B1D-99B5-455B-841C-AB7C74E4DDFC"); // Videos
            public static readonly Guid VideosLibrary = new Guid("491E922F-5643-4AF4-A7EB-4E7A138D8174"); // Videos
            public static readonly Guid Windows = new Guid("F38BF404-1D43-42F2-9305-67DE0B28FC23"); // Windows
        }

        public enum KnownFolderFlag : uint
        {
            DEFAULT = 0x00000000,
            FORCE_APP_DATA_REDIRECTION = 0x00080000,
            RETURN_FILTER_REDIRECTION_TARGET = 0x00040000,
            FORCE_PACKAGE_REDIRECTION = 0x00020000,
            NO_PACKAGE_REDIRECTION = 0x00010000,
            FORCE_APPCONTAINER_REDIRECTION = 0x00020000,
            NO_APPCONTAINER_REDIRECTION = 0x00010000,
            CREATE = 0x00008000,
            DONT_VERIFY = 0x00004000,
            DONT_UNEXPAND = 0x00002000,
            NO_ALIAS = 0x00001000,
            INIT = 0x00000800,
            DEFAULT_PATH = 0x00000400,
            NOT_PARENT_RELATIVE = 0x00000200,
            SIMPLE_IDLIST = 0x00000100,
            ALIAS_ONLY = 0x80000000,
        }

        [DllImport("shell32.dll")]
        static extern uint SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);

        public static string GetKnownFolderPath(Guid guid, KnownFolderFlag flags)
        {
            uint result = SHGetKnownFolderPath(guid, (uint)flags, IntPtr.Zero, out IntPtr pPath);

            try
            {
                if (result == S_OK)
                {
                    string path = Marshal.PtrToStringUni(pPath);
                    return path;
                } else
                {
                    return null;
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(pPath);
            }
        }
    }
}
