<?php
ini_set('soap.wsdl_cache_enabled', 0);
class ws_client_class
{
	function call($method, $param = array())
	{
		require('config.php');
		
		$login = $_SESSION["login"];
		$password = $_SESSION["password"];
		
		$retVal = array();
		
		$base64LoginData = base64_encode(iconv(mb_detect_encoding($login, mb_detect_order(), true), "UTF-8", $login).":".iconv(mb_detect_encoding($password, mb_detect_order(), true), "UTF-8", $password));
		try 
		{
			$client = new SoapClient($ws_url, 
				array( 
					'exceptions' => true,
					'trace' => true, 
					'encoding' => 'UTF-8',
					'features' => SOAP_USE_XSI_ARRAY_TYPE,
					'stream_context' => stream_context_create(
						array(
							'http' => array(
								'header' => 'Authorization: Basic '.$base64LoginData
							)
						)
					)
				) 
			);			
		}
		catch (SoapFault  $e)
		{
			$retVal['result'] = false;
			$retVal['data'] = "SOAP-ERROR: Авторизация не выполнена";
			return $retVal;
		}
		
		try 
		{
			$result = $client->$method($param);
		}
		catch (SoapFault $e)
		{
			var_dump($e);
			$retVal['result'] = false;
			$retVal['data'] = "SOAP-ERROR: ошибка инициализации";
			return $retVal;
		}
		
		$retVal['result'] = true;
		$retVal['data'] = $result;
		return $retVal;
	}
}
$ws_client = new ws_client_class;
?>