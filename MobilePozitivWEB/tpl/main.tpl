<!DOCTYPE html>
<html lang="en">
	<head>
		<meta charset="utf-8">
		<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
		<meta name="description" content="">
		<meta name="author" content="">
		<link rel="icon" href="res/favicon.ico">
		
		<title>{TITLE}</title>
		
		<!-- Bootstrap core CSS -->
		<link href="css/bootstrap.min.css" rel="stylesheet">
		
		<!-- Custom styles for this template -->
		<link href="css/starter-template.css" rel="stylesheet">
			
		<!-- SendPulse
		<script charset="UTF-8" src="//cdn.sendpulse.com/28edd3380a1c17cf65b137fe96516659/js/push/eded9601f5e3de62adfa863d705cc1a0_0.js" async></script> -->
	</head>
	
	<body>
		
		<nav class="navbar navbar-toggleable-md navbar-inverse bg-inverse fixed-top">
			<button class="navbar-toggler navbar-toggler-right" type="button" data-toggle="collapse" data-target="#navbarsExampleDefault" aria-controls="navbarsExampleDefault" aria-expanded="false" aria-label="Toggle navigation">
				<span class="navbar-toggler-icon"></span>
			</button>
			<a class="navbar-brand" href="{SITEPATH}">Главная</a>
			
			<div class="collapse navbar-collapse" id="navbarsExampleDefault">
				<ul class="navbar-nav mr-auto">
					{GROUPS}
				</ul>
				<a class="navbar-brand" href="index.php?action=logout">Выход</a>
				<!--<form class="form-inline my-2 my-lg-0">
					<input class="form-control mr-sm-2" type="text" placeholder="Поиск">
					<button class="btn btn-outline-success my-2 my-sm-0" type="submit">Найти</button>
				</form>-->
			</div>
		</nav>
		
		<div class="container">	
			{PAGEDATA}
		</div><!-- /.container -->
		
		
		<!-- Bootstrap core JavaScript
		================================================== -->
		<!-- Placed at the end of the document so the pages load faster -->
		<script src="js/jquery-3.1.1.js"></script>
		<script>window.jQuery || document.write('<script src="js/jquery.min.js"><\/script>')</script>
		<script src="https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js" integrity="sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb" crossorigin="anonymous"></script>
		<script src="js/bootstrap.min.js"></script>
		<!-- Подключения скрипта control-modal.min.js к странице -->
		<script src="js/control-modal.js"></script>
		<script>		
			var myModal = new ModalApp.ModalProcess({ id: 'myModal', title: '' });
			myModal.init();
			function SelectData(name, title, ref)
			{			
				myModal.changeTitle('Данные загружаются...');
				myModal.changeBody('<img src="res/anim_laoding.gif" class="rounded mx-auto d-block">');
				myModal.showModal();
				$.get('data.php?action=datalist&ref=' + ref, function(data) {
					myModal.changeTitle(title);
					myModal.changeBody(data);
					
					$('#search_in_datalist').keyup(function(){
						_this = this;						
						$.each($('.datalist_item'), function() {
							if($(this).text().toLowerCase().indexOf($(_this).val().toLowerCase()) == -1) {
								$(this).hide();
								} else {
								$(this).show();                
							};
						});						
					});
					$('.datalist_item').click(function(){
						description = $(this).text();
						ref = $(this).attr("value");	
						$('input[name=vie_'+name+']').val(description);
						$('input[name=val_'+name+']').val(ref);
						myModal.hideModal();
					});
				});
			}
		</script>
		
		<!-- IE10 viewport hack for Surface/desktop Windows 8 bug -->
		<script src="js/ie10-viewport-bug-workaround.js"></script>
	</body>
</html>
