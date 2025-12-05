# TradeMaster v1.0.0-qa - QA Test Checklist

**Release Date:** December 5, 2025  
**Build:** v1.0.0-qa  
**Tester:** _______________  
**Test Date:** _______________

---

## 📋 Pre-Test Setup

- [ ] Extract the release package
- [ ] Run `TradeMaster.Desktop.exe`
- [ ] Verify application starts without errors
- [ ] Check that database is created automatically

---

## 🧪 Test Cases

### 1. Product Management

#### 1.1 Add Product
- [ ] Click "Product Management" from main menu
- [ ] Click "Add Product" button
- [ ] Fill in product details:
  - Name: "Test Product 1"
  - SKU: "TEST001"
  - Category: Select any
  - Price: 100.00
  - Currency: NPR
  - Stock: 50
- [ ] Click "Save Product"
- [ ] **Expected:** Product appears in list
- [ ] **Status:** ✅ Pass / ❌ Fail
- [ ] **Notes:** _______________

#### 1.2 Edit Product
- [ ] Click on a product in the list
- [ ] Click "Edit" button
- [ ] Change price to 150.00
- [ ] Click "Save Product"
- [ ] **Expected:** Price updated in list
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 1.3 Delete Product
- [ ] Select a product
- [ ] Click "Delete" button
- [ ] Confirm deletion
- [ ] **Expected:** Product removed from list
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 1.4 Search Products
- [ ] Type product name in search box
- [ ] **Expected:** List filters in real-time
- [ ] Clear search (X button)
- [ ] **Expected:** All products shown again
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 1.5 Sort Products
- [ ] Click "Product Name" column header
- [ ] **Expected:** Products sorted alphabetically (▼ indicator shown)
- [ ] Click again
- [ ] **Expected:** Sort reversed (▲ indicator)
- [ ] Try sorting by "Price" and "Stock Qty"
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 1.6 Pagination
- [ ] Add 60+ products (if needed)
- [ ] **Expected:** Only 50 products shown per page
- [ ] Click "Next" button
- [ ] **Expected:** Page 2 shown
- [ ] Click "Previous" button
- [ ] **Expected:** Back to page 1
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 1.7 Input Validation
- [ ] Click "Add Product"
- [ ] Leave "Name" field empty
- [ ] Try to save
- [ ] **Expected:** Red border on Name field
- [ ] Enter invalid price (letters)
- [ ] **Expected:** Red border on Price field
- [ ] Enter valid data
- [ ] **Expected:** Borders return to normal
- [ ] **Status:** ✅ Pass / ❌ Fail

---

### 2. Point of Sale (POS)

#### 2.1 Add Items to Cart
- [ ] Click "Point of Sale" from main menu
- [ ] Click on a product card
- [ ] **Expected:** Product added to cart with quantity 1
- [ ] Click same product again
- [ ] **Expected:** Quantity increases to 2
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 2.2 Remove from Cart
- [ ] Click ❌ button next to cart item
- [ ] **Expected:** Item removed from cart
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 2.3 Search Products in POS
- [ ] Type product name in search box
- [ ] **Expected:** Product grid filters
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 2.4 Clear Cart
- [ ] Add multiple items to cart
- [ ] Click "Clear" button
- [ ] **Expected:** All items removed
- [ ] **Status:** ✅ Pass / ❌ Fail

---

### 3. Payment Methods

#### 3.1 Cash Payment
- [ ] Add items to cart
- [ ] Select "💵 Cash" payment method
- [ ] Click "CHECKOUT"
- [ ] **Expected:** Success message shown
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 3.2 Card Payment
- [ ] Add items to cart
- [ ] Select "💳 Card" payment method
- [ ] Click "CHECKOUT"
- [ ] **Expected:** Payment method saved with sale
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 3.3 UPI Payment
- [ ] Add items to cart
- [ ] Select "📱 UPI" payment method
- [ ] Click "CHECKOUT"
- [ ] **Expected:** Success message shown
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 3.4 eSewa Payment
- [ ] Add items to cart
- [ ] Select "💰 eSewa" payment method
- [ ] Click "CHECKOUT"
- [ ] **Expected:** Success message shown
- [ ] **Status:** ✅ Pass / ❌ Fail

---

### 4. Bill Sharing

#### 4.1 Share via WhatsApp
- [ ] Complete a checkout
- [ ] Click "Yes" when asked to share bill
- [ ] Enter phone number (optional)
- [ ] Click "📱 WhatsApp" button
- [ ] **Expected:** WhatsApp opens with bill text
- [ ] **Status:** ✅ Pass / ❌ Fail / ⚠️ App not installed

