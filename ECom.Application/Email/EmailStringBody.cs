namespace ECom.Application.Email
{
    public static class EmailStringBody
    {
        public static string Send(string email, string token, string component, string message, string baseUrl)
        {
            string encodedToken = Uri.EscapeDataString(token);

            baseUrl = baseUrl.TrimEnd('/');

            string actionLink = component == "Reset-Password"
                ? $"http://localhost:4200/reset-password?email={email}&token={encodedToken}"
                : $"http://localhost:4200/activate-email?email={email}&code={encodedToken}";

            string title = component == "Reset-Password"
                ? "Password Reset Request"
                : "Welcome to ECom!";

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
