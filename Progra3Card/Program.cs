using System;
using MySql.Data.MySqlClient; 

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Port=3306;Database=mi_banco_db;Uid=root;Pwd=root;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Funciones a completar:

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            // === A realizar ===
            // Aquí deben implementar un SELECT sobre la tabla 'tarjetas'
            // para recorrer las filas e imprimirlas en la consola.
            
            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)
            
            MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine().ToUpper() == "S")
            {
                // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
                
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirTarjeta()
        {
            Console.Clear();
            Console.WriteLine("   EMISIÓN DE NUEVA TARJETA (ALTA DE CLIENTE)   ");

            //datos del cliente
            string tipoDoc = "";
            while (tipoDoc == "")
            {
                Console.Write("Tipo de documento (1: DNI, 2: PASAPORTE): ");
                string tipoDocOpcion = Console.ReadLine();

                if (tipoDocOpcion == "1")
                {
                    tipoDoc = "DNI";
                } 
                else if (tipoDocOpcion == "2") 
                {
                    tipoDoc = "PASAPORTE";
                } 
                else
                {
                Console.WriteLine ("  ERROR: Escriba un número válido!  ");
                }
            }

            Console.Write("Número de documento: ");
            string documento = Console.ReadLine();
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Apellido: ");
            string apellido = Console.ReadLine();
            Console.Write("Fecha de nacimiento (AAAA-MM-DD): ");
            string fechaNacimiento = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Número de tarjeta (16 dígitos): ");
            string numeroTarjeta = Console.ReadLine();

            string[] bancos = {
                "Banco Nación", "Banco Provincia", "Banco Galicia",
                "Banco Santander", "Banco BBVA", "Banco Macro"
            };

            int opcion = -1;
            while (opcion < 1 || opcion > bancos.Length)
            {
                Console.WriteLine("\nSeleccione el Banco Emisor:");
                for (int i = 0; i < bancos.Length; i++)
                    Console.WriteLine($"{i + 1}. {bancos[i]}");
                Console.Write("Opción: ");
                int.TryParse(Console.ReadLine(), out opcion);
            }

            string bancoEmisor = bancos[opcion - 1];

            bool exito = EmitirTarjeta (tipoDoc, documento, nombre, apellido, fechaNacimiento,
                                            email, numeroTarjeta, bancoEmisor);
            
            if (exito)
                Console.WriteLine("\nCliente y Tarjeta registrados correctamente.");
            else
                Console.WriteLine("\nError al registrar el cliente/tarjeta. Verifique los datos ingresados");
            
            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirLiquidacion()
        {
            Console.Clear();
            Console.WriteLine("   EMISIÓN DE NUEVA LIQUIDACIÓN MENSUAL   ");

            Console.Write("Ingrese el Número de Cuenta: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.Write("Período (AAAA-MM): ");
            string periodo = Console.ReadLine();

            Console.Write("Fecha de vencimiento (AAAA-MM-DD): ");
            string fechaVencimiento = Console.ReadLine();

            Console.Write("Total a pagar: ");
            decimal totalAPagar = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Pago mínimo: ");
            decimal pagoMinimo = Convert.ToDecimal(Console.ReadLine());

            bool exito = EmitirLiquidacion(numCuenta, periodo, fechaVencimiento, 
                        totalAPagar, pagoMinimo);
            
            if (exito)
                Console.WriteLine("\nLiquidación registrada correctamente.");
            else
                Console.WriteLine("\nError al registrar la liquidación. Verifique el número de cuenta.");
            
            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }   


        // =========================================================================
        // MÉTODOS BASE QUE DEBEN COMPLETAR CON LA LÓGICA 
        // =========================================================================

        static void ObtenerYMostrarTarjetas()
        {
            // Completar 
            // Ejemplo de impresión dentro del bucle: 
            // Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", reader["num_cuenta"], reader["numero_tarjeta"], ...);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT num_cuenta, numero_tarjeta, banco_emisor, dni_titular FROM tarjetas";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}",
                                reader["num_cuenta"],
                                reader["numero_tarjeta"],
                                reader["banco_emisor"],
                                reader["dni_titular"]);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
        }

        static void MostrarDetalleCompleto(int cuenta)
        {
            // Completar haciendo un SELECT con JOIN de usuarios y tarjetas WHERE num_cuenta = @cuenta
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT t.num_cuenta, t.numero_tarjeta, t.banco_emisor, t.estado, t.saldo,
                            u.nombre, u.apellido, u.email, u.documento
                        FROM tarjetas t
                        JOIN usuarios u ON t.dni_titular = u.documento
                        WHERE t.num_cuenta = @cuenta";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cuenta", cuenta);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) //1 fila maximo
                            {
                                Console.WriteLine($"\nNúmero de Cuenta : {reader["num_cuenta"]}");
                                Console.WriteLine($"Número de Tarjeta  : {reader["numero_tarjeta"]}");
                                Console.WriteLine($"Banco Emisor       : {reader["banco_emisor"]}");
                                Console.WriteLine($"Estado             : {reader["estado"]}");
                                Console.WriteLine($"Saldo              : ${reader["saldo"]}");
                                Console.WriteLine($"Titular            : {reader["nombre"]} {reader["apellido"]}");
                                Console.WriteLine($"Documento          : {reader["documento"]}");
                                Console.WriteLine($"Email              : {reader["email"]}");
                            }
                            else
                            {
                                Console.WriteLine("\nNo se encontró ninguna tarjeta con ese número de cuenta.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
        }

        static bool DarDeBajaTarjeta(int cuenta)
        {
            // Completar usando un DELETE FROM tarjetas WHERE num_cuenta = @cuenta
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";
                    using (MySqlCommand cmd = new MySqlCommand (query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cuenta", cuenta);
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;  // true si borró algo, false si no existía esa cuenta
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine ($"Error de base de datos: {ex.Message}");
                return false;
            }
        }

        static bool EmitirTarjeta (string tipoDoc, string documento, string nombre, string apellido,
                                    string fechaNacimiento, string email, string numeroTarjeta, string bancoEmisor)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    
                    string queryUsuario = @"INSERT INTO usuarios
                        (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email)
                        VALUES (@documento, @tipoDoc, @nombre, @apellido, @fechaNacimiento, @email)";
            
                    using (MySqlCommand cmdUsuario = new MySqlCommand(queryUsuario, conn))
                    {
                        cmdUsuario.Parameters.AddWithValue("@documento", documento);
                        cmdUsuario.Parameters.AddWithValue("@tipoDoc", tipoDoc);
                        cmdUsuario.Parameters.AddWithValue("@nombre", nombre);
                        cmdUsuario.Parameters.AddWithValue("@apellido", apellido);
                        cmdUsuario.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                        cmdUsuario.Parameters.AddWithValue("@email", email);

                        cmdUsuario.ExecuteNonQuery();
                    }
                    
                    string queryTarjeta = @"INSERT INTO tarjetas
                        (numero_tarjeta, banco_emisor, dni_titular)
                        VALUES (@numeroTarjeta, @bancoEmisor, @documento)";

                    using (MySqlCommand cmdTarjeta = new MySqlCommand(queryTarjeta, conn))
                    {
                        cmdTarjeta.Parameters.AddWithValue("@numeroTarjeta", numeroTarjeta);
                        cmdTarjeta.Parameters.AddWithValue("@bancoEmisor", bancoEmisor);
                        cmdTarjeta.Parameters.AddWithValue("@documento", documento);

                        cmdTarjeta.ExecuteNonQuery();
                    }
                    
                    return true;
                }
            }
            catch(MySqlException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
                return false;
            }
        }

        static bool EmitirLiquidacion(int numCuenta, string periodo, string fechaVencimiento,
                                        decimal totalAPagar, decimal pagoMinimo)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO liquidaciones
                        (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo)
                        VALUES (@numCuenta, @periodo, @fechaVencimiento, @totalAPagar, @pagoMinimo)";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@numCuenta", numCuenta);
                        cmd.Parameters.AddWithValue("@periodo", periodo);
                        cmd.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);
                        cmd.Parameters.AddWithValue("@totalAPagar", totalAPagar);
                        cmd.Parameters.AddWithValue("@pagoMinimo", pagoMinimo);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
                return false;
            }
        }
    }
}
