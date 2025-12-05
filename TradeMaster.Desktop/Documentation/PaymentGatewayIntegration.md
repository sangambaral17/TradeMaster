# Payment Gateway Integration Guide

## 📋 Current Implementation

**Status:** Payment methods are for **record-keeping only**

The current system allows cashiers to select payment methods (Cash, Card, UPI, eSewa) which are stored in the database with each sale. No actual payment processing occurs.

**Use Cases:**
- Physical card terminals (cashier processes, then records as "Card")
- UPI QR codes (customer pays, cashier confirms and records as "UPI")
- eSewa transfers (customer transfers, cashier verifies and records as "eSewa")
- Cash payments (traditional)

---

## 🔮 Future: Payment Gateway Integration

### Phase 3 Enhancement Plan

When ready to integrate real-time payment processing, follow this guide.

---

## 1️⃣ eSewa Integration (Nepal)

### Setup
1. **Register:** Create merchant account at https://esewa.com.np/
2. **Get Credentials:** Merchant ID, Secret Key
3. **Install Package:** `dotnet add package RestSharp` (for API calls)

### Implementation

**Step 1: Create eSewa Service**
```csharp
// TradeMaster.Desktop/Services/ESewaPaymentService.cs
public class ESewaPaymentService
{
    private const string ESEWA_URL = "https://uat.esewa.com.np/epay/main";
    private readonly string _merchantId;
    private readonly string _secretKey;

    public ESewaPaymentService(string merchantId, string secretKey)
    {
        _merchantId = merchantId;
        _secretKey = secretKey;
    }

    public string GeneratePaymentUrl(decimal amount, string transactionId)
    {
        var parameters = new Dictionary<string, string>
        {
            { "amt", amount.ToString("F2") },
            { "psc", "0" },
            { "pdc", "0" },
            { "txAmt", "0" },
            { "tAmt", amount.ToString("F2") },
            { "pid", transactionId },
            { "scd", _merchantId },
            { "su", "http://localhost:5000/payment/success" },
            { "fu", "http://localhost:5000/payment/failure" }
        };

        var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
        return $"{ESEWA_URL}?{queryString}";
    }

    public async Task<bool> VerifyPayment(string refId, string transactionId, decimal amount)
    {
        // Call eSewa verification API
        // Return true if payment successful
    }
}
```

**Step 2: Update PosViewModel**
```csharp
[RelayCommand]
private async Task ProcessESewaPayment()
{
    if (SelectedPaymentMethod != "eSewa") return;

    var esewaService = new ESewaPaymentService(merchantId, secretKey);
    var paymentUrl = esewaService.GeneratePaymentUrl(TotalAmount, $"TXN-{DateTime.Now.Ticks}");
    
    // Open browser with payment URL
    System.Diagnostics.Process.Start(new ProcessStartInfo
    {
        FileName = paymentUrl,
        UseShellExecute = true
    });

    // Wait for callback or manual confirmation
}
```

---

## 2️⃣ Stripe Integration (International Cards)

### Setup
1. **Register:** https://stripe.com/
2. **Get API Keys:** Publishable Key, Secret Key
3. **Install Package:** `dotnet add package Stripe.net`

### Implementation

```csharp
// TradeMaster.Desktop/Services/StripePaymentService.cs
using Stripe;

public class StripePaymentService
{
    public StripePaymentService(string secretKey)
    {
        StripeConfiguration.ApiKey = secretKey;
    }

    public async Task<PaymentIntent> CreatePaymentIntent(decimal amount, string currency = "usd")
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // Stripe uses cents
            Currency = currency,
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new PaymentIntentService();
        return await service.CreateAsync(options);
    }

    public async Task<bool> ConfirmPayment(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        var intent = await service.GetAsync(paymentIntentId);
        return intent.Status == "succeeded";
    }
}
```

---

## 3️⃣ Razorpay Integration (India - UPI)

### Setup
1. **Register:** https://razorpay.com/
2. **Get Credentials:** Key ID, Key Secret
3. **Install Package:** `dotnet add package Razorpay`

### Implementation

```csharp
// TradeMaster.Desktop/Services/RazorpayService.cs
using Razorpay.Api;

public class RazorpayService
{
    private readonly RazorpayClient _client;

    public RazorpayService(string keyId, string keySecret)
    {
        _client = new RazorpayClient(keyId, keySecret);
    }

    public async Task<Order> CreateOrder(decimal amount)
    {
        var options = new Dictionary<string, object>
        {
            { "amount", (int)(amount * 100) }, // Paise
            { "currency", "INR" },
            { "receipt", $"rcpt_{DateTime.Now.Ticks}" }
        };

        return _client.Order.Create(options);
    }

    public bool VerifyPayment(string orderId, string paymentId, string signature)
    {
        // Verify signature using Razorpay SDK
        return true; // Simplified
    }
}
```

