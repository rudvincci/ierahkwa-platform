namespace Mamey.ISO20022.FIN.SystemMessages.GeneralPurpose;

public class MT008SystemRequestToQuit : SystemMessage
{
    public const string _code = "008";

    public MT008SystemRequestToQuit()
        : base(_code)
    {
    }
}
public class MT009SystemRequestToLogout : SystemMessage
{
    public const string _code = "009";

    public MT009SystemRequestToLogout()
        : base(_code)
    {
    }
}
public class MT010NonDeliveryWarning : SystemMessage
{
    public const string _code = "010";

    public MT010NonDeliveryWarning()
        : base(_code)
    {
    }
}
public class MT011DeliveryNotification : SystemMessage
{
    public const string _code = "011";

    public MT011DeliveryNotification()
        : base(_code)
    {
    }
}
public class MT012SenderNotification : SystemMessage
{
    public const string _code = "012";

    public MT012SenderNotification()
        : base(_code)
    {
    }
}
public class MT015DelayedNAK : SystemMessage
{
    public const string _code = "015";

    public MT015DelayedNAK()
        : base(_code)
    {
    }
}
public class MT019AbortNotification : SystemMessage
{
    public const string _code = "019";

    public MT019AbortNotification()
        : base(_code)
    {
    }
}
public class MT020RetrievalRequest : SystemMessage
{
    public const string _code = "020";

    public MT020RetrievalRequest()
        : base(_code)
    {
    }
}
public class MT021RetrievedMessage : SystemMessage
{
    public const string _code = "021";

    public MT021RetrievedMessage()
        : base(_code)
    {
    }
}
public class MT022RetrievalRequest : SystemMessage
{
    public const string _code = "022";

    public MT022RetrievalRequest()
        : base(_code)
    {
    }
}
public class MT023RetrievedMessage : SystemMessage
{
    public const string _code = "023";

    public MT023RetrievedMessage()
        : base(_code)
    {
    }
}
public class MT024BulkRetrievalRequest : SystemMessage
{
    public const string _code = "024";

    public MT024BulkRetrievalRequest()
        : base(_code)
    {
    }
}
public class MT025BulkRetrievalResponse : SystemMessage
{
    public const string _code = "025";

    public MT025BulkRetrievalResponse()
        : base(_code)
    {
    }
}
public class MT026FINCopyBulkRetrievalRequest : SystemMessage
{
    public const string _code = "026";

    public MT026FINCopyBulkRetrievalRequest()
        : base(_code)
    {
    }
}
public class MT027FINCopyBulkRetrievalResponse : SystemMessage
{
    public const string _code = "027";

    public MT027FINCopyBulkRetrievalResponse()
        : base(_code)
    {
    }
}
public class MT028FINCopyMessageStatusRequest : SystemMessage
{
    public const string _code = "028";

    public MT028FINCopyMessageStatusRequest()
        : base(_code)
    {
    }
}
public class MT029FINCopyMessageStatusReport : SystemMessage
{
    public const string _code = "029";

    public MT029FINCopyMessageStatusReport()
        : base(_code)
    {
        MaxOutputLength = 10000;
    }
    protected override int MaxOutputLength { get; }
}
public class MT031SessionHistoryRequest : SystemMessage
{
    public const string _code = "031";

    public MT031SessionHistoryRequest()
        : base(_code)
    {
    }
}
public class MT032DeliverySubsetStatusRequest : SystemMessage
{
    public const string _code = "032";

    public MT032DeliverySubsetStatusRequest()
        : base(_code)
    {
    }
}
public class MT035DeliveryInstructionRequest : SystemMessage
{
    public const string _code = "035";

    public MT035DeliveryInstructionRequest()
        : base(_code)
    {
    }
}
public class MT036LogicalTerminalHistoryStatusRequest : SystemMessage
{
    public const string _code = "036";

    public MT036LogicalTerminalHistoryStatusRequest()
        : base(_code)
    {
    }
}
public class MT037TimeZoneStatusRequest : SystemMessage
{
    public const string _code = "037";

    public MT037TimeZoneStatusRequest()
        : base(_code)
    {
    }
}
public class MT041SelectStatusRequestForFIN : SystemMessage
{
    public const string _code = "041";

    public MT041SelectStatusRequestForFIN()
        : base(_code)
    {
    }
}
public class MT042CutOffTimesListRequest : SystemMessage
{
    public const string _code = "042";

    public MT042CutOffTimesListRequest()
        : base(_code)
    {
    }
}
public class MT043NonBankingDaysListRequest : SystemMessage
{
    public const string _code = "043";

    public MT043NonBankingDaysListRequest()
        : base(_code)
    {
    }
}
public class MT044UndeliveredReportRulesRedefinition : SystemMessage
{
    public const string _code = "044";

    public MT044UndeliveredReportRulesRedefinition()
        : base(_code)
    {
    }
}
public class MT045DailyCheckTimeChangeRequest : SystemMessage
{
    public const string _code = "045";

    public MT045DailyCheckTimeChangeRequest()
        : base(_code)
    {
    }
}
public class MT046UndeliveredMessageReportRequest : SystemMessage
{
    public const string _code = "046";

    public MT046UndeliveredMessageReportRequest()
        : base(_code)
    {
    }
}
public class MT047DeliveryInstructionsRedefinitionRequest : SystemMessage
{
    public const string _code = "047";

    public MT047DeliveryInstructionsRedefinitionRequest()
        : base(_code)
    {
    }
}
public class MT048UndeliveredReportRulesRequest : SystemMessage
{
    public const string _code = "048";

    public MT048UndeliveredReportRulesRequest()
        : base(_code)
    {
    }
}

