//
// using System.Xml;
// using Mamey.ISO20022.Messages.BusinessApplicationHeader;
//
// namespace Mamey.ISO.ISO20022.Builders;
//
// public class MessageBuilder
// {
//     public MessageBuilder()
//     {
//         Header = new BusinessApplicationHeaderV04()
//         {
//             BizMsgIdr = "",
//             BizPrcgDt = new DateTime(),
//             BizPrcgDtSpecified = false,
//             BizSvc = "",
//             CharSet = "",
//             CpyDplct = CopyDuplicate1Code.CODU,
//             CpyDplctSpecified = false,
//             CreDt = new DateTime(),
//             Fr = new Party51Choice
//             {
//                 Item = new object()
//             },
//             MktPrctc = new ImplementationSpecification1()
//             {
//                 Id = "",
//                 Regy = ""
//             },
//             MsgDefIdr = "",
//             Prty = "",
//             PssblDplct = false,
//             PssblDplctSpecified = false,
//             Rltd = Enumerable.Empty<BusinessApplicationHeader8>().ToArray(),
//             Sgntr = default,
//             To = new Party51Choice
//             {
//                 Item = new object()
//             }
//         };
//     }
//     public BusinessApplicationHeaderV04 Header { get; set;}
// }