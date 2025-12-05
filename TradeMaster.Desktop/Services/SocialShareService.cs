using System.Diagnostics;
using System.Web;

namespace TradeMaster.Desktop.Services;

/// <summary>
/// Handles sharing bills via social media platforms
/// </summary>
public static class SocialShareService
{
    /// <summary>
    /// Share via WhatsApp
    /// </summary>
    public static bool ShareViaWhatsApp(string message, string phoneNumber = "")
    {
        try
        {
            var encodedMessage = HttpUtility.UrlEncode(message);
            var url = string.IsNullOrEmpty(phoneNumber)
                ? $"whatsapp://send?text={encodedMessage}"
                : $"whatsapp://send?phone={phoneNumber}&text={encodedMessage}";
            
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Share via Viber
    /// </summary>
    public static bool ShareViaViber(string message)
    {
        try
        {
            var encodedMessage = HttpUtility.UrlEncode(message);
            var url = $"viber://forward?text={encodedMessage}";
            
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Share via Telegram
    /// </summary>
    public static bool ShareViaTelegram(string message)
    {
        try
        {
            var encodedMessage = HttpUtility.UrlEncode(message);
            var url = $"tg://msg?text={encodedMessage}";
            
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Share via SMS
    /// </summary>
    public static bool ShareViaSMS(string message, string phoneNumber)
    {
        try
        {
            var encodedMessage = HttpUtility.UrlEncode(message);
            var url = $"sms:{phoneNumber}?body={encodedMessage}";
            
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Share via Email
    /// </summary>
    public static bool ShareViaEmail(string message, string email, string subject = "Invoice from Walsong TradeMaster")
    {
        try
        {
            var encodedSubject = HttpUtility.UrlEncode(subject);
            var encodedBody = HttpUtility.UrlEncode(message);
            var url = $"mailto:{email}?subject={encodedSubject}&body={encodedBody}";
            
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Copy to clipboard as fallback
    /// </summary>
    public static void CopyToClipboard(string text)
    {
        System.Windows.Clipboard.SetText(text);
    }
}
