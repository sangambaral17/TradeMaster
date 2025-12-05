# TradeMaster v1.0.0-qa - QA Release

**Release Date:** December 5, 2025  
**Build Type:** QA Testing Build  
**Platform:** Windows x64

---

## 📦 What's Included

- `TradeMaster.exe` - Main application (self-contained, no .NET required)
- `QA_Test_Checklist.md` - Comprehensive test checklist
- `README.md` - This file

---

## 🚀 Quick Start

### Installation
1. Extract all files to a folder (e.g., `C:\TradeMaster-QA`)
2. Double-click `TradeMaster.exe` to run
3. No installation or .NET runtime required!

### First Launch
- Application will create a database automatically
- Sample data may be pre-loaded (if configured)
- Main window will appear with navigation menu

---

## ✨ Features to Test

### Phase 1 Features
- **Product Management**
  - Add, edit, delete products
  - Search and filter
  - Sortable columns (Name, Price, Stock)
  - Pagination (50 items/page)
  - Real-time input validation

- **Enhanced UI/UX**
  - Keyboard shortcuts (Ctrl+P, Ctrl+O, etc.)
  - Tooltips on all buttons
  - Loading indicators
  - Error logging

### Phase 2 Features
- **Payment Methods**
  - Cash, Card, UPI, eSewa selection
  - Payment method saved with each sale

- **Bill Sharing**
  - Share via WhatsApp, Viber, Telegram
  - Share via Email, SMS
  - Copy to clipboard fallback
  - Professional bill formatting

---

## 🧪 Testing Instructions

1. **Read the Test Checklist**
   - Open `QA_Test_Checklist.md`
   - Follow test cases in order
   - Mark each test as Pass/Fail

2. **Test All Features**
   - Product Management (CRUD operations)
   - POS checkout flow
   - Payment method selection
   - Bill sharing on all platforms
   - Stock management
   - Data persistence

3. **Report Bugs**
   - Use the bug template in the checklist
   - Include steps to reproduce
   - Note severity (High/Medium/Low)
   - Take screenshots if possible

4. **Provide Feedback**
   - UI/UX suggestions
   - Performance issues
   - Missing features
   - Overall assessment

---

## 🎯 Focus Areas

### Critical Tests
1. **POS Checkout** - Must work flawlessly
2. **Stock Updates** - Verify stock decreases after sale
3. **Payment Methods** - All 4 methods should save correctly
4. **Bill Sharing** - Test at least 2 platforms

### Nice-to-Have Tests
1. Large dataset (100+ products)
2. Long-running sessions
3. Edge cases (special characters, etc.)

---

## 📝 Known Limitations

- **Payment Gateways:** Not integrated yet (manual recording only)
- **Multi-user:** Single user only
- **Cloud Sync:** Not available
- **Barcode Scanner:** Not integrated

---

## 🐛 How to Report Issues

### Bug Report Format
```
**Title:** Brief description
**Severity:** High/Medium/Low
**Steps to Reproduce:**
1. Step 1
2. Step 2
3. Step 3

**Expected Result:** What should happen
**Actual Result:** What actually happened
**Screenshot:** (if applicable)
```

### Where to Report
- Email: [Your Email]
- GitHub Issues: https://github.com/sangambaral17/TradeMaster/issues
- Or use the bug template in QA_Test_Checklist.md

---

## 💾 Database Location

The application stores data in:
```
%AppData%\Walsong\TradeMaster\trademaster.db
```

To reset the database:
1. Close the application
2. Delete the `trademaster.db` file
3. Restart the application (fresh database created)

---

## 📊 System Requirements

- **OS:** Windows 10/11 (64-bit)
- **RAM:** 4GB minimum
- **Disk Space:** 200MB
- **Display:** 1280x720 minimum resolution

---

## 🔧 Troubleshooting

### Application Won't Start
- Check Windows Defender/Antivirus
- Run as Administrator
- Check if port 5000 is available

### Database Errors
- Delete database file and restart
- Check folder permissions

### Bill Sharing Not Working
- Ensure target app is installed (WhatsApp, Viber, etc.)
- Use "Copy to Clipboard" as fallback

---

## 📞 Support

For questions or issues during QA testing:
- Contact: [Your Contact Info]
- GitHub: https://github.com/sangambaral17/TradeMaster

---

## ✅ QA Sign-Off

After completing all tests, please provide:
- [ ] Overall assessment (Ready/Not Ready for Production)
- [ ] List of critical bugs (if any)
- [ ] Suggestions for improvement
- [ ] Tester name and date

---

**Thank you for testing TradeMaster!** 🙏

Your feedback helps us deliver a better product.
