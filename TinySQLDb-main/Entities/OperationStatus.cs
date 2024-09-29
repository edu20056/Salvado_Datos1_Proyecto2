using System;
using System.Collections.Generic;

namespace Entities
{
    public enum OperationStatus
    {
        Success,
        Warning,
        Error // Solo como un identificador
    }

    public static class OperationStatusMessages
    {
        private static readonly Dictionary<OperationStatus, string> Messages = new()
        {
            { OperationStatus.Success, "Operación exitosa." },
            { OperationStatus.Warning, "Advertencia: algo no está bien." },
            { OperationStatus.Error, "Se produjo un error: " } // Mensaje base
        };

        public static string GetMessage(OperationStatus status, string detail = "")
        {
            return Messages[status] + detail;
        }
    }
}