<?php
	require('web_service.php');
	require('config.php');
	session_start();
	
	if (isset($_GET["action"]))
	{
		if ($_GET["action"] == "datalist")
		{
			$soapResult = $ws_client->call("GetList", array("Ref" => $_GET['ref']));
			if ($soapResult['result'])
			{
				$jsonResult = json_decode($soapResult["data"]->return);
				$pageData = "<div class=\"list-group\">";
				foreach ($jsonResult->Data as $jsonItem)
				{
					$name = $jsonItem->Name;
					$description = $jsonItem->Description == ""? "": " (".$jsonItem->Description.")";
					$ref = $jsonItem->Ref;
					$lref = $_GET['ref'];
					switch ($jsonItem->Image)
                    {
						case "Document":
						$image = "ic_document.png";
						break;
						case "DocumentDeleted":
						$image = "ic_document_deleted.png";;
						break;
						case "DocumentAccept":
						$image = "ic_document_accept.png";;
						break;
					}
					$pageData .= "<a href=\"#\" class=\"list-group-item list-group-item-action datalist_item\" id=\"datalist_item\" value=\"$ref\"><img width=\"32\" height=\"32\" src=\"res/$image\">&nbsp;<b>$name</b>&nbsp;$description</a>";
				}
				$pageData .= "</div>";
			}
			else
			{
				//return $soapResult['data'];
			}
		}
		echo $pageData;
	}
	else
	{
		//Если не передаем никаких данных то переходим в корень сайта.
		if (count($_POST)==0)
		{
			header("HTTP/1.1 200 OK");
			header("Location: $site_path");		
			exit();
		}
		
		$id = 1;
		$dataArray = array();
		foreach ($_POST as $key=>$value)
		{
			$name = substr($key, 4);
			if (substr($key, 0, 4) == "btn_")
			{
				$dataArray["0"] = array("Name" => $name, "Data" => $value);
			}
			elseif (substr($key, 0, 4) == "val_")
			{
				$dataArray[$id] = array("Name" => $name, "Data" => $value);
			}
			else continue;
			$id++;
		}
		$jsonString = json_encode($dataArray, JSON_UNESCAPED_UNICODE);	
		$soapResult = $ws_client->call("SetData", array("RefListMod" => $_POST['sys_lref'], "Ref" => $_POST['sys_ref'], "Data" => $jsonString));	
		if ($soapResult["result"])
		{
			$soapString = str_replace("\n", "", $soapResult["data"]->return);
			$jsonData = json_decode($soapString);
			switch ($jsonData->Result)
			{
				case "CompletedClose":
				goToList();
				break;
				case "Completed":
				goBack();
				break;
				case "ErrorClose":
				goToList();
				break;
				case "Error":
				goBack();
				break;
				case "ReportGenerated":
				echo base64_decode($jsonData->Data);
				break;
			}
		}
		else
		{
			//если ошибка
		}
	}
	
	function goBack()
	{
		header('HTTP/1.1 200 OK');
		header('Location: '.$_SERVER['HTTP_REFERER']);
	}
	
	function goToList()
	{
		header('HTTP/1.1 200 OK');
		header('Location: '.$_POST["sys_pref"]);
	}
?>