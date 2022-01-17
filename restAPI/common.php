<?php
function checkLoggedIn($u, $p) {
	global $con;
	$result = $con -> query("SELECT id, name, password,Admin FROM users WHERE name = '$u' AND password = '$p'");
	return $result->fetch_all(MYSQLI_ASSOC)[0];
}
?>