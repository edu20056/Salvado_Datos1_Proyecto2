using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        public static OperationStatus Execute(string sentence)
        {
            /// The following is example code. Parser should be called
            /// on the sentence to understand and process what is requested
            if (sentence.StartsWith("CREATE TABLE"))
            {
                // Divide la cadena en partes y extrae el nombre de la tabla
                var parts = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length >= 3)
                {
                    // `parts[2]` debería ser el nombre de la tabla
                    var tableName = parts[2].TrimEnd(';'); // Remueve el punto y coma al final

                    return new CreateTable().Execute(tableName);
                }
                else
                {
                    throw new InvalidCastException("The SQL syntax is incorrect."); //ESTA NO DEBERÏA SER LA EXCEPTION
                }
            } 
            if (sentence.StartsWith("SELECT"))
            {
                return new Select().Execute();
            }
            else
            {
                throw new UnknownSQLSentenceException();
            }
        }
    }
}
