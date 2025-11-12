using System;
using System.Linq;
using GimnasioApp.Connection;
using GimnasioApp.Models;
using GimnasioApp.Managers;

class Program
{
    static readonly UsuarioManager usuarioManager = new();
    static readonly SocioManager socioManager = new();
    static readonly PlanManager planManager = new();
    static readonly PagoManager pagoManager = new();
    static readonly AsistenciaManager asistenciaManager = new();
    static readonly ReporteManager reporteManager = new();
    
    static Usuario? _currentUser;

    static async Task Main(string[] args)
    {
        try
        {
            // Inicializar base de datos SQLite
            Console.WriteLine("Inicializando base de datos...");
            await DatabaseConnection.InitializeDatabaseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }
        
        try
        {
            Console.WriteLine("Iniciando aplicación del gimnasio...");
            
            if (!await DatabaseConnection.TestConnectionAsync())
            {
                Console.WriteLine("Error de conexión a la base de datos. Verifique la configuración.");
                return;
            }
            Console.WriteLine("Conexión a la base de datos exitosa!");

            // Solicitar login antes de continuar
            if (!await DoLoginAsync())
            {
                Console.WriteLine("Login fallido. Cerrando aplicación.");
                return;
            }

            bool continuar = true;
            while (continuar)
            {
                continuar = await MostrarMenuPrincipal();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task<bool> DoLoginAsync()
    {
        int intentos = 0;
        const int MAX_INTENTOS = 3;

        while (intentos < MAX_INTENTOS)
        {
            Console.Clear();
            Console.WriteLine("=== LOGIN ===");
            Console.Write("Usuario o Email: ");
            string userOrMail = Console.ReadLine() ?? "";
            
            Console.Write("Contraseña: ");
            string password = ReadPassword();

            try
            {
                var (ok, usuario) = await usuarioManager.ValidateLoginAsync(userOrMail, password);
                if (ok && usuario != null)
                {
                    _currentUser = usuario;
                    Console.Clear();
                    Console.WriteLine($"¡Bienvenido {usuario.NombreUsuario}!");
                    Console.WriteLine($"Rol: {usuario.Rol}");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return true;
                }
                else
                {
                    intentos++;
                    Console.WriteLine($"\nCredenciales inválidas. Intentos restantes: {MAX_INTENTOS - intentos}");
                    Console.WriteLine("Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                return false;
            }
        }

        return false;
    }

    static string ReadPassword()
    {
        var pass = string.Empty;
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && pass.Length > 0)
            {
                Console.Write("\b \b");
                pass = pass[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                pass += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);
        
        Console.WriteLine();
        return pass;
    }

    static async Task RegistrarEntradaAsync()
    {
        Console.WriteLine("=== REGISTRAR ENTRADA ===");
        Console.Write("DNI del socio: ");
        var dni = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(dni)) { Console.WriteLine("DNI inválido."); return; }

        var socio = (await socioManager.GetAllAsync()).FirstOrDefault(s => s.DNI == dni);
        if (socio == null) { Console.WriteLine("Socio no encontrado."); return; }
        if (!string.Equals(socio.Estado, "Activo", StringComparison.OrdinalIgnoreCase))
        { Console.WriteLine("El socio no está activo."); return; }

        var asistencia = new Asistencia { SocioId = socio.Id, Fecha = DateTime.Now.Date, HoraEntrada = DateTime.Now.TimeOfDay };
        await asistenciaManager.AddAsistenciaAsync(asistencia);
        Console.WriteLine($"Entrada registrada a las {DateTime.Now:HH:mm} para {socio.Nombre} {socio.Apellido}.");
    }

    static async Task RegistrarSalidaAsync()
    {
        Console.WriteLine("=== REGISTRAR SALIDA ===");
        Console.Write("DNI del socio: ");
        var dni = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(dni)) { Console.WriteLine("DNI inválido."); return; }

        var socio = (await socioManager.GetAllAsync()).FirstOrDefault(s => s.DNI == dni);
        if (socio == null) { Console.WriteLine("Socio no encontrado."); return; }

        var asistenciasHoy = await asistenciaManager.GetAsistenciasByDateAsync(DateTime.Now.Date);
        var asistencia = asistenciasHoy.FirstOrDefault(a => a.SocioId == socio.Id && !a.HoraSalida.HasValue);
        if (asistencia == null) { Console.WriteLine("No hay entrada registrada hoy para este socio."); return; }

        asistencia.HoraSalida = DateTime.Now.TimeOfDay;
        await asistenciaManager.UpdateAsistenciaAsync(asistencia);
        Console.WriteLine($"Salida registrada a las {DateTime.Now:HH:mm} para {socio.Nombre} {socio.Apellido}.");
    }

    static async Task VerAsistenciasHoyAsync()
    {
        Console.WriteLine("=== ASISTENCIAS DE HOY ===");
        var asistencias = await asistenciaManager.GetAsistenciasByDateAsync(DateTime.Now.Date);
        if (!asistencias.Any()) { Console.WriteLine("Sin asistencias registradas hoy."); return; }

        foreach (var a in asistencias)
        {
            var socio = await socioManager.GetByIdAsync(a.SocioId);
            Console.WriteLine($"Socio: {socio?.Nombre} {socio?.Apellido} | Entrada: {a.HoraEntrada:hh\\:mm} | Salida: {(a.HoraSalida.HasValue ? a.HoraSalida.Value.ToString(@"hh\:mm") : "No registrada")}");
        }
    }

    static async Task<bool> MostrarMenuPrincipal()
    {
        Console.Clear();
        Console.WriteLine($"=== SISTEMA DE GESTIÓN DE GIMNASIO === [Usuario: {_currentUser?.NombreUsuario} - Rol: {_currentUser?.Rol}]");

        switch (_currentUser?.Rol)
        {
            case "Administrador":
                Console.WriteLine("1. Gestión de Socios");
                Console.WriteLine("2. Gestión de Planes");
                Console.WriteLine("3. Pagos");
                Console.WriteLine("4. Control de Asistencia");
                Console.WriteLine("5. Reportes");
                Console.WriteLine("6. Gestión de Usuarios");
                Console.WriteLine("7. Salir");
                break;
            case "Recepcionista":
                Console.WriteLine("1. Registrar Entrada");
                Console.WriteLine("2. Registrar Salida");
                Console.WriteLine("3. Ver Asistencias de Hoy");
                Console.WriteLine("4. Consultar Socio");
                Console.WriteLine("5. Registrar Pago");
                Console.WriteLine("6. Salir");
                break;
            case "Profesor":
                Console.WriteLine("1. Ver lista de socios activos");
                Console.WriteLine("2. Consultar asistencias");
                Console.WriteLine("3. Ver planes activos");
                Console.WriteLine("4. Salir");
                break;
            default:
                Console.WriteLine("Rol no válido");
                return false;
        }

        Console.Write("\nSeleccione una opción: ");
        return await ProcesarOpcionMenu(Console.ReadLine());
    }

    static async Task<bool> ProcesarOpcionMenu(string? opcion)
    {
        if (_currentUser == null) return false;

        Console.Clear();
        try
        {
            switch (_currentUser.Rol)
            {
                case "Administrador":
                    switch (opcion)
                    {
                        case "1": await MenuSociosAsync(); break;
                        case "2": await MenuPlanesAsync(); break;
                        case "3": await MenuPagosAsync(); break;
                        case "4": await MenuAsistenciaAsync(); break;
                        case "5": await MenuReportesAsync(); break;
                        case "6": await MenuUsuariosAsync(); break;
                        case "7": return false;
                        default: Console.WriteLine("Opción no válida"); break;
                    }
                    break;

                case "Recepcionista":
                    switch (opcion)
                    {
                        case "1": await RegistrarEntradaAsync(); break;
                        case "2": await RegistrarSalidaAsync(); break;
                        case "3": await VerAsistenciasHoyAsync(); break;
                        case "4": await ConsultarSocioRapidoAsync(); break;
                        case "5": await RegistrarPagoRapidoAsync(); break;
                        case "6": return false;
                        default: Console.WriteLine("Opción no válida"); break;
                    }
                    break;

                case "Profesor":
                    switch (opcion)
                    {
                        case "1": await VerSociosActivosAsync(); break;
                        case "2": await ConsultarAsistenciasAsync(); break;
                        case "3": await VerPlanesActivosAsync(); break;
                        case "4": return false;
                        default: Console.WriteLine("Opción no válida"); break;
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
        return true;
    }

    static async Task MenuSociosAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== GESTIÓN DE SOCIOS ===");
            Console.WriteLine("1. Listar socios");
            Console.WriteLine("2. Agregar socio");
            Console.WriteLine("3. Modificar socio");
            Console.WriteLine("4. Buscar socio");
            Console.WriteLine("5. Dar de baja socio");
            Console.WriteLine("6. Socios con cuota vencida");
            Console.WriteLine("7. Volver");

            Console.Write("\nSeleccione una opción: ");
            var opt = Console.ReadLine();

            try
            {
                switch (opt)
                {
                    case "1":
                        var socios = await socioManager.GetAllAsync();
                        Console.WriteLine("\nListado de Socios:");
                        Console.WriteLine("ID | Nombre | Apellido | DNI | Estado | Plan");
                        foreach (var s in socios)
                        {
                            var plan = s.PlanId.HasValue ? await planManager.GetByIdAsync(s.PlanId.Value) : null;
                            Console.WriteLine($"{s.Id} | {s.Nombre} {s.Apellido} | {s.DNI} | {s.Estado} | {plan?.NombrePlan ?? "Sin plan"}");
                        }
                        break;

                    case "2":
                        var nuevoSocio = new Socio();
                        Console.Write("Nombre: "); nuevoSocio.Nombre = Console.ReadLine() ?? "";
                        Console.Write("Apellido: "); nuevoSocio.Apellido = Console.ReadLine() ?? "";
                        Console.Write("DNI: "); nuevoSocio.DNI = Console.ReadLine() ?? "";
                        Console.Write("Teléfono: "); nuevoSocio.Telefono = Console.ReadLine() ?? "";
                        Console.Write("Email: "); nuevoSocio.Mail = Console.ReadLine() ?? "";
                        Console.Write("Dirección: "); nuevoSocio.Direccion = Console.ReadLine() ?? "";

                        // Mostrar planes disponibles
                        var planes = (await planManager.GetAllAsync())
                            .GroupBy(p => (p.NombrePlan ?? string.Empty).Trim().ToUpperInvariant())
                            .Select(g => g.OrderByDescending(x => x.Id).First())
                            .OrderBy(p => p.NombrePlan)
                            .ToList();
                        Console.WriteLine("\nPlanes disponibles:");
                        foreach (var p in planes)
                        {
                            Console.WriteLine($"{p.Id}. {p.NombrePlan} - ${p.Precio}");
                        }
                        Console.Write("\nSeleccione ID del plan: ");
                        if (int.TryParse(Console.ReadLine(), out int planId))
                        {
                            nuevoSocio.PlanId = planId;
                        }

                        nuevoSocio.FechaIngreso = DateTime.Now;
                        nuevoSocio.Estado = "Activo";

                        var idSocio = await socioManager.AddAsync(nuevoSocio);
                        Console.WriteLine($"\nSocio registrado con ID: {idSocio}");
                        break;

                    case "3":
                        Console.Write("ID del socio a modificar: ");
                        if (int.TryParse(Console.ReadLine(), out int idMod))
                        {
                            var socioMod = await socioManager.GetByIdAsync(idMod);
                            if (socioMod != null)
                            {
                                Console.Write($"Nombre [{socioMod.Nombre}]: "); 
                                var tmp = Console.ReadLine(); 
                                if (!string.IsNullOrEmpty(tmp)) socioMod.Nombre = tmp;

                                Console.Write($"Apellido [{socioMod.Apellido}]: "); 
                                tmp = Console.ReadLine(); 
                                if (!string.IsNullOrEmpty(tmp)) socioMod.Apellido = tmp;

                                Console.Write($"DNI [{socioMod.DNI}]: "); 
                                tmp = Console.ReadLine(); 
                                if (!string.IsNullOrEmpty(tmp)) socioMod.DNI = tmp;

                                await socioManager.UpdateAsync(socioMod);
                                Console.WriteLine("Socio actualizado correctamente.");
                            }
                            else
                            {
                                Console.WriteLine("Socio no encontrado.");
                            }
                        }
                        break;

                    case "4":
                        Console.Write("Buscar por DNI: ");
                        var dniBusqueda = Console.ReadLine();
                        var sociosEncontrados = await socioManager.GetAllAsync();
                        var resultados = sociosEncontrados.Where(s => s.DNI.Contains(dniBusqueda ?? ""));
                        
                        foreach (var s in resultados)
                        {
                            Console.WriteLine($"ID: {s.Id}, Nombre: {s.Nombre} {s.Apellido}, DNI: {s.DNI}, Estado: {s.Estado}");
                        }
                        break;

                    case "5":
                        Console.Write("ID del socio a dar de baja: ");
                        if (int.TryParse(Console.ReadLine(), out int idBaja))
                        {
                            await socioManager.DeleteAsync(idBaja);
                            Console.WriteLine("Socio dado de baja correctamente.");
                        }
                        break;

                    case "6":
                        var morosos = await reporteManager.GetSociosConCuotaVencidaAsync();
                        Console.WriteLine("\nSocios con cuota vencida:");
                        foreach (var m in morosos)
                        {
                            Console.WriteLine($"ID: {m.Id}, Nombre: {m.Nombre} {m.Apellido}, DNI: {m.DNI}");
                        }
                        break;

                    case "7":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (!back)
            {
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    static async Task MenuPlanesAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== GESTIÓN DE PLANES ===");
            Console.WriteLine("1. Listar planes");
            Console.WriteLine("2. Agregar plan");
            Console.WriteLine("3. Modificar plan");
            Console.WriteLine("4. Eliminar plan");
            Console.WriteLine("5. Volver");

            Console.Write("\nSeleccione una opción: ");
            var opt = Console.ReadLine();

            try
            {
                switch (opt)
                {
                    case "1":
                        var planes = (await planManager.GetAllAsync())
                            .GroupBy(p => (p.NombrePlan ?? string.Empty).Trim().ToUpperInvariant())
                            .Select(g => g.OrderByDescending(x => x.Id).First())
                            .OrderBy(p => p.NombrePlan)
                            .ToList();
                        Console.WriteLine("\nListado de Planes:");
                        Console.WriteLine("ID | Nombre | Duración (días) | Precio | Descripción");
                        foreach (var p in planes)
                        {
                            Console.WriteLine($"{p.Id} | {p.NombrePlan} | {p.DuracionDias} | ${p.Precio} | {p.Descripcion}");
                        }
                        break;

                    case "2":
                        var nuevoPlan = new Plan();
                        Console.Write("Nombre del plan: "); nuevoPlan.NombrePlan = Console.ReadLine() ?? "";
                        Console.Write("Duración en días: "); 
                        if (int.TryParse(Console.ReadLine(), out int dias))
                            nuevoPlan.DuracionDias = dias;
                        
                        Console.Write("Precio: $"); 
                        if (decimal.TryParse(Console.ReadLine(), out decimal precio))
                            nuevoPlan.Precio = precio;
                        
                        Console.Write("Descripción: "); nuevoPlan.Descripcion = Console.ReadLine() ?? "";

                        if (nuevoPlan.DuracionDias > 0 && nuevoPlan.Precio > 0)
                        {
                            var idPlan = await planManager.AddAsync(nuevoPlan);
                            Console.WriteLine($"\nPlan registrado con ID: {idPlan}");
                        }
                        else
                        {
                            Console.WriteLine("Error: La duración y el precio deben ser mayores a 0");
                        }
                        break;

                    case "3":
                        Console.Write("ID del plan a modificar: ");
                        if (int.TryParse(Console.ReadLine(), out int idMod))
                        {
                            var planMod = await planManager.GetByIdAsync(idMod);
                            if (planMod != null)
                            {
                                Console.Write($"Nombre [{planMod.NombrePlan}]: "); 
                                var tmp = Console.ReadLine(); 
                                if (!string.IsNullOrEmpty(tmp)) planMod.NombrePlan = tmp;

                                Console.Write($"Duración en días [{planMod.DuracionDias}]: "); 
                                if (int.TryParse(Console.ReadLine(), out dias))
                                    planMod.DuracionDias = dias;

                                Console.Write($"Precio [{planMod.Precio}]: $"); 
                                if (decimal.TryParse(Console.ReadLine(), out precio))
                                    planMod.Precio = precio;

                                Console.Write($"Descripción [{planMod.Descripcion}]: "); 
                                tmp = Console.ReadLine(); 
                                if (!string.IsNullOrEmpty(tmp)) planMod.Descripcion = tmp;

                                if (planMod.DuracionDias > 0 && planMod.Precio > 0)
                                {
                                    await planManager.UpdateAsync(planMod);
                                    Console.WriteLine("Plan actualizado correctamente.");
                                }
                                else
                                {
                                    Console.WriteLine("Error: La duración y el precio deben ser mayores a 0");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Plan no encontrado.");
                            }
                        }
                        break;

                    case "4":
                        Console.Write("ID del plan a eliminar: ");
                        if (int.TryParse(Console.ReadLine(), out int idDel))
                        {
                            // Verificar si hay socios usando este plan
                            var socios = await socioManager.GetAllAsync();
                            if (socios.Any(s => s.PlanId == idDel))
                            {
                                Console.WriteLine("No se puede eliminar el plan porque hay socios que lo están usando.");
                            }
                            else
                            {
                                await planManager.DeleteAsync(idDel);
                                Console.WriteLine("Plan eliminado correctamente.");
                            }
                        }
                        break;

                    case "5":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (!back)
            {
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    static async Task MenuPagosAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== GESTIÓN DE PAGOS ===");
            Console.WriteLine("1. Ver pagos por socio");
            Console.WriteLine("2. Registrar nuevo pago");
            Console.WriteLine("3. Ver pagos del mes");
            Console.WriteLine("4. Volver");

            Console.Write("\nSeleccione una opción: ");
            var opt = Console.ReadLine();

            try
            {
                switch (opt)
                {
                    case "1":
                        Console.Write("ID del Socio: ");
                        if (int.TryParse(Console.ReadLine(), out int idSocio))
                        {
                            var socio = await socioManager.GetByIdAsync(idSocio);
                            if (socio != null)
                            {
                                Console.WriteLine($"\nPagos de {socio.Nombre} {socio.Apellido}:");
                                var pagos = await pagoManager.GetPagosBySocioAsync(idSocio);
                                if (pagos.Any())
                                {
                                    foreach (var p in pagos)
                                    {
                                        Console.WriteLine($"ID: {p.IdPago} | Fecha: {p.FechaPago:dd/MM/yyyy} | Monto: ${p.Monto} | Método: {p.Metodo}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No hay pagos registrados");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Socio no encontrado");
                            }
                        }
                        break;

                    case "2":
                        Console.Write("ID del Socio: ");
                        if (int.TryParse(Console.ReadLine(), out int idPago))
                        {
                            var socio = await socioManager.GetByIdAsync(idPago);
                            if (socio != null)
                            {
                                Console.Write("Monto: $");
                                if (decimal.TryParse(Console.ReadLine(), out decimal monto))
                                {
                                    Console.WriteLine("\nMétodo de pago:");
                                    Console.WriteLine("1. Efectivo");
                                    Console.WriteLine("2. Tarjeta");
                                    Console.WriteLine("3. Transferencia");
                                    Console.Write("Seleccione: ");
                                    
                                    string metodo = Console.ReadLine()?.Trim() switch
                                    {
                                        "1" => "Efectivo",
                                        "2" => "Tarjeta",
                                        "3" => "Transferencia",
                                        _ => "Efectivo"
                                    };

                                    var pago = new Pago
                                    {
                                        SocioId = idPago,
                                        FechaPago = DateTime.Now,
                                        Monto = monto,
                                        Metodo = metodo,
                                    };

                                    var newId = await pagoManager.AddPagoAsync(pago);
                                    Console.WriteLine($"\nPago registrado con ID: {newId}");
                                }
                                else
                                {
                                    Console.WriteLine("Monto inválido");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Socio no encontrado");
                            }
                        }
                        break;

                    case "3":
                        var now = DateTime.Now;
                        var pagosMes = await pagoManager.GetPagosByMonthAsync(now.Year, now.Month);
                        decimal total = 0;
                        
                        Console.WriteLine($"\nPagos del mes {now:MMMM yyyy}:");
                        foreach (var p in pagosMes)
                        {
                            var socio = await socioManager.GetByIdAsync(p.SocioId);
                            Console.WriteLine($"ID: {p.IdPago} | Socio: {socio?.Nombre} {socio?.Apellido} | Fecha: {p.FechaPago:dd/MM/yyyy} | Monto: ${p.Monto}");
                            total += p.Monto;
                        }
                        Console.WriteLine($"\nTotal recaudado: ${total}");
                        break;

                    case "4":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (!back)
            {
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    static async Task MenuAsistenciaAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== CONTROL DE ASISTENCIA ===");
            Console.WriteLine("1. Registrar entrada");
            Console.WriteLine("2. Registrar salida");
            Console.WriteLine("3. Ver asistencias por socio");
            Console.WriteLine("4. Ver asistencias del día");
            Console.WriteLine("5. Volver");

            Console.Write("\nSeleccione una opción: ");
            var opt = Console.ReadLine();

            try
            {
                switch (opt)
                {
                    case "1":
                        Console.Write("ID del Socio: ");
                        if (int.TryParse(Console.ReadLine(), out int idEntrada))
                        {
                            var socio = await socioManager.GetByIdAsync(idEntrada);
                            if (socio != null)
                            {
                                var asistencia = new Asistencia
                                {
                                    SocioId = idEntrada,
                                    Fecha = DateTime.Now.Date,
                                    HoraEntrada = DateTime.Now.TimeOfDay
                            };

                            var id = await asistenciaManager.AddAsistenciaAsync(asistencia);
                            Console.WriteLine($"Entrada registrada para {socio.Nombre} {socio.Apellido}");
                        }
                        else
                            Console.WriteLine("Socio no encontrado");
                    }
                    break;

                case "2":
                    Console.Write("ID del Socio: ");
                    if (int.TryParse(Console.ReadLine(), out int idSalida))
                    {
                        var asistencias = await asistenciaManager.GetAsistenciasByDateAsync(DateTime.Now.Date);
                        var ultima = asistencias.FirstOrDefault(a => a.SocioId == idSalida && !a.HoraSalida.HasValue);
                        
                        if (ultima != null)
                        {
                            ultima.HoraSalida = DateTime.Now.TimeOfDay;
                            await asistenciaManager.UpdateAsistenciaAsync(ultima);
                            Console.WriteLine("Salida registrada correctamente");
                        }
                        else
                            Console.WriteLine("No se encontró una entrada sin salida registrada para hoy");
                    }
                    break;

                case "3":
                    Console.Write("ID del Socio: ");
                    if (int.TryParse(Console.ReadLine(), out int idHistorial))
                    {
                        var socio = await socioManager.GetByIdAsync(idHistorial);
                        if (socio != null)
                        {
                            var asistencias = await asistenciaManager.GetAsistenciasBySocioAsync(idHistorial);
                            Console.WriteLine($"\nHistorial de asistencias de {socio.Nombre} {socio.Apellido}:");
                            foreach (var a in asistencias)
                            {
                                Console.WriteLine($"Fecha: {a.Fecha:dd/MM/yyyy} | Entrada: {a.HoraEntrada:hh\\:mm} | Salida: {(a.HoraSalida.HasValue ? a.HoraSalida.Value.ToString(@"hh\:mm") : "No registrada")}");
                            }
                        }
                        else
                            Console.WriteLine("Socio no encontrado");
                    }
                    break;

                case "4":
                    var hoy = await asistenciaManager.GetAsistenciasByDateAsync(DateTime.Now.Date);
                    Console.WriteLine($"\nAsistencias de hoy ({DateTime.Now:dd/MM/yyyy}):");
                    foreach (var a in hoy)
                    {
                        var socio = await socioManager.GetByIdAsync(a.SocioId);
                        Console.WriteLine($"Socio: {socio?.Nombre} {socio?.Apellido} | Entrada: {a.HoraEntrada:hh\\:mm} | Salida: {(a.HoraSalida.HasValue ? a.HoraSalida.Value.ToString(@"hh\:mm") : "No registrada")}");
                    }
                    break;

                case "5":
                    back = true;
                    break;

                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        if (!back)
        {
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }
    }
}

static async Task MenuReportesAsync()
{
    bool back = false;
    while (!back)
    {
        Console.Clear();
        Console.WriteLine("=== REPORTES ===");
        
        // Definir now al inicio del método
        var now = DateTime.Now;

        Console.WriteLine("1. Total de socios activos");
        Console.WriteLine("2. Ingresos del mes");
        Console.WriteLine("3. Socios con cuota vencida");
        Console.WriteLine("4. Asistencias por período");
        Console.WriteLine("5. Estadísticas generales");
        Console.WriteLine("6. Volver");

        Console.Write("\nSeleccione una opción: ");
        var opt = Console.ReadLine();

        try
        {
            switch (opt)
            {
                case "1":
                    var socios = await socioManager.GetAllAsync();
                    var activos = socios.Count(s => s.Estado == "Activo");
                    var inactivos = socios.Count(s => s.Estado == "Inactivo");
                    
                    Console.WriteLine("\nEstadísticas de Socios:");
                    Console.WriteLine($"Total de socios: {socios.Count}");
                    Console.WriteLine($"Socios activos: {activos}");
                    Console.WriteLine($"Socios inactivos: {inactivos}");
                    break;

                case "2":
                    var pagosMes = await pagoManager.GetPagosByMonthAsync(now.Year, now.Month);
                    decimal total = pagosMes.Sum(p => p.Monto);
                    
                    Console.WriteLine($"\nIngresos de {now:MMMM yyyy}:");
                    Console.WriteLine($"Total recaudado: ${total}");
                    Console.WriteLine($"Cantidad de pagos: {pagosMes.Count}");
                    Console.WriteLine($"Promedio por pago: ${total / (pagosMes.Count == 0 ? 1 : pagosMes.Count):F2}");
                    break;

                case "3":
                    var morosos = await reporteManager.GetSociosConCuotaVencidaAsync();
                    Console.WriteLine($"\nSocios con cuota vencida ({morosos.Count}):");
                    foreach (var m in morosos)
                    {
                        var ultimoPago = (await pagoManager.GetPagosBySocioAsync(m.Id)).FirstOrDefault();
                        Console.WriteLine($"ID: {m.Id} | {m.Nombre} {m.Apellido} | Último pago: {ultimoPago?.FechaPago.ToString("dd/MM/yyyy") ?? "Nunca"}");
                    }
                    break;

                case "4":
                    Console.Write("Ingrese fecha inicial (dd/MM/yyyy): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaInicial))
                    {
                        Console.Write("Ingrese fecha final (dd/MM/yyyy): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaFinal))
                        {
                            var asistencias = await asistenciaManager.GetAsistenciasByRangeAsync(fechaInicial, fechaFinal);
                            Console.WriteLine($"\nAsistencias del {fechaInicial:dd/MM/yyyy} al {fechaFinal:dd/MM/yyyy}:");
                            Console.WriteLine($"Total de asistencias: {asistencias.Count}");
                            Console.WriteLine($"Promedio diario: {asistencias.Count / (fechaFinal - fechaInicial).Days:F1}");
                        }
                    }
                    break;

                case "5":
                    Console.WriteLine("\nEstadísticas Generales:");
                    
                    var totalSocios = await socioManager.GetTotalCountAsync();
                    var sociosActivos = await socioManager.GetTotalSociosActivosAsync();
                    var ingresosMes = await pagoManager.GetIngresosMensualesAsync(now.Year, now.Month);
                    var asistenciasHoy = (await asistenciaManager.GetAsistenciasByDateAsync(DateTime.Now.Date)).Count;
                    
                    Console.WriteLine($"Total de socios: {totalSocios}");
                    Console.WriteLine($"Socios activos: {sociosActivos}");
                    Console.WriteLine($"Ingresos del mes: ${ingresosMes}");
                    Console.WriteLine($"Asistencias de hoy: {asistenciasHoy}");
                    break;

                case "6":
                    back = true;
                    break;

                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        if (!back)
        {
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }
    }
}

    // Métodos rápidos para Recepcionista
    static async Task RegistrarAsistenciaRapidaAsync()
    {
        Console.Write("ID del Socio: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido");
            return;
        }

        var asistencia = new Asistencia
        {
            SocioId = id,
            Fecha = DateTime.Now.Date,
            HoraEntrada = DateTime.Now.TimeOfDay
        };

        try
        {
            var newId = await asistenciaManager.AddAsistenciaAsync(asistencia);
            Console.WriteLine($"Asistencia registrada con ID: {newId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al registrar asistencia: {ex.Message}");
        }
    }

    static async Task ConsultarSocioRapidoAsync()
    {
        Console.Write("ID del Socio: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido");
            return;
        }

        var socio = await socioManager.GetByIdAsync(id);
        if (socio == null)
        {
            Console.WriteLine("Socio no encontrado");
            return;
        }

        Console.WriteLine($"\nDatos del Socio:");
        Console.WriteLine($"Nombre: {socio.Nombre} {socio.Apellido}");
        Console.WriteLine($"DNI: {socio.DNI}");
        Console.WriteLine($"Estado: {socio.Estado}");
        
        var pagos = await pagoManager.GetPagosBySocioAsync(id);
        if (pagos.Any())
        {
            Console.WriteLine("\nÚltimo pago:");
            var ultimoPago = pagos.First();
            Console.WriteLine($"Fecha: {ultimoPago.FechaPago:dd/MM/yyyy}");
            Console.WriteLine($"Monto: ${ultimoPago.Monto}");
        }
        else
        {
            Console.WriteLine("\nNo hay pagos registrados");
        }
    }

    static async Task RegistrarPagoRapidoAsync()
    {
        Console.Write("ID del Socio: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido");
            return;
        }

        Console.Write("Monto: $");
        if (!decimal.TryParse(Console.ReadLine(), out decimal monto))
        {
            Console.WriteLine("Monto inválido");
            return;
        }

        var pago = new Pago
        {
            SocioId = id,
            FechaPago = DateTime.Now,
            Monto = monto,
            Metodo = "Efectivo"
        };

        try
        {
            var newId = await pagoManager.AddPagoAsync(pago);
            Console.WriteLine($"Pago registrado con ID: {newId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al registrar pago: {ex.Message}");
        }
    }

    // Menú de Usuarios (solo Admin)
    static async Task MenuUsuariosAsync()
    {
        if (_currentUser?.Rol != "Administrador")
        {
            Console.WriteLine("Acceso denegado");
            return;
        }

        bool back = false;
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== GESTIÓN DE USUARIOS ===");
            Console.WriteLine("1. Listar usuarios");
            Console.WriteLine("2. Agregar usuario");
            Console.WriteLine("3. Cambiar rol");
            Console.WriteLine("4. Resetear contraseña");
            Console.WriteLine("5. Volver");
            
            Console.Write("Seleccione: ");
            var opt = Console.ReadLine();

            try
            {
                switch (opt)
                {
                    case "1":
                        var usuarios = await usuarioManager.GetAllAsync();
                        Console.WriteLine("\nID | Usuario | Mail | Rol");
                        foreach (var u in usuarios)
                        {
                            Console.WriteLine($"{u.Id} | {u.NombreUsuario} | {u.Mail} | {u.Rol}");
                        }
                        break;

                    case "2":
                        var nuevo = new Usuario();
                        Console.Write("Nombre de usuario: "); nuevo.NombreUsuario = Console.ReadLine() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(nuevo.NombreUsuario)) { Console.WriteLine("Usuario requerido."); break; }

                        // Evitar duplicados de nombre de usuario
                        if (await usuarioManager.ExistsByNombreAsync(nuevo.NombreUsuario))
                        {
                            Console.WriteLine("Ya existe un usuario con ese nombre.");
                            break;
                        }

                        Console.Write("Mail (opcional): "); nuevo.Mail = Console.ReadLine() ?? string.Empty;

                        Console.Write("Contraseña: ");
                        nuevo.Password = ReadPassword();
                        if (string.IsNullOrEmpty(nuevo.Password)) { Console.WriteLine("Contraseña requerida."); break; }

                        Console.WriteLine("Rol: 1) Administrador  2) Recepcionista  3) Profesor");
                        Console.Write("Seleccione: ");
                        var r = Console.ReadLine()?.Trim();
                        nuevo.Rol = r switch { "1" => "Administrador", "2" => "Recepcionista", "3" => "Profesor", _ => "Recepcionista" };

                        try
                        {
                            var newId = await usuarioManager.AddAsync(nuevo);
                            Console.WriteLine($"Usuario creado con ID: {newId}");
                        }
                        catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067) // UNIQUE constraint
                        {
                            Console.WriteLine("El mail ya existe. Intente con otro.");
                        }
                        break;

                    case "3":
                        Console.Write("ID de usuario: ");
                        if (!int.TryParse(Console.ReadLine(), out int idRol)) { Console.WriteLine("ID inválido"); break; }
                        var usr = await usuarioManager.GetByIdAsync(idRol);
                        if (usr == null) { Console.WriteLine("Usuario no encontrado."); break; }
                        Console.WriteLine($"Usuario: {usr.NombreUsuario} | Rol actual: {usr.Rol}");
                        Console.WriteLine("Nuevo rol: 1) Administrador  2) Recepcionista  3) Profesor");
                        Console.Write("Seleccione: ");
                        var nr = Console.ReadLine()?.Trim();
                        var nuevoRol = nr switch { "1" => "Administrador", "2" => "Recepcionista", "3" => "Profesor", _ => usr.Rol };
                        if (nuevoRol == usr.Rol) { Console.WriteLine("Sin cambios."); break; }
                        var okRol = await usuarioManager.UpdateRolAsync(idRol, nuevoRol);
                        Console.WriteLine(okRol ? "Rol actualizado." : "No se pudo actualizar el rol.");
                        break;

                    case "4":
                        Console.Write("ID de usuario: ");
                        if (!int.TryParse(Console.ReadLine(), out int idPwd)) { Console.WriteLine("ID inválido"); break; }
                        var userPwd = await usuarioManager.GetByIdAsync(idPwd);
                        if (userPwd == null) { Console.WriteLine("Usuario no encontrado."); break; }
                        Console.Write("Nueva contraseña: ");
                        var pass1 = ReadPassword();
                        if (string.IsNullOrEmpty(pass1)) { Console.WriteLine("Contraseña requerida."); break; }
                        Console.Write("Repetir contraseña: ");
                        var pass2 = ReadPassword();
                        if (pass1 != pass2) { Console.WriteLine("Las contraseñas no coinciden."); break; }
                        var okPwd = await usuarioManager.ResetPasswordAsync(idPwd, pass1);
                        Console.WriteLine(okPwd ? "Contraseña actualizada." : "No se pudo actualizar la contraseña.");
                        break;

                    case "5":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Opción inválida");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (!back)
            {
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    static async Task VerSociosActivosAsync()
    {
        var socios = await socioManager.GetAllAsync();
        var activos = socios.Where(s => string.Equals(s.Estado, "Activo", StringComparison.OrdinalIgnoreCase)).ToList();
        Console.WriteLine("\nSocios activos:");
        foreach (var s in activos)
        {
            var plan = s.PlanId.HasValue ? await planManager.GetByIdAsync(s.PlanId.Value) : null;
            Console.WriteLine($"ID:{s.Id} - {s.Nombre} {s.Apellido} - DNI:{s.DNI} - Plan:{plan?.NombrePlan ?? "Sin plan"}");
        }
    }

    static async Task ConsultarAsistenciasAsync()
    {
        Console.Write("Ingrese ID del socio para ver asistencias: ");
        if (!int.TryParse(Console.ReadLine() ?? "", out int id)) { Console.WriteLine("ID inválido."); return; }
        var socio = await socioManager.GetByIdAsync(id);
        if (socio == null) { Console.WriteLine("Socio no encontrado."); return; }

        var asistencias = await asistenciaManager.GetAsistenciasBySocioAsync(id);
        Console.WriteLine($"\nAsistencias de {socio.Nombre} {socio.Apellido}:");
        foreach (var a in asistencias)
        {
            Console.WriteLine($"Fecha: {a.Fecha:dd/MM/yyyy} | Entrada: {a.HoraEntrada:hh\\:mm} | Salida: {(a.HoraSalida.HasValue ? a.HoraSalida.Value.ToString(@"hh\:mm") : "—")}");
        }
    }

    static async Task VerPlanesActivosAsync()
    {
        // Evita mostrar planes duplicados (por ejemplo, si se ejecutó el seed varias veces).
        // Criterio: un plan único por nombre (ignorando mayúsculas/minúsculas y espacios).
        var planesUnicos = (await planManager.GetAllAsync())
            .GroupBy(p => (p.NombrePlan ?? string.Empty).Trim().ToUpperInvariant())
            .Select(g => g.OrderByDescending(x => x.Id).First()) // en caso de duplicados, muestra el más reciente
            .OrderBy(p => p.NombrePlan)
            .ToList();

        Console.WriteLine("\nPlanes disponibles:");
        if (planesUnicos.Count == 0)
        {
            Console.WriteLine("(No hay planes para mostrar)");
            return;
        }

        foreach (var p in planesUnicos)
        {
            Console.WriteLine($"ID:{p.Id} - {p.NombrePlan} - Duración: {p.DuracionDias}d - Precio: ${p.Precio} - {p.Descripcion}");
        }
    }
}