using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string x)
        {
            return Store.GetInstance().CreateTable(x);
        }
    }
}