using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace EConnect.Psb.Models;

[Serializable]
public class EConnectException : Exception
{
    public string Code { get; }
    public string? RequestId { get; }
    public DateTimeOffset? DateTime { get; }
    public new string? HelpLink { get; }

    public EConnectException(string code, string message, string? helpLink = null, string? requestId = null, DateTimeOffset? dateTime = null, Exception innerException = null)
        : base(message, innerException)
    {
        Code = code;
        HelpLink = helpLink;
        RequestId = requestId;
        DateTime = dateTime;
    }
        
    public EConnectException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Code = info.GetString("Code");
        RequestId = info.GetString("RequestId");
        DateTime = info.GetDateTime("DateTime");
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        info.AddValue("Code", Code);
        info.AddValue("RequestId", RequestId);
        info.AddValue("DateTime", DateTime);

        base.GetObjectData(info, context);
    }

    public override string ToString()
    {
        return $"[{Code}] {base.ToString()}";
    }
}