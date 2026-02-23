//using System;
//namespace Mamey.ISO20022.FIN.ErrorCodes.Numeric;
////https://www2.swift.com/knowledgecentre/publications/ufec_20230720/2.0?topic=retrieval.htm

///// <summary>
///// The following error codes are returned in field 401 of Logout and Quit
///// acknowledgements. Logout and Quit commands are always positively acknowledged
///// and the session (General Purpose Application or FIN) closed. However, one of
///// the following error codes can be included in the acknowledgement.
///// </summary>
//public class GeneralQuitErrorCode
//{
//    public GeneralQuitErrorCode()
//    {
//    }
//    /// <summary>
//    /// The Logout command can include the time/day inhibitor which prevents the
//    /// next login occurring before the time/day specified. The time/day in the
//    /// format DDHHMM cannot be more than 7 days after the current date.
//    /// </summary>
//    public string IncorrectTimeDay = "01";
//    /// <summary>
//    /// The trailer block is only present if the message is sent by a training
//    /// logical terminal. If the Logout command is sent from a training logical
//    /// terminal, it must contain a Training trailer.
//    /// </summary>
//    public string TrainingTrailerMissing = "02";
//    /// <summary>
//    /// Each message sent from a logical terminal has an input sequence number.
//    /// The first message sent in the General Purpose Application will always
//    /// have an input sequence number of 000001, whereas the first message sent
//    /// in FIN will have an input sequence number value of the last input sequence
//    /// number+1 sent from that logical terminal. This error will be returned in
//    /// the acknowledgement of a Logout or Quit command when the input sequence
//    /// number of that command is incorrect.
//    /// </summary>
//    public string InputSequenceNumberError = "03";


//}
///// <summary>
///// The following error codes are returned in fields 331, and 333 of
///// acknowledgements, session history reports, and daily check reports:
///// </summary>
//public class ReLoginRequestError
//{
//    /// <summary>
//    /// Re-login request received while logical terminal is active on the Logical Terminal Control association.
//    /// </summary>
//    public string IncorrectTimeDay = "010";
//    /// <summary>
//    /// Logical Terminal Control state error, unable to recover the logical terminal session.
//    /// </summary>
//    public string IncorrectTimeDay = "011";
//    /// <summary>
//    /// Re-login request authentication failed.
//    /// </summary>
//    public string IncorrectTimeDay = "012";
//    /// <summary>
//    /// Re-login request semantic error.
//    /// </summary>
//    public string IncorrectTimeDay = "013";
//    /// <summary>
//    /// Re-login request format error.
//    /// </summary>
//    public string IncorrectTimeDay = "014";
//    /// <summary>
//    /// Re-login request login request number is incorrect.
//    /// </summary>
//    public string IncorrectTimeDay = "015";
//    /// <summary>
//    /// Multiple re-logins from user.
//    /// </summary>
//    public string IncorrectTimeDay = "016";
//    /// <summary>
//    /// Re-login request received while the logical terminal is in unrecoverable state.
//    /// </summary>
//    public string IncorrectTimeDay = "017";
//    /// <summary>
//    /// Re-login request, session recovery information: incorrect General Purpose Application session number.
//    /// </summary>
//    public string IncorrectTimeDay = "019";
//    /// <summary>
//    /// Re-login request, session recovery information: incorrect input sequence number.
//    /// </summary>
//    public string IncorrectTimeDay = "020";
//    /// <summary>
//    /// Re-login request, session recovery information: input sequence number-ACK greater than input sequence number.
//    /// </summary>
//    public string IncorrectTimeDay = "021";
//    /// <summary>
//    /// Re-login request, session recovery information: input sequence number-ACK less than the lower bound.
//    /// </summary>
//    public string IncorrectTimeDay = "022";
//    /// <summary>
//    /// Re-login request, session recovery information: incorrect output sequence number.
//    /// </summary>
//    public string IncorrectTimeDay = "023";
//    /// <summary>
//    /// Re-login request, session recovery information: incorrect window size.
//    /// </summary>
//    public string IncorrectTimeDay = "024";
//    /// <summary>
//    /// Re-login request, a new login request is sent to login a logical terminal while same logical terminal has active login session.
//    /// </summary>
//    public string IncorrectTimeDay = "025";
//    /// <summary>
//    /// Re-login request, login and re-login protocol versions are not the same.
//    /// </summary>
//    public string IncorrectTimeDay = "026";
//    /// <summary>
//    /// Re-login request, logical terminal does not belong to Sign Distinguished Name (DN) organisation.
//    /// </summary>
//    public string IncorrectTimeDay = "027";
//    /// <summary>
//    /// Re-login request, the Live destination that owns the Test and Training destination does not belong to Sign Distinguished Name (DN) organisation.
//    /// </summary>
//    public string IncorrectTimeDay = "028";

//}
///// <summary>
///// The following codes are returned in field 421 of message retrievals
///// </summary>
//public class RetrievalError
//{
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "002";
//    public string IncorrectTimeDay = "003";
//    public string IncorrectTimeDay = "004";
//    public string IncorrectTimeDay = "005";
//    public string IncorrectTimeDay = "006";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//    public string IncorrectTimeDay = "000";
//}
//public class MessageStatusError
//{

//}
//public class AbortReasonError
//{

//}
//public class FINGeneralPurposeApplicationSessionTerminationError
//{

//}
//public class ReportErrors
//{

//}
//public class BulkRetrievalErrorsCode
//{

//}

