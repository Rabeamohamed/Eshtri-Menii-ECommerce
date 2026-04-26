

namespace ECom.Infrastructure.Repositories
{
    public class EmailStringBody  // This class is used to generate the body of the email
                                  // that will be sent to the user when they register or reset their password.
    {
        public static string send(string email, string token, string component, string message)
        {
            string encodedToken = Uri.EscapeDataString(token);
            
            // Generate different links based on component type
            string actionLink = component == "Reset-Password" 
                ? $"https://localhost:4200/reset-password?email={email}&token={encodedToken}"
                : $"https://localhost:7076/api/Account/activate-email?email={email}&code={encodedToken}";
            
            string title = component == "Reset-Password" 
                ? "Password Reset Request" 
                : $"Welcome to ECom, {email}!";
            
            return $@"
            <html> 
                <head>
                    <style>
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                        }}
                        .header {{
                            background-color: #007BFF;
                            color: white;
                            padding: 10px 20px;
                            text-align: center;
                        }}
                        .content {{
                            padding: 20px;
                            background-color: white;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            color: white;
                            background-color: #007BFF;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            padding: 10px;
                            font-size: 12px;
                            color: #666;
                        }}
                </style>
                </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h2>ECom Notification</h2>
                            </div>
                            <div class=""content"">
                                <h1>{title}</h1>
                                <p>{message}</p>
                                <a class=""button"" href=""{actionLink}"">Click Here</a>
                                <div class=""footer"">
                                    <p>If you did not request this action, please ignore this email.</p>
                                    <p>This link will expire in 24 hours.</p>
                                </div>
                            </div>
                        </div>
                    </body>
            </html>
                    ";
        }
    } 
}
