using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;

public class ExceptionRoomNotAttachable : Exception
{
    public override string Message => "Unable to attach the rooms together or one of the rooms are invalid.";
    public static string Smessage = "Unable to attach the rooms together or one of the rooms are invalid.";


}
