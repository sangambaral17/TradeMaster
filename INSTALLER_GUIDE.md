# 📦 Creating Installer with Advanced Installer

## Step 1: Publish the Application ✅

The application has been published to `PublishedApp/` folder with the following configuration:
- **Configuration**: Release
- **Runtime**: win-x64 (Windows 64-bit)
- **Self-Contained**: Yes (includes .NET runtime)
- **Single File**: Yes (all files bundled into one EXE)

## Step 2: Install Advanced Installer

1. Download **Advanced Installer** from: https://www.advancedinstaller.com/
2. Install the **Free Edition** (sufficient for basic installers)
3. Launch Advanced Installer

## Step 3: Create New Project

1. Open Advanced Installer
2. Click **"New Project"**
3. Select **"Simple"** project type
4. Choose **"Professional"** or **"Architect"** template
5. Click **"Create"**

## Step 4: Configure Product Details

### Product Information Tab
- **Product Name**: `Walsong TradeMaster - Enterprise Inventory & Sales Management`
- **Product Version**: `1.0.0`
- **Company**: `Walsong Nepal`
- **Website**: Your company website

### Application Details
- **Application Name**: `Walsong TradeMaster`
- **Application Version**: `1.0.0`
- **Publisher**: `Walsong Nepal`

## Step 5: Add Application Files

1. In Advanced Installer, go to **"Files and Folders"** section
2. Click **"Add Files"** button
3. Navigate to: `d:\AntigravityProjects\C# Projects\TradeMaster\PublishedApp\`
4. Select **ALL files** in the PublishedApp folder:
   - `TradeMaster.Desktop.exe` (main executable)
   - `trademaster.db` (database file)
   - `appsettings.json` (settings file)
   - All DLL files
5. Click **"Open"** to add them

**Important**: Set installation folder to:
- Default: `[ProgramFilesFolder]\Walsong TradeMaster`

## Step 6: Configure Shortcuts

### Desktop Shortcut
1. Go to **"Shortcuts/Folders"** section
2. Right-click on **"Desktop"** folder
3. Select **"New Shortcut"**
4. Configure:
   - **Name**: `TradeMaster`
   - **Target**: `TradeMaster.Desktop.exe`
   - **Icon**: Use the EXE icon or add custom icon

### Start Menu Shortcut
1. Right-click on **"Application Shortcut Folder"** (in Start Menu)
2. Select **"New Shortcut"**
3. Configure:
   - **Name**: `TradeMaster`
   - **Target**: `TradeMaster.Desktop.exe`

## Step 7: Set Application Icon (Optional)

1. Go to **"Product Details"** tab
2. Under **"Resources"**, click **"Icon"**
3. Browse and select your application icon (.ico file)
4. This will be shown in Add/Remove Programs

## Step 8: Configure Installation Settings

### Install Parameters
1. Go to **"Install Parameters"** section
2. Set:
   - **Installation Type**: Per-machine (all users) or Per-user
   - **Installation Folder**: `[ProgramFilesFolder]\Walsong TradeMaster`
   - **Create uninstaller**: ✅ Checked

### Prerequisites (Optional)
1. Go to **"Prerequisites"** section
2. Add **.NET 9.0 Runtime** if not using self-contained
   - Since we used `--self-contained`, this is NOT needed

## Step 9: Customize Installer UI

### Dialogs
1. Go to **"Dialogs"** section
2. Customize:
   - **Welcome Dialog**: Add welcome message
   - **License Agreement**: Add your license (optional)
   - **Install Folder Dialog**: Let users choose install location
   - **Finish Dialog**: Add completion message

### Themes
1. Go to **"Themes"** section
2. Choose a modern theme or customize colors

## Step 10: Build the Installer

1. Click **"Build"** menu → **"Build"**
2. Or press **F7**
3. Choose output location for the installer
4. Wait for build to complete

**Output**: You'll get a `.msi` or `.exe` installer file

## Step 11: Test the Installer

1. Locate the generated installer file
2. Run it on a test machine or VM
3. Verify:
   - ✅ Application installs correctly
   - ✅ Desktop shortcut created
   - ✅ Start menu shortcut created
   - ✅ Application runs without errors
   - ✅ Database file is accessible
   - ✅ Uninstaller works properly

## Step 12: Sign the Installer (Optional but Recommended)

### For Production Deployment:
1. Get a **Code Signing Certificate**
2. In Advanced Installer:
   - Go to **"Digital Signature"** section
   - Add your certificate
   - Sign the installer

This prevents Windows SmartScreen warnings.

---

## 📋 Quick Checklist

- [ ] Application published to PublishedApp folder
- [ ] Advanced Installer installed
- [ ] New project created
- [ ] Product details configured
- [ ] All files added from PublishedApp
- [ ] Desktop shortcut created
- [ ] Start menu shortcut created
- [ ] Installation settings configured
- [ ] Installer built successfully
- [ ] Installer tested on clean machine

---

## 🎯 Important Files to Include

Make sure these files are in your installer:

### Required Files (from PublishedApp):
- ✅ `TradeMaster.Desktop.exe` - Main application
- ✅ `trademaster.db` - Database file
- ✅ `appsettings.json` - Settings file
- ✅ All `.dll` files - Dependencies

### Optional Files:
- 📄 `README.md` - User documentation
- 📄 `LICENSE.txt` - License information
- 🖼️ Application icon (.ico)

---

## 🚀 Advanced Installer Tips

### 1. **Auto-Update Feature**
- Go to **"Updater"** section
- Configure automatic update checks
- Specify update URL

### 2. **Custom Actions**
- Add registry entries
- Create database on first run
- Set file associations

### 3. **Multiple Languages**
- Go to **"Translations"** section
- Add Nepali language support
- Translate installer dialogs

### 4. **Silent Installation**
- Users can run: `installer.exe /quiet`
- Useful for enterprise deployment

---

## 📝 Sample Advanced Installer Configuration

```
Product Name: Walsong TradeMaster
Version: 1.0.0
Publisher: Walsong Nepal
Install Location: C:\Program Files\Walsong TradeMaster
Shortcuts:
  - Desktop: TradeMaster.lnk → TradeMaster.Desktop.exe
  - Start Menu: TradeMaster.lnk → TradeMaster.Desktop.exe
Files:
  - TradeMaster.Desktop.exe
  - trademaster.db
  - appsettings.json
  - *.dll (all dependencies)
```

---

## ⚠️ Common Issues & Solutions

### Issue 1: "Application requires .NET Runtime"
**Solution**: You used `--self-contained true`, so this shouldn't happen. If it does, rebuild with self-contained flag.

### Issue 2: Database file not found
**Solution**: Ensure `trademaster.db` is in the same folder as the EXE in the installer.

### Issue 3: Settings not saving
**Solution**: Install to a writable location or handle permissions for `appsettings.json`.

### Issue 4: SmartScreen warning
**Solution**: Sign the installer with a code signing certificate.

---

## 🎉 You're Done!

After following these steps, you'll have a professional installer that:
- ✅ Installs Walsong TradeMaster on any Windows PC
- ✅ Creates desktop and start menu shortcuts
- ✅ Includes all dependencies (no .NET installation needed)
- ✅ Can be uninstalled cleanly
- ✅ Looks professional with custom branding

**Next Steps**:
1. Test the installer thoroughly
2. Get feedback from users
3. Create update versions as needed
4. Consider code signing for production

---

**Need Help?** 
- Advanced Installer Documentation: https://www.advancedinstaller.com/user-guide/
- Video Tutorials: https://www.advancedinstaller.com/video-tutorials.html
