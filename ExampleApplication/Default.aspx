<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ExampleApplication.Default" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>NoCaptcha for ASP.NET - Example</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="//maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]>
	<script src="/site/js/html5shiv.js"></script>
	<script src="/site/js/respond.min.js"></script>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
    <![endif]-->
    <!--[if gte IE 9]><!-->
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script>
    <!--<![endif]-->
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="baseFrm" runat="server">
        <div class="container">
            <h1>Example NoCAPTCHA For ASP.NET</h1>
           <h3>Default Options</h3>
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="col-sm-5 col-sm-offset-2">
                        <nc:NoCaptchaControl runat="server" SiteKey="6Ld5aQsTAAAAAKdsNZ0GfSGqm71cJs4VkaCPFbxU" SecretKey="6Ld5aQsTAAAAADrl9-25nDCNHTOxbCIGWaGjm04M" ValidationGroup="Group1" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-5 col-sm-offset-2">
                        <asp:Button ID="testButton" runat="server" CssClass="btn btn-primary" Text="Submit" OnClick="testButton_Click" ValidationGroup="Group1" />
                    </div>
                </div>
            </div>
           <h3>Custom Options</h3>
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="col-sm-5 col-sm-offset-2">
                        <nc:NoCaptchaControl runat="server" SiteKey="6Ld5aQsTAAAAAKdsNZ0GfSGqm71cJs4VkaCPFbxU" SecretKey="6Ld5aQsTAAAAADrl9-25nDCNHTOxbCIGWaGjm04M" Theme="Dark" Type="Audio" Size="Compact" ValidationGroup="Group2" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-5 col-sm-offset-2">
                        <asp:Button ID="altTestButton" runat="server" CssClass="btn btn-primary" Text="Submit" OnClick="testButton_Click" ValidationGroup="Group2" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
