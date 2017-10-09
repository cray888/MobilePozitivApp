<?php
	session_start();
	
	require('config.php');
	require('template.php');
	require('web_service.php');
	
	$message = "";
	
	if (isset($_POST["action"]))
	{
		if ($_POST["action"] == "login")
		{
			$_SESSION["login"] = $_POST["login"];
			$_SESSION["password"] = $_POST["password"];
			
			$jsonstring = "";
			if (isset($_POST['signinwithapin']))
			{
				$_SESSION["password"] = "";
				$arr_SessionParameters = array("pin" => $_POST["password"]);
				$jsonstring = json_encode($arr_SessionParameters, JSON_UNESCAPED_UNICODE);
			}
			
			$soapResult = $ws_client->call("InitApp", array("Data" => $jsonstring));
			
			if ($soapResult['result'] == true) 
			{
				$jsonResult = json_decode($soapResult["data"]->return);
			
				if (isset($jsonResult->Data->Error))
				{
					session_unset();
					session_destroy();
					$message = '<div class="alert alert alert-warning" role="alert">'."Error: ".($jsonResult->Data->Error).'</div>';
				}
				else
				{
					$_SESSION["ApplicationLable"] = $jsonResult->Data->ApplicationLable;
					
					$sessionParameters = array();
					
					foreach ($jsonResult->Data->SessionParameters as $parametr)
					{
					var_dump($parametr);
						$sessionParameters[$parametr->Name] = $parametr->Value;
					}
					$_SESSION["SessionParameters"] = $sessionParameters;
			
					header('HTTP/1.1 200 OK');
					header('Location: http://'.$_SERVER['HTTP_HOST'].$site_path."index.php");
					if (isset($_POST['remember']))
					{
						session_set_cookie_params(60*60*24*30);
					}
					exit();
				}
			}
			else
			{
				session_unset();
				session_destroy();
				$message = '<div class="alert alert alert-warning" role="alert">'."Error: ".$soapResult['data'].'</div>';
			}
		}
	}
	
	$parse->get_tpl('tpl/login.tpl');
	$parse->set_tpl('{TITLE}', $title." (Авторизация)");
	$parse->set_tpl('{MESSAGE}', $message);
	$parse->tpl_parse();
	print $parse->template;
?>