using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Infrastructure.Repositories
{
    public class EmailStringBody  // This class is used to generate the body of the email
                                  // that will be sent to the user when they register or reset their password.
    {
        public static string send(string email, string token, string component, string message)
        {
            string encodedToken = Uri.EscapeDataString(token);
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
                </style>
                </head>
                    <body>
                        <h1> Welcome to ECom, {email}!</h1>
                            <a class=""button"" href=""https://localhost:7076/api/Account/activate-email?email={email}&code={encodedToken}"">{message}</a>
                    </body>
                </head>
            </html>
                    ";
        }
    } 
}