#### 4.2 Share via Viber
- [ ] Complete a checkout
- [ ] Click "Yes" to share
- [ ] Click "💬 Viber" button
- [ ] **Expected:** Viber opens with bill
- [ ] **Status:** ✅ Pass / ❌ Fail / ⚠️ App not installed

#### 4.3 Share via Telegram
- [ ] Complete a checkout
- [ ] Click "Yes" to share
- [ ] Click "✈️ Telegram" button
- [ ] **Expected:** Telegram opens with bill
- [ ] **Status:** ✅ Pass / ❌ Fail / ⚠️ App not installed

#### 4.4 Share via Email
- [ ] Complete a checkout
- [ ] Click "Yes" to share
- [ ] Enter email address
- [ ] Click "📧 Email" button
- [ ] **Expected:** Email client opens with bill
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 4.5 Share via SMS
- [ ] Complete a checkout
- [ ] Click "Yes" to share
- [ ] Enter phone number
- [ ] Click "💬 SMS" button
- [ ] **Expected:** SMS app opens with bill
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 4.6 Copy to Clipboard
- [ ] In share dialog, click "📋 Copy to Clipboard"
- [ ] Paste into notepad (Ctrl+V)
- [ ] **Expected:** Bill text pasted correctly
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 4.7 Bill Format Verification
- [ ] Check bill contains:
  - [ ] Company name
  - [ ] Date and time
  - [ ] Invoice number
  - [ ] Item list with quantities and prices
  - [ ] Total amount
  - [ ] Payment method
  - [ ] Thank you message
- [ ] **Status:** ✅ Pass / ❌ Fail

---

### 5. Stock Management

#### 5.1 Stock Deduction
- [ ] Note current stock of a product (e.g., 50)
- [ ] Sell 3 units via POS
- [ ] Go to Product Management
- [ ] **Expected:** Stock reduced to 47
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 5.2 Low Stock Display
- [ ] Find product with stock < 10
- [ ] **Expected:** Displayed in red/warning color
- [ ] **Status:** ✅ Pass / ❌ Fail

---

### 6. Data Persistence

#### 6.1 Data Retention
- [ ] Add 5 products
- [ ] Complete 2 sales
- [ ] Close application
- [ ] Reopen application
- [ ] **Expected:** All products and sales data retained
- [ ] **Status:** ✅ Pass / ❌ Fail

---

### 7. Error Handling

#### 7.1 Empty Cart Checkout
- [ ] Go to POS with empty cart
- [ ] Click "CHECKOUT"
- [ ] **Expected:** Nothing happens (button should be disabled or show message)
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 7.2 Invalid Input Handling
- [ ] Try entering negative price
- [ ] Try entering negative stock
- [ ] **Expected:** Validation prevents saving
- [ ] **Status:** ✅ Pass / ❌ Fail

---

### 8. UI/UX

#### 8.1 Navigation
- [ ] Test all keyboard shortcuts:
  - Ctrl+P (Product Management)
  - Ctrl+O (Point of Sale)
  - Ctrl+C (Customer Management)
  - Ctrl+R (Sales Reports)
  - Ctrl+T (Settings)
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 8.2 Tooltips
- [ ] Hover over navigation buttons
- [ ] **Expected:** Tooltips show with keyboard shortcuts
- [ ] **Status:** ✅ Pass / ❌ Fail

#### 8.3 Responsive UI
- [ ] Resize window
- [ ] **Expected:** UI adapts properly
- [ ] **Status:** ✅ Pass / ❌ Fail

---

## 🐛 Bugs Found

| # | Description | Severity | Steps to Reproduce | Screenshot |
|---|-------------|----------|-------------------|------------|
| 1 |             | High/Medium/Low |                   |            |
| 2 |             |          |                   |            |
| 3 |             |          |                   |            |

---

## 💡 Suggestions for Improvement

1. _______________________________________________
2. _______________________________________________
3. _______________________________________________

---

## ✅ Overall Assessment

- [ ] **Ready for Production** - No critical issues found
- [ ] **Minor Issues** - Can proceed with fixes
- [ ] **Major Issues** - Requires another QA cycle

**Tester Signature:** _______________  
**Date:** _______________

---

## 📝 Notes

Additional comments or observations:

_______________________________________________
_______________________________________________
_______________________________________________
