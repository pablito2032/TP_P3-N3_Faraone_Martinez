<?php

session_start();

$server  = "localhost";
$db      = "mi_banco_db";
$db_user = "root";
$db_pass = "root";

$conn = new mysqli ($server, $db_user, $db_pass, $db);
if ($conn->connect_error){
    die ("Error al conectar la base de datos: ". $conn->connect_error);
}

$tipo_doc = $_POST['tipo_doc'];
$numero   = $_POST['documento'];
$usuario  = $_POST['usuario'];
$password = $_POST['password'];

$sql = "SELECT documento, `password` FROM usuarios
            WHERE usuario = '$usuario'
            AND tipo_doc  = '$tipo_doc'
            AND documento = '$numero'";

$result = $conn->query($sql);

if ($result && $result->num_rows > 0){
    $row = $result->fetch_assoc();

    if ($password === $row['password']){
        $_SESSION['documento'] = $row['documento'];
        $_SESSION['usuario']   = $usuario;

        header("Location: resumen.php");
        exit();

    } else {
        echo "Usuario o contraseña incorrectos";
    }
} else {
    echo "Usuario o contraseña incorrectos";
}

$conn->close();
?>