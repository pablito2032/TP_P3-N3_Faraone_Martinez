<?php

$server  = "localhost";
$db      = "mi_banco_db";
$db_user = "root";
$db_pass = "root";

$conn = new mysqli($server, $db_user, $db_pass, $db);
if ($conn->connect_error) {
    die("Error al conectar la base de datos: " . $conn->connect_error);
}

$nombre       = $_POST['nombre'];
$apellido     = $_POST['apellido'];
$tipo_doc     = $_POST['tipo_doc'];
$documento    = $_POST['documento'];
$fecha_nac    = $_POST['fecha_nacimiento'];
$email        = $_POST['email'];
$password     = $_POST['passwordA'];
$usuario      = $_POST['usuario'];

$sqlVerificar = "SELECT u.documento 
                    FROM usuarios u
                    JOIN tarjetas t ON t.dni_titular = u.documento
                    WHERE u.documento = '$documento' AND u.tipo_doc = '$tipo_doc'";

$result = $conn->query($sqlVerificar);

if ($result && $result->num_rows > 0) {

    $sqlActualizar = "UPDATE usuarios 
                        SET usuario = '$usuario', `password` = '$password' 
                        WHERE documento = '$documento' AND tipo_doc = '$tipo_doc'";

    if ($conn->query($sqlActualizar) === TRUE) {
        echo "Cuenta web activada con éxito.";
    } else {
        echo "Error al activar la cuenta: " . $conn->error;
    }

} else {
    echo "Error: No existe una tarjeta registrada por el banco para ese documento. 
            Consulte con su entidad financiera.";
}

$conn->close();
?>
