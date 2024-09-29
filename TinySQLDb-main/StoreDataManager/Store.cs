using Entities;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace StoreDataManager
{
    public sealed class Store
    {
        public class ColumnDefinition
        {
            public string Name { get; set; }
            public string DataType { get; set; }

            public ColumnDefinition(string name, string dataType)
            {
                Name = name;
                DataType = dataType;
            }
        }

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
            
            // Extract the table name (first part of the command)
            var tableName = parts[0].Trim().Split(' ')[2]; // Asumiendo que el comando comienza con "CREATE TABLE"
            
            // Extract columns and their definitions (second part of the command)
            var columnsString = parts[1].Trim();
            var columnDefinitions = columnsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var columns = new List<ColumnDefinition>();

            // Parse each column definition
            foreach (var columnDef in columnDefinitions)
            {
                var columnParts = columnDef.Trim().Split(new[] { ' ' }, 2); // Split into column name and data type
                if (columnParts.Length != 2)
                {
                    Console.WriteLine($"Definición de columna inválida: {columnDef}");
                    return OperationStatus.Error;
                }

                var columnName = columnParts[0].Trim();
                var dataType = columnParts[1].Trim();

                // Create a new instance of ColumnDefinition with the name and dataType
                columns.Add(new ColumnDefinition(columnName, dataType));
            }

            // Define the path for the table
            var tablePath = $@"{DataPath}\TESTDB\{tableName}.Table";

            // Check if the table already exists
            if (File.Exists(tablePath))
            {
                Console.WriteLine($"La tabla {tableName} ya existe.");
                return OperationStatus.Error;
            }

            // Create the table file
            using (var writer = new StreamWriter(tablePath))
            {
                // Write the table name
                writer.WriteLine(tableName);

                // Write the column definitions
                for (int i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    // Write each column in the format: ColumnName DataType
                    writer.Write($"{column.Name} {column.DataType}");
                    if (i < columns.Count - 1) // Add a comma if not the last column
                    {
                        writer.Write(", ");
                    }
                }

                // End the line after all columns
                writer.WriteLine();
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