---

## 🔧 Integration Steps

### Step 1: Add Payment Gateway Settings
```csharp
// TradeMaster.Core/Settings/PaymentSettings.cs
public class PaymentSettings
{
    public bool EnableEsewa { get; set; }
    public string EsewaMerchantId { get; set; }
    public string EsewaSecretKey { get; set; }

    public bool EnableStripe { get; set; }
    public string StripePublishableKey { get; set; }
    public string StripeSecretKey { get; set; }

    public bool EnableRazorpay { get; set; }
    public string RazorpayKeyId { get; set; }
    public string RazorpayKeySecret { get; set; }
}
```

### Step 2: Update Sale Entity
```csharp
// Add to Sale.cs
public string? PaymentGatewayTransactionId { get; set; }
public string? PaymentStatus { get; set; } // Pending, Success, Failed
public DateTime? PaymentProcessedAt { get; set; }
```

### Step 3: Create Payment Dialog
```xaml
<!-- TradeMaster.Desktop/Views/PaymentProcessingDialog.xaml -->
<Window Title="Processing Payment...">
    <StackPanel>
        <TextBlock Text="Please complete payment"/>
        <ProgressBar IsIndeterminate="True"/>
        <Button Content="I've Completed Payment" Command="{Binding ConfirmPaymentCommand}"/>
        <Button Content="Cancel" Command="{Binding CancelPaymentCommand}"/>
    </StackPanel>
</Window>
```

### Step 4: Update Checkout Flow
```csharp
[RelayCommand]
private async Task Checkout()
{
    if (!CartItems.Any()) return;

    // Determine if gateway processing needed
    bool requiresGatewayProcessing = SelectedPaymentMethod switch
    {
        "eSewa" => PaymentSettings.EnableEsewa,
        "Card" => PaymentSettings.EnableStripe,
        "UPI" => PaymentSettings.EnableRazorpay,
        _ => false
    };

    if (requiresGatewayProcessing)
    {
        // Process through gateway
        var result = await ProcessPaymentGateway();
        if (!result.Success)
        {
            MessageBox.Show("Payment failed. Please try again.");
            return;
        }
    }

    // Continue with normal checkout...
}
```

---

## 📊 Payment Flow Diagram

```
┌─────────────────┐
│  Select Payment │
│     Method      │
└────────┬────────┘
         │
         ▼
    ┌────────┐
    │ Cash?  │──Yes──► Record & Complete
    └───┬────┘
        │ No
        ▼
┌───────────────────┐
│ Gateway Enabled?  │
└───────┬───────────┘
        │
    Yes │           No
        ▼            ▼
┌──────────────┐  ┌──────────────┐
│ Process via  │  │ Manual       │
│ Gateway API  │  │ Confirmation │
└──────┬───────┘  └──────┬───────┘
       │                 │
       ▼                 ▼
┌──────────────┐  ┌──────────────┐
│ Wait for     │  │ Cashier      │
│ Callback     │  │ Confirms     │
└──────┬───────┘  └──────┬───────┘
       │                 │
       └────────┬────────┘
                ▼
        ┌──────────────┐
        │ Save Sale    │
        │ with Payment │
        │ Details      │
        └──────────────┘
```

---

## ⚠️ Important Considerations

### Security
- **Never store** card details in your database
- Use **HTTPS** for all payment communications
- Store only transaction IDs and status
- Implement **PCI DSS** compliance for card payments

### Testing
- Use **sandbox/test** environments first
- Test all payment scenarios (success, failure, timeout)
- Verify webhook handling
- Test refund flows

### Error Handling
- Handle network failures gracefully
- Implement retry logic
- Log all payment attempts
- Provide clear error messages to users

---

## 🎯 Recommended Implementation Order

1. **eSewa** (if targeting Nepal market)
2. **Razorpay** (if targeting India market)
3. **Stripe** (for international cards)

---

## 📝 Configuration File Example

```json
// appsettings.json
{
  "PaymentGateways": {
    "ESewa": {
      "Enabled": false,
      "Environment": "UAT",
      "MerchantId": "your-merchant-id",
      "SecretKey": "your-secret-key"
    },
    "Stripe": {
      "Enabled": false,
      "PublishableKey": "pk_test_...",
      "SecretKey": "sk_test_..."
    },
    "Razorpay": {
      "Enabled": false,
      "KeyId": "rzp_test_...",
      "KeySecret": "..."
    }
  }
}
```

---

**Note:** This is a comprehensive guide for future implementation. Current system works perfectly for manual payment recording. Implement gateways only when you have merchant accounts and specific business requirements.
