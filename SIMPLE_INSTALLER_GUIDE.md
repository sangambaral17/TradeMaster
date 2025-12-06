# 🎯 Simple Advanced Installer Guide - Step by Step

## ✅ Before You Start

Your application is already published here:
```
d:\AntigravityProjects\C# Projects\TradeMaster\PublishedApp\
```

---

## 📝 Step-by-Step Instructions

### **STEP 1: Open Advanced Installer**

1. Launch **Advanced Installer** application
2. You'll see a welcome screen

---

### **STEP 2: Create New Project**

1. Click **"New Project"** button (big button in center)
2. In the left panel, select **"Installer"** → **"Simple"**
3. Click **"Create Project"** button at bottom
4. Save the project file as `WalsongTradeMaster.aip` in your project folder

---

### **STEP 3: Product Details** (First Screen)

You'll see the main window with tabs at the top. You're on **"Product Details"** tab.

Fill in:
- **Product Name**: `Walsong TradeMaster`
- **Product Version**: `1.0.0`
- **Company**: `Walsong Nepal`

Click **Next** or go to next tab.

---

### **STEP 4: Files and Folders** ⭐ **YOU ARE HERE**

This is where you add your application files.

#### What You'll See:
- Left side: Tree view showing folders (Application Folder, Desktop, etc.)
- Right side: Files in selected folder

#### What To Do:

**A. Select Application Folder**
1. In the left tree, click on **"Application Folder"**
   - This represents `C:\Program Files\Walsong TradeMaster` on user's PC

**B. Add Your Files**
1. Look at the toolbar at the top
2. Click the **"Add Files..."** button (folder with + icon)
3. A file browser opens
4. Navigate to: `d:\AntigravityProjects\C# Projects\TradeMaster\PublishedApp\`
5. Select **ALL files** in that folder:
   - Hold `Ctrl+A` to select all
   - Or manually select all files you see
6. Click **"Open"** button

**C. Verify Files Added**
- You should now see all files listed in the right panel
- Main file: `TradeMaster.Desktop.exe`
- Database: `trademaster.db`
- Settings: `appsettings.json`
- Many DLL files

✅ **Files and Folders step is DONE!**

---

### **STEP 5: Shortcuts** (Next Tab)

Click the **"Shortcuts/Folders"** tab at the top.

#### What You'll See:
- Tree showing Desktop, Start Menu, etc.

#### Create Desktop Shortcut:

1. In the left tree, find **"Desktop"** folder
2. **Right-click** on "Desktop"
3. Select **"New Shortcut to Published File..."**
4. A dialog opens showing your files
5. Select **"TradeMaster.Desktop.exe"**
6. Click **"OK"**
7. The shortcut appears under Desktop folder

#### Create Start Menu Shortcut:

1. In the left tree, find **"Application Shortcut Folder"** (under Programs Menu)
2. **Right-click** on it
3. Select **"New Shortcut to Published File..."**
4. Select **"TradeMaster.Desktop.exe"**
5. Click **"OK"**

✅ **Shortcuts step is DONE!**

---

### **STEP 6: Launch Conditions** (Optional - Skip for now)

Click the **"Launch Conditions"** tab.

- This is where you'd add .NET requirements
- Since we used `--self-contained`, **you can SKIP this**
- Just click next tab

---

### **STEP 7: Build the Installer**

Now you're ready to create the installer!

#### Build Steps:

1. Click **"Build"** menu at the top
2. Select **"Build"** (or press `F7`)
3. A dialog asks where to save the installer
4. Choose a location (e.g., Desktop or project folder)
5. Click **"Save"**
6. Wait for build to complete (progress bar shows)

#### What You Get:

After build completes, you'll have:
- **WalsongTradeMaster.msi** - The installer file
- This is what you distribute to users!

✅ **Installer is BUILT!**

---

### **STEP 8: Test the Installer**

1. Find the `WalsongTradeMaster.msi` file you just created
2. **Double-click** it to run
3. Follow the installation wizard:
   - Click "Next"
You now have a working installer that:
- ✅ Installs Walsong TradeMaster to Program Files
- ✅ Creates Desktop shortcut
- ✅ Creates Start Menu shortcut
- ✅ Includes all files (database, settings, DLLs)
- ✅ Can be uninstalled from Control Panel

---

## 📋 Quick Reference - What Goes Where

```
Advanced Installer Project Structure:

Application Folder (C:\Program Files\Walsong TradeMaster\)
├── TradeMaster.Desktop.exe  ← Main app
├── trademaster.db           ← Database
├── appsettings.json         ← Settings
└── *.dll files              ← Dependencies

Desktop
└── TradeMaster.lnk          ← Shortcut

Start Menu → Programs → Walsong TradeMaster
└── TradeMaster.lnk          ← Shortcut
```

---

## ❓ Common Questions

**Q: Where do I find "Add Files" button?**
A: In Files and Folders tab, look at the toolbar. It's a folder icon with a + sign.

**Q: I don't see all my files after adding them**
A: Make sure you selected "Application Folder" in the left tree first, then add files.

**Q: How do I create shortcuts?**
A: Go to Shortcuts tab, RIGHT-CLICK on Desktop or Start Menu folder, select "New Shortcut to Published File".

**Q: The build failed**
A: Check that all files are added correctly. Make sure TradeMaster.Desktop.exe is in the list.

**Q: Installer shows "Windows protected your PC"**
A: This is normal for unsigned installers. Click "More info" → "Run anyway". For production, get a code signing certificate.

---

## 🔧 Advanced Installer Interface Guide

```
┌─────────────────────────────────────────────────────┐
│ File  Edit  Build  View  Tools  Help               │ ← Menu Bar
├─────────────────────────────────────────────────────┤
│ [+] [📁] [✂️] [📋]                                   │ ← Toolbar
├─────────────────────────────────────────────────────┤
│ Product Details | Files and Folders | Shortcuts |  │ ← Tabs
├──────────────┬──────────────────────────────────────┤
│ Tree View    │  File List / Details                 │
│              │                                       │
│ Application  │  TradeMaster.Desktop.exe             │
│ Desktop      │  trademaster.db                      │
│ Start Menu   │  appsettings.json                    │
│              │  ... more files ...                  │
└──────────────┴──────────────────────────────────────┘
```

---

## 📞 Need More Help?

If you're stuck at any step:
1. Take a screenshot of what you see
2. Tell me which step number you're on
3. I'll guide you through it!

**Current Status**: Your files are ready in `PublishedApp` folder. Just follow steps 1-8 above!