public class MT049DailyCheckReportTimeQuery : SystemMessage
{
    public const string _code = "049";

    public MT049DailyCheckReportTimeQuery()
        : base(_code)
    {
    }
}
public class MT051SessionHistoryReport : SystemMessage
{
    public const string _code = "008";

    public MT051SessionHistoryReport()
        : base(_code)
    {
    }
}
public class MT052DeliverySubsetStatusReport : SystemMessage
{
    public const string _code = "052";

    public MT052DeliverySubsetStatusReport()
        : base(_code)
    {
    }
}
public class MT055DeliveryInstructionsReport : SystemMessage
{
    public const string _code = "055";

    public MT055DeliveryInstructionsReport()
        : base(_code)
    {
    }
}
public class MT056LogicalTerminalHistoryReport : SystemMessage
{
    public const string _code = "056";

    public MT056LogicalTerminalHistoryReport()
        : base(_code)
    {
    }
}
public class MT057TimeZoneStatusReport : SystemMessage
{
    public const string _code = "057";

    public MT057TimeZoneStatusReport()
        : base(_code)
    {
    }
}
public class MT061SelectStatusReportForFIN : SystemMessage
{
    public const string _code = "061";

    public MT061SelectStatusReportForFIN()
        : base(_code)
    {
    }
}
public class MT062CutoffTimeListReport : SystemMessage
{
    public const string _code = "062";

    public MT062CutoffTimeListReport()
        : base(_code)
    {
    }
}
public class MT063NonBankingDaysListReport : SystemMessage
{
    public const string _code = "063";

    public MT063NonBankingDaysListReport()
        : base(_code)
    {
    }
}
public class MT064UndeliveredReportRulesChangeReport : SystemMessage
{
    public const string _code = "064";

    public MT064UndeliveredReportRulesChangeReport()
        : base(_code)
    {
    }
}
public class MT065TimeChangeReportForDailyCheckReport : SystemMessage
{
    public const string _code = "065";

    public MT065TimeChangeReportForDailyCheckReport()
        : base(_code)
    {
    }
}
public class MT066SolicitedUndeliveredMessageReport : SystemMessage
{
    public const string _code = "066";
    public MT066SolicitedUndeliveredMessageReport()
        : base(_code)
    {
        MaxOutputLength = 10000;
    }
    protected override int MaxOutputLength { get; }

}
public class MT067DeliveryInstructionsRedefinitionReport : SystemMessage
{
    public const string _code = "067";

    public MT067DeliveryInstructionsRedefinitionReport()
        : base(_code)
    {
    }
}
public class MT068UndeliveredReportRules : SystemMessage
{
    public const string _code = "068";

    public MT068UndeliveredReportRules()
        : base(_code)
    {
    }
}
public class MT069DailyCheckReportTimeStatus : SystemMessage
{
    public const string _code = "069";

    public MT069DailyCheckReportTimeStatus()
        : base(_code)
    {
    }
}
public class MT070UndeliveredSSIUpdateNotificationReportRequest : SystemMessage
{
    public const string _code = "070";

    public MT070UndeliveredSSIUpdateNotificationReportRequest()
        : base(_code)
    {
    }
}
public class MT071UndeliveredSSIUpdateNotificationReport : SystemMessage
{
    public const string _code = "071";

    public MT071UndeliveredSSIUpdateNotificationReport()
        : base(_code)
    {
    }
}
public class MT072TestModeSelection : SystemMessage
{
    public const string _code = "072";

    public MT072TestModeSelection()
        : base(_code)
    {
    }
}
public class MT073MessageSampleRequest : SystemMessage
{
    public const string _code = "073";

    public MT073MessageSampleRequest()
        : base(_code)
    {
    }
}
public class MT074BroadcastRequest : SystemMessage
{
    public const string _code = "074";

    public MT074BroadcastRequest()
        : base(_code)
    {
    }
}
public class MT077AdditionalSelectionCriteriaForFIN : SystemMessage
{
    public const string _code = "077";

    public MT077AdditionalSelectionCriteriaForFIN()
        : base(_code)
    {
    }
}
public class MT081DailyCheckReport : SystemMessage
{
    public const string _code = "081";

    public MT081DailyCheckReport()
        : base(_code)
    {
    }
}
public class MT082UndeliveredMessageReportAtFixedHour : SystemMessage
{
    public const string _code = "082";
    public MT082UndeliveredMessageReportAtFixedHour()
        : base(_code)
    {
        MaxOutputLength = 10000;
    }

    protected override int MaxOutputLength { get; }
}
public class MT083UndeliveredMessageReportAtCutoffTime : SystemMessage
{
    public const string _code = "083";

    public MT083UndeliveredMessageReportAtCutoffTime()
        : base(_code)
    {
        MaxOutputLength = 10000;
    }

    protected override int MaxOutputLength { get; }
}
public class MT090UserToSwiftMessage : SystemMessage
{
    public const string _code = "090";

    public MT090UserToSwiftMessage()
        : base(_code)
    {
    }
}
public class MT092SwiftToUserMessage : SystemMessage
{
    public const string _code = "092";

    public MT092SwiftToUserMessage()
        : base(_code)
    {
    }
}
public class MT094Broadcast : SystemMessage
{
    public const string _code = "094";

    public MT094Broadcast()
        : base(_code)
    {
    }
}
public class MT096FINCopyToServerDestinationMessage : SystemMessage
{
    public const string _code = "096";

    public MT096FINCopyToServerDestinationMessage()
        : base(_code)
    {
    }
}
public class MT097FINCopyMessageAuthorizationRefusalNotification : SystemMessage
{
    public const string _code = "097";

    public MT097FINCopyMessageAuthorizationRefusalNotification()
        : base(_code)
    {
    }
}