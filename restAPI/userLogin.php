<?php
include("db.php");
$request = $_SERVER['REQUEST_METHOD'];

switch ($request) {
	case "GET":
		if (!empty($_GET["username"]) && !empty($_GET["password"])) {
			$users = login($_GET["username"], $_GET["password"]);
            if ($users === False) {
                echo json_encode($users);
                break;
            }
            else {
                echo json_encode($users);
                break;
            }
		}
		else {
			$users = null;
            break;
		}
	default:
		header('HTTP/1.1 405 Method Not Allowed');
		header('Allow: GET');
		break;
}
function login($u, $p) {
	global $con;
	$result = count($con -> query("SELECT * FROM users WHERE name = '$u' AND password = MD5('$p')")->fetch_all());
	if ($result > 0) {
	    return $con -> query("SELECT * FROM users WHERE name = '$u' AND password = MD5('$p')")->fetch_all(MYSQLI_ASSOC)[0];
	}
    else {
        return False;
    }
}
?>