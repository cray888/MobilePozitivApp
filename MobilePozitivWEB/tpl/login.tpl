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
    <link href="css/signin.css" rel="stylesheet">
	
	<!-- Custom styles for this template -->
    <link href="css/sticky-footer.css" rel="stylesheet">
	<!-- SendPulse -->
	<script charset="UTF-8" src="//cdn.sendpulse.com/28edd3380a1c17cf65b137fe96516659/js/push/eded9601f5e3de62adfa863d705cc1a0_0.js" async></script>
  </head>

  <body>
	{MESSAGE}
    <div class="container">

      <form class="form-signin" action="login.php" method="post">
        <h2 class="form-signin-heading">Вход:</h2>
        
		<label for="inputLogin" class="sr-only">Логин</label>
        <input name="login" type="login" id="inputLogin" class="form-control" placeholder="Логин" required autofocus>
        
		<label for="inputPassword" class="sr-only">Пароль</label>
        <input name="password" type="password" id="inputPassword" class="form-control" placeholder="Пароль">
		
		<div class="checkbox">
          <label>
            <input name="signinwithapin" type="checkbox" value="true"> Войти через PIN
          </label>
        </div>
		
        <div class="checkbox">
          <label>
            <input name="remember" type="checkbox" value="true"> Запомнить меня
          </label>
        </div>
		
        <button name="action" value="login" class="btn btn-lg btn-primary btn-block" type="submit">Вход</button>
      </form>

    </div> <!-- /container -->	
	
	<footer class="footer">
		<a href="http://1c.pozitivtelecom.ru/MobileApp/app.apk"><img src="res/apk-android_n.png" width="86" height="50"></a>
    </footer>

    <!-- Bootstrap core JavaScript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <!-- IE10 viewport hack for Surface/desktop Windows 8 bug -->
    <script src="js/ie10-viewport-bug-workaround.js"></script>
	
  </body>
</html>
