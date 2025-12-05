using System.Windows;
using TradeMaster.Core.Entities;
using TradeMaster.Desktop.Services;

namespace TradeMaster.Desktop.Views;

public partial class ShareBillDialog : Window
{
    private readonly Sale _sale;
    private readonly List<SaleItem> _items;
    private readonly string _billText;

    public ShareBillDialog(Sale sale, List<SaleItem> items)
    {
        InitializeComponent();
        
        _sale = sale;
        _items = items;
        
        // Generate bill text
        _billText = BillFormatter.FormatBillForSocial(sale, items);
        
        // Show preview
        BillPreviewText.Text = _billText;
    }

    private void ShareWhatsApp_Click(object sender, RoutedEventArgs e)
    {
        var phone = ContactTextBox.Text.Trim();
        var success = SocialShareService.ShareViaWhatsApp(_billText, phone);
        
        if (success)
        {
            MessageBox.Show("Opening WhatsApp...", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Could not open WhatsApp. Make sure it's installed.\n\nBill copied to clipboard instead.", 
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            SocialShareService.CopyToClipboard(_billText);
        }
    }

    private void ShareViber_Click(object sender, RoutedEventArgs e)
    {
        var success = SocialShareService.ShareViaViber(_billText);
        
        if (success)
        {
            MessageBox.Show("Opening Viber...", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Could not open Viber. Make sure it's installed.\n\nBill copied to clipboard instead.", 
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            SocialShareService.CopyToClipboard(_billText);
        }
    }

    private void ShareTelegram_Click(object sender, RoutedEventArgs e)
    {
        var success = SocialShareService.ShareViaTelegram(_billText);
        
        if (success)
        {
            MessageBox.Show("Opening Telegram...", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Could not open Telegram. Make sure it's installed.\n\nBill copied to clipboard instead.", 
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            SocialShareService.CopyToClipboard(_billText);
        }
    }

    private void ShareEmail_Click(object sender, RoutedEventArgs e)
    {
        var email = ContactTextBox.Text.Trim();
        
        if (string.IsNullOrEmpty(email))
        {
            MessageBox.Show("Please enter customer email address.", "Email Required", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            ContactTextBox.Focus();
            return;
        }
        
        var success = SocialShareService.ShareViaEmail(_billText, email);
        
        if (success)
        {
            MessageBox.Show("Opening email client...", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Could not open email client.\n\nBill copied to clipboard instead.", 
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            SocialShareService.CopyToClipboard(_billText);
        }
    }

    private void ShareSMS_Click(object sender, RoutedEventArgs e)
    {
        var phone = ContactTextBox.Text.Trim();
        
        if (string.IsNullOrEmpty(phone))
        {
            MessageBox.Show("Please enter customer phone number.", "Phone Required", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            ContactTextBox.Focus();
            return;
        }
        
        var success = SocialShareService.ShareViaSMS(_billText, phone);
        
        if (success)
        {
            MessageBox.Show("Opening SMS app...", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Could not open SMS app.\n\nBill copied to clipboard instead.", 
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            SocialShareService.CopyToClipboard(_billText);
        }
    }

    private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
    {
        SocialShareService.CopyToClipboard(_billText);
        MessageBox.Show("Bill copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
