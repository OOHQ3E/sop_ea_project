<?php
include("db.php");
include("common.php");
$request = $_SERVER['REQUEST_METHOD'];

switch ($request) {
	case "GET":
		$reservations = getReservations();
		echo json_encode($reservations);
		break;
	default:
		header('HTTP/1.1 405 Method Not Allowed');
		header('Allow: GET');
		break;
}
function getReservations() {
	global $con;
	$result = $con -> query("SELECT id, reservedBy, seatRow, seatColumn FROM reservation");
	return $result->fetch_all(MYSQLI_ASSOC);
}
?>