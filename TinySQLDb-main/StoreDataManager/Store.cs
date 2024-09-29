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
        private OperationStatus Insert(string tableName, List<object> values)
        {
            // Ruta de la tabla donde se insertarán los datos
            var tablePath = $@"{DataPath}\TESTDB\{tableName}.Table";

            // Verificar si la tabla existe
            if (!File.Exists(tablePath))
            {
                Console.WriteLine($"La tabla {tableName} no existe.");
                return OperationStatus.Error;
            }

            // Abrir el archivo de la tabla para agregar los nuevos datos
            using (FileStream stream = new FileStream(tablePath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Escribir los valores en el archivo de la tabla
                foreach (var value in values)
                {
                    if (value is int intValue)
                    {
                        writer.Write(intValue);
                    }
                    else if (value is string stringValue)
                    {
                        // Ajustar el tamaño de las cadenas
                        writer.Write(stringValue.PadRight(30)); // Asumiendo tamaño 30 para nombres
                    }
                    else if (value is DateTime dateTimeValue)
                    {
                        writer.Write(dateTimeValue.ToBinary()); // Guardar DateTime como un número
                    }
                    else
                    {
                        Console.WriteLine($"Tipo de dato no soportado: {value.GetType()}");
                        return OperationStatus.Error;
                    }
                }
            }
            
            return OperationStatus.Success;
        }


        public OperationStatus CreateTable(string command)
        {
            // Parse the command string
            var parts = command.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length != 2)
            {
                Console.WriteLine("Comando SQL inválido.");
                return OperationStatus.Error;
            }

            // Get the table name and columns definition
            var tableName = parts[0].Trim().Split(' ')[2]; // Get the table name after "CREATE TABLE"
            var columnsDefinition = parts[1].Trim();

            // Check if the table file already exists
            var tablePath = $@"{DataPath}\TESTDB\{tableName}.Table";
            if (File.Exists(tablePath))
            {
                Console.WriteLine("Ya existe la tabla " + tableName);
                return OperationStatus.Error;
            }

            // Create the table file
            using (FileStream stream = File.Create(tablePath))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Write the table name
                writer.Write(tableName);
                
                // Parse the columns definition and write to the file
                var columns = columnsDefinition.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var column in columns)
                {
                    var columnDetails = column.Trim().Split(' '); // Split by space to get type and name
                    if (columnDetails.Length != 2)
                    {
                        Console.WriteLine("Definición de columna inválida: " + column);
                        return OperationStatus.Error;
                    }

                    // Example: Store column name and type (you can customize how you store this)
                    writer.Write(columnDetails[1]); // Column name
                    writer.Write(columnDetails[0]); // Column type
                }
            }

            return OperationStatus.Success;
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
