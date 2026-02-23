using System.Text.RegularExpressions;

namespace Mamey.Constants;

public static class RegularExpressions
{
    public static Regex PositiveIntegers = new Regex(@"^\d+$");
    public static Regex NegativeIntegers = new Regex(@"^-\d+$");
    public static Regex DecimalNumbers = new Regex(@"/^\d*\.\d+$/");
    public static Regex Integer = new Regex(@"^-?\d+$");
    public static Regex PositiveNumber = new Regex(@"^\d*\.?\d+$");
    public static Regex NegativeNumber = new Regex(@"^-\d*\.?\d+$");
    public static Regex PositiveOrNegativeNumber = new Regex(@"^-?\d*\.?\d+$");
    public static Regex PositiveOrNegativeDecimalNumber = new Regex(@"/^-?\d*(\.\d+)?$/");
    public static Regex IPv4 = new Regex(@"/\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b/");
    public static Regex Domain = new Regex(@"^([a-z][a-z0-9-]+(\.|-*\.))+[a-z]{2,6}$");
    public static Regex Url = new Regex(@"/http[s]?\:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)/");
    public static Regex ZipCode = new Regex(@"/[0-9]{5}(\-[0-9]{4})?$/");
    public static Regex IntegerDecimalFraction = new Regex(@"/[-]?[0-9]+[,.]?[0-9]*([\/][0-9]+[,.]?[0-9]*)*/");
    public static Regex AlphanumericWithoutSpace = new Regex(@"/^[a-zA-Z0-9]*$/");
    public static Regex AlphanumericWithSpace = new Regex(@"/^[a-zA-Z0-9 ]*$/");
    public static Regex Username = new Regex(@"/^[a-z0-9_-]{3,16}$/");
    public static Regex SocialSecurityNumberWithDashes = new Regex(@"^(?!219-09-9999|078-05-1120)(?!666|000|9\d{2})\d{3}-(?!00)\d{2}-(?!0{4})\d{4}$");
    public static Regex SocialSecurityNumberWithoutDashes = new Regex(@"^(?!219099999|078051120)(?!666|000|9\d{2})\d{3}(?!00)\d{2}(?!0{4})\d{4}$");
    public static Regex CreditCardNumber = new Regex(@"/^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$/ ");
    public static Regex IPv6 = new Regex(@"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])) ");
    public static Regex Base64 = new Regex(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$");
    public static Regex ISBN = new Regex(@"/\b(?:ISBN(?:: ?| ))?((?:97[89])?\d{9}[\dx])\b/i");
    public static Regex TwitterUsername = new Regex(@"/@([A-Za-z0-9_]{1,15})/");
    public static Regex FacebookAccountUrl = new Regex(@"/(?:https:\/\/)?(?:www\.)?facebook\.com\/(?:(?:\w)*#!\/)?(?:pages\/)?(?:[\w\-]*\/)*([\w\-]*)/");
    public static Regex HexNumber = new Regex(@"/\#([a-fA-F]|[0-9]){3, 6}/");
    public static Regex YouTubeVideoId = new Regex(@"/http:\/\/(?:youtu\.be\/|(?:[a-z]{2,3}\.)?youtube\.com\/watch(?:\?|#\!)v=)([\w-]{11}).*/gi");
    public static Regex Email = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

}