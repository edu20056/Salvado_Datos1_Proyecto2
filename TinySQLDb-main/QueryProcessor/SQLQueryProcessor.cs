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
                return new CreateTable().Execute(sentence);

            } 
            if (sentence.StartsWith("SELECT"))
            {
                return new Select().Execute();
            }
            if (sentence.StartsWith("INSERT INTO"))
            {
                return new Select().Execute(); //Por el momento se le pone lo mismo de select.....
            }
            else
            {
                throw new UnknownSQLSentenceException();
            }
        }
    }
}
