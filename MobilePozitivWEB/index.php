<?php
	require('config.php');
	require('template.php');
	require('web_service.php');	
	
	session_start();
	
	if (!isset($_SESSION["login"]))
	{
		toLoginScreen();
	}
	
	if (isset($_GET['action']))
	{		
		if ($_GET['action'] == "logout")
		{
			toLoginScreen();
		}
	}
	
	$parse->get_tpl('tpl/main.tpl'); //Файл который мы будем парсить
	$parse->set_tpl('{TITLE}', $title);
	$parse->set_tpl('{SITEPATH}', $site_path);
	$parse->set_tpl('{GROUPS}', getAllowGroups($ws_client));
	$parse->set_tpl('{PAGEDATA}', getPageData($ws_client));
	$parse->tpl_parse(); //Парсим
	print $parse->template; //Выводим нашу страничку
	
	function toLoginScreen()
	{
		require('config.php');
		header('HTTP/1.1 200 OK');
		header('Location: http://'.$_SERVER['HTTP_HOST'].$site_path."login.php");
		session_unset();
		session_destroy();
		exit();
	}
	
	function getAllowGroups($ws_client)
	{
		require('config.php');
		
		$soapResult = $ws_client->call("GetGroups");
		
		if ($soapResult['result'])
		{
			$jsonResult = json_decode($soapResult["data"]->return);
			$menuString = "";
			foreach ($jsonResult->Data as $jsonItem)
			{				
				$name = $jsonItem->Name;
				$ref = $jsonItem->Ref;
			
				$itemCount = "";
				if (isset($jsonItem->ItemCount))
				{
					$itemCount = "<span class=\"badge badge-default\">".$jsonItem->ItemCount."</span>";
				}
			
				if ($name == "Отчеты") 
				{
					$menuString .= "<li class=\"nav-item\"><a class=\"nav-link\" href=\"index.php?page=allowelements&type=reports&ref=$ref\">$name&nbsp;$itemCount</a></li>";
				}
				else
				{
					$menuString .= "<li class=\"nav-item\"><a class=\"nav-link\" href=\"index.php?page=allowelements&ref=$ref\">$name&nbsp;$itemCount</a></li>";
				}				
			}
			return $menuString;
		}
	}
	
	function getPageData($ws_client)
	{
		$pageData = "Ничего не найдено...";
		if (isset($_GET['page']))
		{
			if ($_GET['page'] == "allowelements")
			{
				$soapResult = $ws_client->call("GetElements", array("Ref" => $_GET['ref']));
				if ($soapResult['result'])
				{
					$jsonResult = json_decode($soapResult["data"]->return);					
					if (count($jsonResult->Data) > 0) $pageData = "<div class=\"sidebar-offcanvas\" id=\"sidebar\"><div class=\"list-group\">";
					foreach ($jsonResult->Data as $jsonItem)
					{
						$name = $jsonItem->Name;
						$description = $jsonItem->Description != ""? "(".$jsonItem->Description.")": "";
						$ref = $jsonItem->Ref;
						if (!isset($_GET["type"]))
						{
							$pageData .= "<a href=\"index.php?page=datalist&ref=$ref\" class=\"list-group-item\"><b>$name</b>&nbsp;$description</a>";
						}
						else
						{
							if ($_GET["type"] = "reports")
							{
								$pageData .= "<a href=\"index.php?page=dataview&lref=$ref&ref=$ref\" class=\"list-group-item\"><b>$name</b>&nbsp;$description</a>";
							}
						}
					}
					$pageData .= "</div></div>";
				}
				else
				{
					return $soapResult['data'];
				}
			}
			elseif ($_GET['page'] == "datalist")
			{
				$jsonStrinSessionParameters = json_encode($_SESSION["SessionParameters"]);
				$soapResult = $ws_client->call("GetList", array("Ref" => $_GET['ref'], "SessionPar" => $jsonStrinSessionParameters));
				if ($soapResult['result'])
				{
					$jsonResult = json_decode($soapResult["data"]->return);
					if (count($jsonResult->Data) > 0) $pageData = "<div class=\"sidebar-offcanvas\" id=\"sidebar\"><div class=\"list-group\">";
					foreach ($jsonResult->Data as $jsonItem)
					{
						$name = $jsonItem->Name;
						$description = $jsonItem->Description == ""? "": " (".$jsonItem->Description.")";
						$ref = $jsonItem->Ref;
						$lref = $_GET['ref'];
						$pageData .= "<a href=\"index.php?page=dataview&lref=$lref&ref=$ref\" class=\"list-group-item\"><b>$name</b>&nbsp;$description</a>";
					}
					$pageData .= "</div></div>";
				}
				else
				{
					return $soapResult['data'];
				}
			}
			elseif ($_GET['page'] == "dataview")
			{				
				$soapResult = $ws_client->call("GetData", array("RefListMod" => $_GET['lref'], "Ref" => $_GET['ref']));
				if ($soapResult['result'])
				{
					$pageData = "<form action=\"data.php\" method=\"post\"><div class=\"table-responsive\"><table class=\"table table-striped\">";
					$pageData .= "<input name=\"sys_pref\" type=\"hidden\" value=\"".$_SERVER['HTTP_REFERER']."\">";
					$pageData .= "<input name=\"sys_lref\" type=\"hidden\" value=\"".$_GET['lref']."\">";
					$pageData .= "<input name=\"sys_ref\" type=\"hidden\" value=\"".$_GET['ref']."\">";
					$jsonResult = json_decode($soapResult["data"]->return);
					foreach ($jsonResult->Data as $jsonItem)
					{
						$itsButton = $jsonItem->Type == "button";
						
						$pageData .= "<tr>";
						if ($itsButton)
						{
							$pageData .= "<td colspan=\"2\">";
						}	
						else
						{
							$name = $jsonItem->Name != ""? $jsonItem->Description : "";
							$pageData .= "<td>$name:</td>";						
							$pageData .= "<td width=\"100%\">";
						}
						
						switch ($jsonItem->Type)
						{
							case "button":
							$pageData .= "<button  name=\"btn_".$jsonItem->Name."\" type=\"submit\" class=\"btn btn-primary\" value=\"".$jsonItem->Value."\" id=\"".$jsonItem->Id."\">".$jsonItem->Description."</button>";
							break;
							//////////////////////////////////
							case "textview":
							if (isset($jsonItem->Param->SubType))
							{
								switch ($jsonItem->Param->SubType)
								{
									case "text":
									$pageData .= "<div style=\"white-space:pre-wrap;\">".$jsonItem->Value."</div>";
									break;
									case "phone":
									$pageData .= "<a href=\"tel:".$jsonItem->Value."\">".$jsonItem->Value."</a>";
									break;
								}
							}	
							else
							{
								$pageData .= $jsonItem->Value;
							}								
							break;
							//////////////////////////////////
							case "textedit":
							$readOnly = $jsonItem->Param->ReadOnly? " placeholder=\"Disabled input\"": "";
							if (isset($jsonItem->Param->SubType))
							{
								switch ($jsonItem->Param->SubType)
								{
									case "text":
									if ($jsonItem->Param->Multiline == false)
									{
										$pageData .= "<input name=\"val_".$jsonItem->Name."\" class=\"form-control\" type=\"text\" value=\"".$jsonItem->Value."\" id=\"".$jsonItem->Id."\" $readOnly>";
									}
									else
									{
										$pageData .= "<textarea name=\"val_".$jsonItem->Name."\" class=\"form-control\" value=\"".$jsonItem->Value."\" id=\"".$jsonItem->Id."\" rows=\"3\" $readOnly></textarea>";
									}
									break;
									case "number":
									$pageData .= "<input name=\"val_".$jsonItem->Name."\" class=\"form-control\" type=\"number\" value=\"".$jsonItem->Value."\" id=\"".$jsonItem->Id."\" $readOnly>";
									break;
									case "email":
									$pageData .= "<input name=\"val_".$jsonItem->Name."\" class=\"form-control\" type=\"email\" value=\"".$jsonItem->Value."\" id=\"".$jsonItem->Id."\" $readOnly>";
									break;
									case "password":
									$pageData .= "<input name=\"val_".$jsonItem->Name."\" class=\"form-control\" type=\"password\" value=\"".$jsonItem->Value."\" id=\"".$jsonItem->Id."\" $readOnly>";
									break;
									case "phone":
									$pageData .= "<input name=\"val_".$jsonItem->Name."\" class=\"form-control\" type=\"tel\" value=\"".$jsonItem->Value."\" id=\"".$jsonItem->Id."\" $readOnly>";
									break;
								}
							}	
							else
							{
								$pageData .= $jsonItem->Value;
							}
							break;
							//////////////////////////////////
							case "datalist":
							$pageData .= "<select name=\"val_".$jsonItem->Name."\" class=\"form-control\" id=\"".$jsonItem->Id."\">";
							foreach ($jsonItem->DataArray as $dataArrayItem)
							{
								//var_dump($dataArrayItem);
								if (isset($dataArrayItem->Data))
								{
									$selected = $dataArrayItem->Data == $jsonItem->Value? " selected": "";
									$pageData .= "<option value=\"".$dataArrayItem->Data."\"$selected>".$dataArrayItem->Present."</option>";
								}
								else
								{
									$selected = $dataArrayItem == $jsonItem->Value? " selected": "";
									$pageData .= "<option value=\"".$dataArrayItem."\"$selected>".$dataArrayItem."</option>";
								}
							}
							$pageData .= "</select>";
							break;
							//////////////////////////////////
							case "datalist":
							$pageData .= "<select name=\"val_".$jsonItem->Name."\" class=\"form-control\" id=\"".$jsonItem->Id."\">";
							foreach ($jsonItem->DataArray as $dataArrayItem)
							{
								if (isset($dataArrayItem->Data))
								{
									$selected = $dataArrayItem->Data == $jsonItem->Value? " selected": "";
									$pageData .= "<option value=\"".$dataArrayItem->Data."\"$selected>".$dataArrayItem->Present."</option>";
								}
								else
								{
									$selected = $dataArrayItem == $jsonItem->Value? " selected": "";
									$pageData .= "<option value=\"".$dataArrayItem."\"$selected>".$dataArrayItem."</option>";
								}
							}
							$pageData .= "</select>";
							break;
							//////////////////////////////////
							case "dateedit":	
							$pageData .= "<input name=\"val_".$jsonItem->Name."\" class=\"form-control\" type=\"date\" value=\"".substr($jsonItem->Value,0,10)."\" id=\"".$jsonItem->Id."\">";
							break;
							//////////////////////////////////
							case "timeedit":
							$pageData .= "<input name=\"val_".$jsonItem->Name."\" class=\"form-control\" type=\"time\" value=\"".substr($jsonItem->Value,11)."\" id=\"".$jsonItem->Id."\">";
							break;
							//////////////////////////////////
							case "data":
							$pageData .= "<div class=\"input-group\">";
							$pageData .= "<input name=\"vie_".$jsonItem->Name."\" class=\"form-control\" type=\"text\" value=\"".$jsonItem->Value->Present."\" id=\"".$jsonItem->Id."\" disabled>";
							$pageData .= "<span class=\"input-group-btn\">";
							$pageData .= "<button name=\"btn_".$jsonItem->Name."\" type=\"button\" class=\"btn btn-secondary\" id=\"".$jsonItem->Id."\" onclick=\"SelectData('".$jsonItem->Name."', '".$jsonItem->Description."','".$jsonItem->DataType."')\">Выбрать</button>";
							$pageData .= "</span></div>";							
							$pageData .= "<input name=\"val_".$jsonItem->Name."\" type=\"hidden\" value=\"".$jsonItem->Value->Ref."\">";
							break;
							//////////////////////////////////
							case "file":
							//<a class="btn btn-primary" href="#" role="button">Link</a>
							$pageData .= "<div class=\"input-group\">";
							$pageData .= "<div style=\"white-space:pre-wrap;\">".$jsonItem->Value->Present."</div>";
							$ref = $jsonItem->Value->Ref;
							$pageData .= "<a style=\"margin-left: 5px;\" class=\"btn btn-primary btn-sm\" href=\"index.php?page=file&ref=$ref\" role=\"button\">Открыть</a>";
							$pageData .= "</div>";							
							break;
						}
						$pageData .= "</td>";
						$pageData .= "</tr>";
					}
					return $pageData."</table></div></form>"; 
				}
				else
				{
					return $soapResult['data'];
				}
			}
			elseif ($_GET['page'] == "file")
			{
				$soapResult = $ws_client->call("GetFile", array("Ref" => $_GET['ref']));
				if ($soapResult['result'])
				{
					$jsonResult = json_decode($soapResult["data"]->return)->Data;	

					$name = $jsonResult->Name;
					$autor = $jsonResult->Autor;
					$version = $jsonResult->Version;
					$size = $jsonResult->Size;
					$extension = $jsonResult->Extension;
					$href = $jsonResult->Href;
					$moddate = date('d.m.Y H:i:s', strtotime(str_replace('T',' ', $jsonResult->ModDate)));
					
					$pageData = "<div class=\"table-responsive\"><table class=\"table table-striped\">";
					$pageData .= "<tr><td>Наименование:</td><td width=\"100%\">\"$name\"</td></tr>";		
					$pageData .= "<tr><td>Автор:</td><td>$autor</td></tr>";
					$pageData .= "<tr><td>Версия:</td><td>$version</td></tr>";
					$pageData .= "<tr><td>Размер:</td><td>$size</td></tr>";
					$pageData .= "<tr><td>Расширение:</td><td>$extension</td></tr>";
					$pageData .= "<tr><td>Дата редактирования:</td><td>$moddate</td></tr>";
					$pageData .= "<tr><td colspan=\"2\"><a class=\"btn btn-primary\" href=\"http://1c.pozitivtelecom.ru/Files/$href\" role=\"button\" target=\"_blank\">Скачать</a></td></tr>";
					return $pageData."</table></div>";
				}
				else
				{
					return $soapResult['data'];
				}				
			}
			else
			{
				
			}
		}
		else
		{
			//Новости
			$soapResult = $ws_client->call("GetNews");
			if ($soapResult['result'])
			{
				$jsonResult = json_decode($soapResult["data"]->return);	
				if (count($jsonResult->Data) > 0) $pageData = "";
				foreach ($jsonResult->Data as $jsonItem)
				{
					$title = $jsonItem->Title;
					$date = date('d.m.Y H:i:s', strtotime(str_replace('T',' ', $jsonItem->Date)));
					$text = $jsonItem->Text;
					$autor = $jsonItem->Autor;
					$pageData .= "<div class=\"jumbotron\" style=\"padding:15px; margin-bottom:10px;\"><h3>$title</h3><div style=\"white-space:pre-wrap;\"><p>$text</p></div><span class=\"badge badge-primary\">$date</span> <span class=\"badge badge-success\">$autor</span></div>";
				}
			}
			else
			{
				return $soapResult['data'];
			}
		}
		
		return $pageData;
	}
	
	function getRefByNaviLink($navikink)
	{
		return substr($navikink, strpos($navikink, "?ref=") + 5);
	}
?>