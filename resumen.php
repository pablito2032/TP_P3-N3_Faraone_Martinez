<?php

session_start();

if (!isset($_SESSION['documento'])){
    header("Location: ingreso.html");
    exit();
}

$documento = $_SESSION['documento'];

$server  = "localhost";
$db      = "mi_banco_db";
$db_user = "root";
$db_pass = "root";

$conn = new mysqli($server, $db_user, $db_pass, $db);
if ($conn->connect_error) {
    die("Error al conectar la base de datos: " . $conn->connect_error);
}

$sqlCliente = "SELECT u.nombre, u.apellido, t.num_cuenta, t.numero_tarjeta,
                    t.banco_emisor, t.estado, t.saldo
                FROM usuarios u
                JOIN tarjetas t ON t.dni_titular = u.documento
                WHERE u.documento = '$documento'";

$resultCliente = $conn->query($sqlCliente);

if ($resultCliente->num_rows === 0){
    die ("No se encontró una tarjeta asociada a su cuenta.");
}

$cliente = $resultCliente->fetch_assoc();
$numCuenta = $cliente['num_cuenta'];

//liquidacion
$sqlActual = "SELECT periodo, fecha_vencimiento, total_a_pagar, pago_minimo
                FROM liquidaciones
                WHERE num_cuenta = '$numCuenta'
                ORDER BY periodo DESC
                LIMIT 1";

$resultActual = $conn->query($sqlActual);
$liquidacionActual = $resultActual->fetch_assoc();

//historial de liqui. (menos la mas reciente)
$sqlHistorial = "SELECT periodo, fecha_vencimiento, total_a_pagar, pago_minimo
                    FROM liquidaciones
                    WHERE num_cuenta = '$numCuenta'
                    ORDER BY periodo DESC 
                    LIMIT 100 OFFSET 1";

$resultHistorial = $conn->query($sqlHistorial);
$historial = $resultHistorial->fetch_all(MYSQLI_ASSOC);

$conn->close();
?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mis Tarjetas - Resumen de Cuenta</title>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Fraunces:opsz,wght@9..144,500;9..144,600&family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="resumen.css">
</head>
<body>

    <header class="site-header">
        <h1>Mis <span>Tarjetas</span></h1>
    </header>

    <main class="resumen-container">

        <!-- Saludo y datos del cliente/tarjeta -->
        <section class="card card-cliente">
            <div class="card-cliente-top">
                <div>
                    <p class="eyebrow">Hola de nuevo</p>
                    <h2><?php echo $cliente['nombre'] . " " . $cliente['apellido']; ?></h2>
                </div>
                <span class="badge badge-<?php echo $cliente['estado'] === 'Activa' ? 'activa' : 'bloqueada'; ?>">
                    <?php echo $cliente['estado']; ?>
                </span>
            </div>

            <div class="card-cliente-bottom">
                <div class="dato-tarjeta">
                    <span class="dato-label">Tarjeta</span>
                    <span class="dato-valor mono">•••• •••• •••• <?php echo substr($cliente['numero_tarjeta'], -4); ?></span>
                </div>
                <div class="dato-tarjeta">
                    <span class="dato-label">Banco emisor</span>
                    <span class="dato-valor"><?php echo $cliente['banco_emisor']; ?></span>
                </div>
                <div class="dato-tarjeta">
                    <span class="dato-label">Saldo actual</span>
                    <span class="dato-valor monto-grande">$<?php echo number_format($cliente['saldo'], 2); ?></span>
                </div>
            </div>
        </section>

        <!-- Liquidación actual -->
        <section class="card card-liquidacion">
            <h3>Última liquidación</h3>

            <?php if ($liquidacionActual): ?>
                <div class="grid-liquidacion">
                    <div>
                        <span class="dato-label-dark">Período</span>
                        <span class="dato-valor-dark"><?php echo $liquidacionActual['periodo']; ?></span>
                    </div>
                    <div>
                        <span class="dato-label-dark">Vencimiento</span>
                        <span class="dato-valor-dark"><?php echo $liquidacionActual['fecha_vencimiento']; ?></span>
                    </div>
                    <div>
                        <span class="dato-label-dark">Total a pagar</span>
                        <span class="dato-valor-dark monto">$<?php echo number_format($liquidacionActual['total_a_pagar'], 2); ?></span>
                    </div>
                    <div>
                        <span class="dato-label-dark">Pago mínimo</span>
                        <span class="dato-valor-dark monto">$<?php echo number_format($liquidacionActual['pago_minimo'], 2); ?></span>
                    </div>
                </div>
            <?php else: ?>
                <p class="vacio">Todavía no tenés liquidaciones emitidas.</p>
            <?php endif; ?>
        </section>

        <!-- Historial de liquidaciones anteriores -->
        <section class="card card-historial">
            <h3>Historial de liquidaciones</h3>

            <?php if (count($historial) > 0): ?>
                <table>
                    <thead>
                        <tr>
                            <th>Período</th>
                            <th>Vencimiento</th>
                            <th>Total a pagar</th>
                            <th>Pago mínimo</th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php foreach ($historial as $liquidacion): ?>
                            <tr>
                                <td><?php echo $liquidacion['periodo']; ?></td>
                                <td><?php echo $liquidacion['fecha_vencimiento']; ?></td>
                                <td class="monto">$<?php echo number_format($liquidacion['total_a_pagar'], 2); ?></td>
                                <td class="monto">$<?php echo number_format($liquidacion['pago_minimo'], 2); ?></td>
                            </tr>
                        <?php endforeach; ?>
                    </tbody>
                </table>
            <?php else: ?>
                <p class="vacio">No hay liquidaciones anteriores para mostrar.</p>
            <?php endif; ?>
        </section>

    </main>

    <footer class="site-footer">
        Portal Oficial de Consultas de Liquidaciones Progra3card.
    </footer>
</body>
</html>

