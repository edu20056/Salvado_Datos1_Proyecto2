using Entities;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace StoreDataManager
{
    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();
               
        public static Store GetInstance()
        {
            lock(_lock)
            {
                if (instance == null) 
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        private const string DatabaseBasePath = @"C:\Users\ejcan\Desktop\U\FSC\Proyecto 2\Salvado_Datos1_Proyecto2\TinySql";
        private const string DataPath = $@"{DatabaseBasePath}\Data";
        private const string SystemCatalogPath = $@"{DataPath}\SystemCatalog";
        private const string SystemDatabasesFile = $@"{SystemCatalogPath}\SystemDatabases.table";
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.table";

        public Store()
        {
            this.InitializeSystemCatalog();
            
        }

        private void InitializeSystemCatalog()
        {
            // Always make sure that the system catalog and above folder
            // exist when initializing
            Directory.CreateDirectory(SystemCatalogPath);
        }


        public OperationStatus CreateTable(string x)
        {
            // Creates a default DB called TESTDB
            Directory.CreateDirectory($@"{DataPath}\TESTDB");

            // Creates a table file with the name provided in the parameter x
            var tablePath = $@"{DataPath}\TESTDB\{x}.Table";

            // Check if the table file already exists
            if (!File.Exists(tablePath))
            {
                using (File.Create(tablePath)) { } // Create the file if it does not exist
                return OperationStatus.Success;
            }
            else
            {
                Console.WriteLine("Ya existe la tabla " +x);
                return OperationStatus.Error;
            }
        }

        public OperationStatus Select()
        {
            // Creates a default Table called ESTUDIANTES
            var tablePath = $@"{DataPath}\TESTDB\ESTUDIANTES.Table";
            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryReader reader = new (stream))
            {
                // Print the values as a I know exactly the types, but this needs to be done right
                Console.WriteLine(reader.ReadInt32());
                Console.WriteLine(reader.ReadString());
                Console.WriteLine(reader.ReadString());
                return OperationStatus.Success;
            }
        }
    }
}
