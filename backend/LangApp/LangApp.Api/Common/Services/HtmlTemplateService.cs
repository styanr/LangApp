using LangApp.Application.Auth.Options;

namespace LangApp.Api.Common.Services;

public class HtmlTemplateService : IHtmlTemplateService
{
    private const string CommonStyles = """
                                        body {
                                            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
                                            display: flex;
                                            justify-content: center;
                                            align-items: center;
                                            min-height: 100vh;
                                            margin: 0;
                                            background-color: #f5f5f5;
                                        }
                                        .container {
                                            text-align: center;
                                            padding: 2rem;
                                            background: white;
                                            border-radius: 8px;
                                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                                            max-width: 90%;
                                            width: 400px;
                                        }
                                        h1, h2 {
                                            margin-bottom: 1rem;
                                        }
                                        p {
                                            color: #34495e;
                                            line-height: 1.6;
                                            margin-bottom: 2rem;
                                        }
                                        .button {
                                            display: inline-block;
                                            background-color: #3498db;
                                            color: white;
                                            padding: 12px 24px;
                                            text-decoration: none;
                                            border-radius: 4px;
                                            transition: background-color 0.3s;
                                        }
                                        .button:hover {
                                            background-color: #2980b9;
                                        }
                                        .icon {
                                            width: 64px;
                                            height: 64px;
                                            margin-bottom: 1rem;
                                        }
                                        """;

    public string RenderEmailConfirmationSuccess()
    {
        return $$"""
                 <!DOCTYPE html>
                 <html>
                 <head>
                     <title>Email Confirmed - LangApp</title>
                     <meta charset='UTF-8'>
                     <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                     <style>
                         {{CommonStyles}}
                         h1 {color: #2c3e50;}
                     </style>
                 </head>
                 <body>
                     <div class='container'>
                         <svg class='icon' xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='#2ecc71'>
                             <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/>
                         </svg>
                         <h1>Email Confirmed!</h1>
                         <p>Your email address has been successfully verified. You can now sign in to your account.</p>
                     </div>
                 </body>
                 </html>
                 """;
    }

    public string RenderEmailConfirmationError()
    {
        return $$"""
                 <!DOCTYPE html>
                 <html>
                 <head>
                     <title>Email Confirmation Failed - LangApp</title>
                     <meta charset="UTF-8">
                     <meta name="viewport" content="width=device-width, initial-scale=1.0">
                     <style>
                         {{CommonStyles}}
                         h1 {
                             color: #c0392b;
                         }
                     </style>
                 </head>
                 <body>
                     <div class="container">
                         <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="#e74c3c">
                             <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/>
                         </svg>
                         <h1>Confirmation Failed</h1>
                         <p>Sorry, we couldn't verify your email address. The link may have expired or is invalid.</p>
                     </div>
                 </body>
                 </html>
                 """;
    }

    public string RenderDeepLinkRedirect(string deepLink, ClientAppOptions options)
    {
        return $$"""
                 <!DOCTYPE html>
                 <html lang='en'>
                 <head>
                     <meta charset='UTF-8'>
                     <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                     <title>Opening App...</title>
                     <style>
                         {{CommonStyles}}
                         .loader {
                             border: 4px solid #f3f3f3;
                             border-top: 4px solid #3498db;
                             border-radius: 50%;
                             width: 30px;
                             height: 30px;
                             animation: spin 1s linear infinite;
                             margin: 20px auto;
                         }
                         @keyframes spin {
                             0% { transform: rotate(0deg); }
                             100% { transform: rotate(360deg); }
                         }
                         #fallbackMessage {
                             display: none;
                             margin-top: 30px;
                             padding: 15px;
                             border: 1px solid #ddd;
                             border-radius: 4px;
                         }
                         .store-button {
                             display: inline-block;
                             background-color: #FF5722;
                             color: white;
                             padding: 10px 20px;
                             text-decoration: none;
                             border-radius: 4px;
                             margin-top: 10px;
                         }
                     </style>
                 </head>
                 <body>
                     <div class="container">
                         <h2>Opening the application...</h2>
                         <div class="loader"></div>
                         <p>If the app doesn't open automatically, please click the button below:</p>
                         <p><a href='{deepLink}' class="button">Open App</a></p>
                         
                         <div id="fallbackMessage">
                             <h3>App not installed?</h3>
                             <p>If you don't have the app installed, you can download it from:</p>
                             <p>
                                 <a href='{options.PlayStoreUrl}' class="store-button">Google Play</a>
                             </p>
                         </div>
                     </div>

                     <script>
                         // Try to open the app immediately
                         window.location.href = '{deepLink}';
                         
                         // Set a timeout to show the fallback message if the app doesn't open
                         setTimeout(function() {
                             document.getElementById('fallbackMessage').style.display = 'block';
                         }, 2000);
                     </script>
                 </body>
                 </html>
                 """;
    }
}